using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroRabbit.Domain.Core.Events
{

    public abstract class Message : IRequest<bool>
    {
        public Message()
        {
            Messagetype = GetType().Name;
        }
        public string Messagetype  { get; protected set; }
    }
}
