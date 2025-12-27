using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Converters;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Enum;
using PersonalFinanceManager.Models;
using System.Text.RegularExpressions;

namespace PersonalFinanceManager.Services;

public sealed class DataService
{
    private static DataService? _instance;
    private static readonly object _lock = new();
    public User? CurrentUser { get; private set; }

    public static DataService Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new DataService();
            }
        }
    }

    private DataService() { }
    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(
            email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.IgnoreCase);
    }

    public bool Register(string email, string password)
    {
        using var context = new ApplicationDbContext();

        email = email.Trim().ToLowerInvariant();

        if (!IsValidEmail(email))
            throw new ArgumentException("Некорректный email");

        if (context.Users.Any(u => u.Email == email))
            return false;

        var user = new User
        {
            Email = email,
            PasswordHash = PasswordHasher.Hash(password)
        };

        context.Users.Add(user);
        context.SaveChanges();

        CurrentUser = user;
        return true;
    }

    public bool Login(string email, string password)
    {
        using var context = new ApplicationDbContext();

        email = email.Trim().ToLowerInvariant();

        var hash = PasswordHasher.Hash(password);

        var user = context.Users
            .FirstOrDefault(u => u.Email == email && u.PasswordHash == hash);

        if (user == null)
            return false;

        CurrentUser = user;
        return true;
    }
    public void Logout()
    {
        CurrentUser = null;
    }


    public List<Account> GetAccounts()
    {
        using var context = new ApplicationDbContext();
        return context.Accounts
            .Where(a => a.UserId == CurrentUser!.Id)
            .AsNoTracking()
            .ToList();
    }

    public void AddAccount(Account account)
    {
        using var context = new ApplicationDbContext();
        account.UserId = CurrentUser!.Id;
        context.Accounts.Add(account);
        context.SaveChanges();
    }

    public void DeleteAccount(Account account)
    {
        using var context = new ApplicationDbContext();
        context.Accounts.Remove(account);
        context.SaveChanges();
    }

    public List<Category> GetCategories()
    {
        using var context = new ApplicationDbContext();
        return context.Categories
        .Where(c => c.UserId == CurrentUser!.Id)
        .AsNoTracking()
        .ToList();
    }

    public void AddCategory(Category category)
    {
        using var context = new ApplicationDbContext();

        category.UserId = CurrentUser!.Id;

        context.Categories.Add(category);
        context.SaveChanges();
    }

    public void DeleteCategory(Category category)
    {
        using var context = new ApplicationDbContext();

        var existing = context.Categories
            .First(c => c.Id == category.Id && c.UserId == CurrentUser!.Id);

        context.Categories.Remove(existing);
        context.SaveChanges();
    }

    public List<Transaction> GetTransactions()
    {
        using var context = new ApplicationDbContext();

        return context.Transactions
            .Where(t => t.UserId == CurrentUser!.Id)
            .Include(t => t.Account)
            .Include(t => t.Category)
            .AsNoTracking()
            .OrderByDescending(t => t.Date)
            .ToList();
    }

    public void AddTransaction(Transaction transaction)
    {
        using var context = new ApplicationDbContext();

        transaction.UserId = CurrentUser!.Id;

        var account = context.Accounts
            .First(a => a.Id == transaction.AccountId && a.UserId == CurrentUser!.Id);

        if (transaction.TransactionType == TransactionType.Расход &&
            account.Balance < transaction.Amount)
            throw new InvalidOperationException("Недостаточно средств");

        account.Balance += transaction.TransactionType == TransactionType.Доход
            ? transaction.Amount
            : -transaction.Amount;

        context.Transactions.Add(transaction);
        context.SaveChanges();
    }

    public void DeleteTransaction(Transaction transaction)
    {
        using var context = new ApplicationDbContext();

        var existing = context.Transactions
            .Include(t => t.Account)
            .First(t => t.Id == transaction.Id && t.UserId == CurrentUser!.Id);

        existing.Account.Balance += existing.TransactionType == TransactionType.Расход
            ? existing.Amount
            : -existing.Amount;

        context.Transactions.Remove(existing);
        context.SaveChanges();
    }
}
