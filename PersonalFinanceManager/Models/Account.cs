using PersonalFinanceManager.Enum;


namespace PersonalFinanceManager.Models;

public class Account
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public AccountType AccountType { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string Color { get; set; } = "#007ACC";
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public int UserId { get; set; }
    public virtual User User { get; set; } = null!;
}