using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Enum;
using PersonalFinanceManager.Models;
using System.Collections.ObjectModel;

namespace PersonalFinanceManager.ViewModels;

public partial class TransactionsViewModel : ViewModelBase
{
    private readonly ApplicationDbContext _context;

    [ObservableProperty]
    private ObservableCollection<Transaction> _transactions = new();

    [ObservableProperty]
    private ObservableCollection<Account> _accounts = new();

    [ObservableProperty]
    private ObservableCollection<Category> _categories = new();

    [ObservableProperty]
    private decimal _transactionAmount;

    [ObservableProperty]
    private DateTime _transactionDate = DateTime.Now;

    [ObservableProperty]
    private string _transactionDescription = string.Empty;

    [ObservableProperty]
    private TransactionType _transactionType = TransactionType.Expense;

    [ObservableProperty]
    private Account? _selectedAccount;

    [ObservableProperty]
    private Category? _selectedCategory;

    public TransactionsViewModel()
    {
        Title = "Транзакции";
        _context = new ApplicationDbContext();
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            var transactions = _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .OrderByDescending(t => t.Date)
                .ToList();

            Transactions.Clear();
            foreach (var transaction in transactions)
            {
                Transactions.Add(transaction);
            }

            Accounts.Clear();
            var accounts = _context.Accounts.ToList();
            foreach (var account in accounts)
            {
                Accounts.Add(account);
            }

            Categories.Clear();
            var categories = _context.Categories.ToList();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }

            SelectedAccount = Accounts.FirstOrDefault();
            SelectedCategory = Categories.FirstOrDefault(c => c.CategoryType == TransactionType.Expense);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки данных: {ex.Message}");
        }
    }

    [RelayCommand]
    private void AddTransaction()
    {
        if (SelectedAccount == null || SelectedCategory == null || TransactionAmount <= 0)
            return;

        try
        {
            var transaction = new Transaction
            {
                Amount = TransactionAmount,
                Date = TransactionDate,
                Description = TransactionDescription.Trim(),
                TransactionType = TransactionType,
                AccountId = SelectedAccount.Id,
                CategoryId = SelectedCategory.Id,
                Account = SelectedAccount,
                Category = SelectedCategory
            };

            _context.Transactions.Add(transaction);

            if (TransactionType == TransactionType.Income)
                SelectedAccount.Balance += TransactionAmount;
            else
                SelectedAccount.Balance -= TransactionAmount;

            _context.SaveChanges();

            Transactions.Insert(0, transaction);

            ResetForm();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка добавления транзакции: {ex.Message}");
        }
    }

    [RelayCommand]
    private void DeleteTransaction(Transaction transaction)
    {
        if (transaction == null) return;

        try
        {
            var account = _context.Accounts.Find(transaction.AccountId);
            if (account != null)
            {
                if (transaction.TransactionType == TransactionType.Income)
                    account.Balance -= transaction.Amount;
                else
                    account.Balance += transaction.Amount;
            }

            _context.Transactions.Remove(transaction);
            _context.SaveChanges();

            Transactions.Remove(transaction);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка удаления транзакции: {ex.Message}");
        }
    }

    private void ResetForm()
    {
        TransactionAmount = 0;
        TransactionDescription = string.Empty;
        TransactionDate = DateTime.Now;
        SelectedCategory = Categories.FirstOrDefault(c => c.CategoryType == TransactionType);
    }

    partial void OnTransactionTypeChanged(TransactionType value)
    {
        SelectedCategory = Categories.FirstOrDefault(c => c.CategoryType == value);
    }
}