using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Banking.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroRabbit.Banking.Data.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private BankingDBContext _bankingDBContext;
        public AccountRepository(BankingDBContext bankingDBContext)
        {
            _bankingDBContext = bankingDBContext;
        }
        public IEnumerable<Account> GetAccounts()
        {
            return _bankingDBContext.Accounts;
        }
    }
}
