using System;
using System.Collections.Generic;
using System.Text;

namespace MicroRabbit.Banking.Domain.Commands
{
    public class CreateTransferCommand : TansferCommand
    {
        public CreateTransferCommand(int from, int to, decimal amounnt)
        {
            From = from;
            To = to;
            Amount = amounnt;
        }
    }
}
