using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceManager.Enum;
using PersonalFinanceManager.Models;
using PersonalFinanceManager.Services;
using System.Collections.ObjectModel;

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
    private string _newAccountType = "Cash";

    [ObservableProperty]
    private decimal _initialBalance;

    public ObservableCollection<string> AccountTypes { get; } = new()
    {
        "Cash",
        "BankCard",
        "CreditCard",
        "Savings",
        "Investment"
    };

    public AccountsViewModel()
    {
        Title = "Управление счетами";
        LoadAccounts();
    }

    private void LoadAccounts()
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
        System.Diagnostics.Debug.WriteLine($"Name: {NewAccountName}, Type: {NewAccountType}, Balance: {InitialBalance}");

        if (string.IsNullOrWhiteSpace(NewAccountName))
        {
            System.Diagnostics.Debug.WriteLine("ОШИБКА: Пустое название счета");
            return;
        }

        try
        {
            AccountType accountType = NewAccountType switch
            {
                "Cash" => AccountType.Cash,
                "BankCard" => AccountType.BankCard,
                "CreditCard" => AccountType.CreditCard,
                "Savings" => AccountType.Savings,
                "Investment" => AccountType.Investment,
                _ => AccountType.Cash
            };

            var account = new Account
            {
                Name = NewAccountName.Trim(),
                AccountType = accountType,
                Balance = InitialBalance,
                Color = GetDefaultColor(accountType),
                CreatedDate = DateTime.UtcNow
            };

            DataService.AddAccount(account);

            // Перезагружаем список
            LoadAccounts();

            // Сбрасываем форму
            NewAccountName = string.Empty;
            InitialBalance = 0;
            NewAccountType = "Cash";

            System.Diagnostics.Debug.WriteLine($"Счет успешно добавлен");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка добавления счета: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
        }
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

    private string GetDefaultColor(AccountType accountType)
    {
        return accountType switch
        {
            AccountType.Cash => "#4CAF50",
            AccountType.BankCard => "#2196F3",
            AccountType.CreditCard => "#F44336",
            AccountType.Savings => "#FF9800",
            AccountType.Investment => "#9C27B0",
            _ => "#607D8B"
        };
    }
}