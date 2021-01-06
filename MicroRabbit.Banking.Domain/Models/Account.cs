using System;
using System.Collections.Generic;
using System.Text;

namespace MicroRabbit.Banking.Domain.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string AcountType { get; set; }
        public decimal AutoBalance { get; set; }
    }
}
