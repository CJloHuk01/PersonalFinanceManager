using PersonalFinanceManager.Enum;
using System;
using System.Collections.Generic;
using System.Text;


namespace PersonalFinanceManager.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CategoryType Type { get; set; }
        public TransactionType CategoryType { get; set; }
        public string Color { get; set; } = "#808080";

        public int? ParentCategoryId { get; set; }
        public virtual Category? ParentCategory { get; set; }
        public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
