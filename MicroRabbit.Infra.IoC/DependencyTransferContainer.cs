
using MicroRabbit.Transfer.Domain.Events;
using MicroRabbit.Domain.Core.Bus;
using MicroRabbit.Transfer.Application.Interfaces;
using MicroRabbit.Transfer.Application.Services;
using MicroRabbit.Transfer.Data.Context;
using MicroRabbit.Transfer.Data.Repository;
using MicroRabbit.Transfer.Domain.EventHandlers;
using MicroRabbit.Transfer.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MicroRabbit.Infra.IoC
{
    public class DependencyTransferContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {
            //Subscription
            services.AddTransient<TransferEventHandler>();

            //Domain Events
            services.AddTransient<IEventHandler<TransferCreatedEvent>, TransferEventHandler>();

            //Applicationn Services
            services.AddTransient<ITransferService, TransferService>();

            //Data
            services.AddTransient<ITransferRepository, TransfeRepository>();
            services.AddTransient<TransferDBContext>();
        }
    }
}
