using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceManager.Enum;
using PersonalFinanceManager.Models;
using PersonalFinanceManager.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace PersonalFinanceManager.ViewModels;

public partial class TransactionsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<Transaction> _transactions = new();

    [ObservableProperty]
    private ObservableCollection<Account> _accounts = new();

    [ObservableProperty]
    private ObservableCollection<Category> _categories = new();

    [ObservableProperty]
    private ObservableCollection<Category> _filteredCategories = new();

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
        LoadData();
    }

    private void LoadData()
    {
        try
        {
            Transactions.Clear();
            var transactions = DataService.GetTransactions();
            foreach (var transaction in transactions)
            {
                Transactions.Add(transaction);
            }

            Accounts.Clear();
            var accounts = DataService.GetAccounts();
            foreach (var account in accounts)
            {
                Accounts.Add(account);
            }

            Categories.Clear();
            FilteredCategories.Clear();
            var categories = DataService.GetCategories();
            foreach (var category in categories)
            {
                Categories.Add(category);
                if (category.CategoryType == TransactionType)
                {
                    FilteredCategories.Add(category);
                }
            }

            SelectedAccount = Accounts.FirstOrDefault();
            SelectedCategory = FilteredCategories.FirstOrDefault();
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
        {
            System.Diagnostics.Debug.WriteLine("Ошибка: не выбраны счет или категория, или сумма неверна");
            return;
        }

        try
        {
            var transaction = new Transaction
            {
                Amount = TransactionAmount,
                Date = TransactionDate,
                Description = TransactionDescription.Trim(),
                TransactionType = TransactionType,
                AccountId = SelectedAccount.Id,
                CategoryId = SelectedCategory.Id
            };

            DataService.AddTransaction(transaction);

            //if (TransactionType == TransactionType.Income)
            //    SelectedAccount.Balance += TransactionAmount;
            //else
            //    SelectedAccount.Balance -= TransactionAmount;

            LoadData();

            ResetForm();

            System.Diagnostics.Debug.WriteLine($"Транзакция успешно добавлена");
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
            DataService.DeleteTransaction(transaction);
            Transactions.Remove(transaction);

            System.Diagnostics.Debug.WriteLine($"Транзакция удалена");
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
    }

    partial void OnTransactionTypeChanged(TransactionType value)
    {
        FilteredCategories.Clear();
        var filtered = Categories.Where(c => c.CategoryType == value).ToList();
        foreach (var category in filtered)
        {
            FilteredCategories.Add(category);
        }
        SelectedCategory = FilteredCategories.FirstOrDefault();
    }
}