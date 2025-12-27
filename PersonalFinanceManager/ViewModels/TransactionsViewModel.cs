using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceManager.Enum;
using PersonalFinanceManager.Models;
using PersonalFinanceManager.Services;
using PersonalFinanceManager.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

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
    private TransactionType _transactionType = TransactionType.Расход;

    [ObservableProperty]
    private Account? _selectedAccount;

    [ObservableProperty]
    private Category? _selectedCategory;
    [ObservableProperty]
    private TransactionType? _filterTransactionType = null;
    public ICollectionView TransactionsView { get; }
    private bool _dateSortDescending = true;
    private bool _amountSortDescending = true;
    [ObservableProperty]
    private string _filterButtonText = "Все операции";

    private int _filterState = 0;
    public TransactionsViewModel()
    {
        Title = "Транзакции";
        TransactionsView = CollectionViewSource.GetDefaultView(Transactions);
        TransactionsView.Filter = TransactionFilter;
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
                Date = TransactionDate.ToUniversalTime(),
                Description = TransactionDescription.Trim(),
                TransactionType = TransactionType,
                AccountId = SelectedAccount.Id,
                CategoryId = SelectedCategory.Id
            };

            DataService.AddTransaction(transaction);

            LoadData();

            ResetForm();

            System.Diagnostics.Debug.WriteLine($"Транзакция успешно добавлена");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Ошибка добавления транзакции:");
            System.Diagnostics.Debug.WriteLine(ex.InnerException?.Message ?? ex.Message);
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
    public void Reload()
    {
        LoadData();
    }
    partial void OnTransactionTypeChanged(TransactionType value)
    {
        FilteredCategories.Clear();

        foreach (var c in Categories.Where(c => c.CategoryType == value))
            FilteredCategories.Add(c);

        SelectedCategory = FilteredCategories.FirstOrDefault();
    }
    private bool TransactionFilter(object obj)
    {
        if (obj is not Transaction t)
            return false;

        if (FilterTransactionType != null && t.TransactionType != FilterTransactionType)
            return false;

        return true;
    }

    [RelayCommand]
    private void ToggleTransactionFilter()
    {
        _filterState = (_filterState + 1) % 3;

        FilterTransactionType = _filterState switch
        {
            1 => TransactionType.Доход,
            2 => TransactionType.Расход,
            _ => null
        };

        FilterButtonText = _filterState switch
        {
            1 => "Доходы",
            2 => "Расходы",
            _ => "Все операции"
        };

        TransactionsView.Refresh();
    }

    [RelayCommand]
    private void ToggleSortByDate()
    {
        _dateSortDescending = !_dateSortDescending;

        TransactionsView.SortDescriptions.Clear();

        TransactionsView.SortDescriptions.Add(
            new SortDescription(
                nameof(Transaction.Date),
                _dateSortDescending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending));

        TransactionsView.Refresh();
    }
    [RelayCommand]
    private void ToggleSortByAmount()
    {
        _amountSortDescending = !_amountSortDescending;

        TransactionsView.SortDescriptions.Clear();

        TransactionsView.SortDescriptions.Add(
            new SortDescription(
                nameof(Transaction.Amount),
                _amountSortDescending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending));

        TransactionsView.Refresh();
    }
}