using MediatR;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Domain.Core.Commands;
using MicroRabbit.Domain.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroRabbit.Infra.Bus
{
    public sealed class RabbitMQBus : IEventBus
    {
        private readonly IMediator _mediator;
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _eventTypes;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQBus(IMediator mediator, IServiceScopeFactory serviceScopeFactory)
        {
            _mediator = mediator;
            _serviceScopeFactory = serviceScopeFactory;
            _handlers = new Dictionary<string, List<Type>>();
            _eventTypes = new List<Type>();

        }

        public Task SendCommand<T>(T command) where T : Command
        {
            return _mediator.Send(command);
        }

        public void Publish<T>(T @event) where T : Event
        {
            //RabbitMQ Syntax
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection()) //Open connection
            using (var channel = connection.CreateModel()) //Open Channel
            {
                var eventName = @event.GetType().Name;
                channel.QueueDeclare(eventName, false, false, false, null); //Declare queue
                string message = JsonConvert.SerializeObject(@event); //Create a message
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("", eventName, null, body); //Publish Message             

            }
        }

        public void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            if (!_eventTypes.Contains(typeof(T)))
            {
                _eventTypes.Add(typeof(T));
            }

            if (!_handlers.ContainsKey(eventName))
            {
                _handlers.Add(eventName, new List<Type>());
            }

            if (_handlers[eventName].Any(s => s.GetType() == handlerType))
            {
                throw new ArgumentException($"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            _handlers[eventName].Add(handlerType);
            StartBasicConsume<T>();
        }

        private void StartBasicConsume<T>() where T : Event
        {
            var factory = new ConnectionFactory() { HostName = "localhost", DispatchConsumersAsync = true };
            using (var connection = factory.CreateConnection()) //Open connection
            using (var channel = connection.CreateModel()) //Open Channel
            {
                var eventName = typeof(T).Name;
                channel.QueueDeclare(eventName, false, false, false, null); //Declare queue
                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += Consumer_Received; //delegate
                channel.BasicConsume(eventName, true, consumer); // Acknowledge message
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            { }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_handlers.ContainsKey(eventName))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var subsCriptions = _handlers[eventName];
                    foreach (var subsCription in subsCriptions)
                    {
                        //var handler = Activator.CreateInstance(subsCription); //create new instance of type class which is generi class
                        var handler = scope.ServiceProvider.GetService(subsCription);//create new instance of type class which is generi class
                        if (handler == null) continue;
                        var evenType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                        var @event = JsonConvert.DeserializeObject(message, evenType);
                        var concreteType = typeof(IEventHandler<>).MakeGenericType(evenType);
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { @event });
                    }
                }

            }
        }
    }
}
