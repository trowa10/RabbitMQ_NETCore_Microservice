using System;
using System.Collections.Generic;
using System.Text;

namespace MicroRabbit.Domain.Core.Events
{
    public abstract class Event
    {
        public Event()
        {
            TimeStamp = DateTime.Now;
        }
        public DateTime TimeStamp { get; protected set; }
    }
}
