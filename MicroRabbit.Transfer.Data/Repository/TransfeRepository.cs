using MicroRabbit.Transfer.Data.Context;
using MicroRabbit.Transfer.Domain.Interfaces;
using MicroRabbit.Transfer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroRabbit.Transfer.Data.Repository
{
    public class TransfeRepository : ITransferRepository
    {
        private TransferDBContext _transferDBContext;
        public TransfeRepository(TransferDBContext transferDBContext)
        {
            _transferDBContext = transferDBContext;
        }

        public void Add(TransferLog transferLog)
        {
            _transferDBContext.TransferLogs.Add(transferLog);
            _transferDBContext.SaveChanges();
        }

        public IEnumerable<TransferLog> GetTransferLogs()
        {
            return _transferDBContext.TransferLogs;
        }
    }
}
