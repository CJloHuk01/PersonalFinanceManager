using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceManager.Enum;
using PersonalFinanceManager.Models;
using PersonalFinanceManager.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;


namespace PersonalFinanceManager.ViewModels;

public partial class AccountsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<Account> _accounts = new();

    [ObservableProperty]
    private Account? _selectedAccount;

    [ObservableProperty]
    private string _newAccountName = string.Empty;

    [ObservableProperty]
    private decimal _initialBalance;

    public ICollectionView AccountsView { get; }

    [ObservableProperty]
    private AccountType _selectedAccountType = AccountType.Наличные;
    private bool _sortDescending;

    public IEnumerable<AccountType> AccountTypes =>
        System.Enum.GetValues(typeof(AccountType)).Cast<AccountType>();

    public AccountsViewModel()
    {
        Title = "Управление счетами";
        AccountsView = CollectionViewSource.GetDefaultView(Accounts);
        LoadAccounts();
    }

    public void LoadAccounts()
    {
        try
        {
            Accounts.Clear();
            var accounts = DataService.GetAccounts();

            System.Diagnostics.Debug.WriteLine($"=== Загружено счетов: {accounts.Count} ===");

            if (accounts.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("Нет счетов в БД или ошибка загрузки");
            }

            foreach (var account in accounts)
            {
                System.Diagnostics.Debug.WriteLine($"- {account.Id}: {account.Name} ({account.Balance}), Type: {account.AccountType}");
                Accounts.Add(account);
            }

            System.Diagnostics.Debug.WriteLine($"Accounts.Count в коллекции: {Accounts.Count}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки счетов: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
        }
    }

    [RelayCommand]
    private void AddAccount()
    {
        if (string.IsNullOrWhiteSpace(NewAccountName))
            return;

        var account = new Account
        {
            Name = NewAccountName.Trim(),
            AccountType = SelectedAccountType,
            Balance = InitialBalance,
            Color = GetDefaultColor(SelectedAccountType),
            CreatedDate = DateTime.UtcNow
        };

        DataService.AddAccount(account);

        Accounts.Add(account);

        NewAccountName = string.Empty;
        InitialBalance = 0;
        SelectedAccountType = AccountType.Наличные;
    }

    [RelayCommand]
    private void DeleteAccount(Account account)
    {
        if (account == null) return;
        try
        {
            DataService.DeleteAccount(account);
            Accounts.Remove(account);
            System.Diagnostics.Debug.WriteLine($"Счет удален");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка удаления счета: {ex.Message}");
        }
    }
    public void Reload()
    {
        LoadAccounts();
    }
    private string GetDefaultColor(AccountType accountType)
    {
        return accountType switch
        {
            AccountType.Наличные => "#4CAF50", 
            AccountType.Банковская_карта => "#2196F3",
            AccountType.Кредитная_карта => "#F44336", 
            AccountType.Сбережения => "#FF9800",
            AccountType.Инвестиции => "#9C27B0", 
            AccountType.Депозит => "#3F51B5", 
            AccountType.Электронный_кошелек => "#009688", 
            AccountType.Кредит => "#795548", 
            _ => "#607D8B"  
        };
    }
    [RelayCommand]
    private void ToggleSortByBalance()
    {
        _sortDescending = !_sortDescending;

        AccountsView.SortDescriptions.Clear();
        AccountsView.SortDescriptions.Add(
            new SortDescription(nameof(Account.Balance),
            _sortDescending ? ListSortDirection.Descending : ListSortDirection.Ascending));

        AccountsView.Refresh();

    }
}