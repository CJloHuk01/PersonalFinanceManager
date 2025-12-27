using PersonalFinanceManager.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace PersonalFinanceManager.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = string.Empty;
        public TransactionType TransactionType { get; set; }

        public int AccountId { get; set; }
        public virtual Account Account { get; set; } = null!;

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;

    }
}
