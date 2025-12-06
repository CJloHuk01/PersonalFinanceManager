using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Enum;
using PersonalFinanceManager.Models;

namespace PersonalFinanceManager.Services;

public class DataService
{
    private readonly ApplicationDbContext _context;

    public DataService()
    {
        _context = new ApplicationDbContext();
    }

    public List<Account> GetAccounts()
    {
        return _context.Accounts.ToList();
    }
    public void AddAccount(Account account)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"DataService.AddAccount called: {account.Name}");
            System.Diagnostics.Debug.WriteLine($"Account details: Type={account.AccountType}, Balance={account.Balance}, CreatedDate={account.CreatedDate}");

            _context.Accounts.Add(account);
            _context.SaveChanges();

            System.Diagnostics.Debug.WriteLine($"Account saved successfully. ID: {account.Id}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DataService.AddAccount ERROR: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException?.Message}");
            throw;
        }
    }
   

    public void DeleteAccount(Account account)
    {
        _context.Accounts.Remove(account);
        _context.SaveChanges();
    }

    public void UpdateAccountBalance(int accountId, decimal amount, TransactionType transactionType)
    {
        var account = _context.Accounts.Find(accountId);
        if (account != null)
        {
            account.Balance += transactionType == TransactionType.Income ? amount : -amount;
            _context.SaveChanges();
        }
    }

    public List<Category> GetCategories()
    {
        return _context.Categories.ToList();
    }

    public List<Category> GetCategoriesByType(TransactionType type)
    {
        return _context.Categories.Where(c => c.CategoryType == type).ToList();
    }

    public void AddCategory(Category category)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"DataService.AddCategory called: {category.Name}");
            System.Diagnostics.Debug.WriteLine($"Category details: Type={category.CategoryType}, Color={category.Color}");

            _context.Categories.Add(category);
            _context.SaveChanges();

            System.Diagnostics.Debug.WriteLine($"Category saved successfully. ID: {category.Id}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DataService.AddCategory ERROR: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Inner: {ex.InnerException?.Message}");
            throw;
        }
    }

    public void DeleteCategory(Category category)
    {
        _context.Categories.Remove(category);
        _context.SaveChanges();
    }

    public List<Transaction> GetTransactions()
    {
        return _context.Transactions
            .Include(t => t.Account)
            .Include(t => t.Category)
            .OrderByDescending(t => t.Date)
            .ToList();
    }

    public void AddTransaction(Transaction transaction)
    {
        _context.Transactions.Add(transaction);

        UpdateAccountBalance(transaction.AccountId, transaction.Amount, transaction.TransactionType);

        _context.SaveChanges();
    }

    public void DeleteTransaction(Transaction transaction)
    {
        var account = _context.Accounts.Find(transaction.AccountId);
        if (account != null)
        {
            account.Balance += transaction.TransactionType == TransactionType.Income
                ? -transaction.Amount
                : transaction.Amount;
        }

        _context.Transactions.Remove(transaction);
        _context.SaveChanges();
    }

    public decimal GetTotalBalance()
    {
        return _context.Accounts.Sum(a => a.Balance);
    }

    public (decimal Income, decimal Expense) GetIncomeExpenseSummary(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Transactions.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(t => t.Date >= startDate.Value);
        if (endDate.HasValue)
            query = query.Where(t => t.Date <= endDate.Value);

        var income = query.Where(t => t.TransactionType == TransactionType.Income).Sum(t => t.Amount);
        var expense = query.Where(t => t.TransactionType == TransactionType.Expense).Sum(t => t.Amount);

        return (income, expense);
    }
}