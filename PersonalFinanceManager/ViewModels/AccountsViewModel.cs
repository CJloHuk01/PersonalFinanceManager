using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Enum;
using PersonalFinanceManager.Models;
using PersonalFinanceManager.Services;
using System.Collections.ObjectModel;

namespace PersonalFinanceManager.ViewModels;

public partial class AccountsViewModel : ViewModelBase
{
    private readonly ApplicationDbContext _context;
    private readonly DataService _dataService;

    [ObservableProperty]
    private ObservableCollection<Account> _accounts = new();

    [ObservableProperty]
    private Account? _selectedAccount;


    [ObservableProperty]
    private string _newAccountName = string.Empty;

    [ObservableProperty]
    private string _newAccountType = "Наличные";

    public ObservableCollection<string> AccountTypes { get; } = new()
    {
        "Наличные",
        "Банковская карта",
        "Кредитная карта",
        "Сбережения",
        "Инвестиции"
    };
    [ObservableProperty]
    private decimal _initialBalance;

    public AccountsViewModel()
    {
        Title = "Управление счетами";
        _dataService = new DataService();
        LoadAccounts();
    }

    private void LoadAccounts()
    {
        try
        {
            Accounts.Clear();
            var accounts = _dataService.GetAccounts();

            System.Diagnostics.Debug.WriteLine($"=== Загружено счетов: {accounts.Count} ===");
            foreach (var account in accounts)
            {
                Accounts.Add(account);
                System.Diagnostics.Debug.WriteLine($"- {account.Id}: {account.Name} ({account.Balance})");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки счетов: {ex.Message}");
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
            string accountTypeString = NewAccountType; 
            System.Diagnostics.Debug.WriteLine($"AccountType string: '{accountTypeString}'");
            AccountType accountType = accountTypeString switch
            {
                "Наличные" => AccountType.Cash,
                "Банковская карта" => AccountType.BankCard,
                "Кредитная карта" => AccountType.CreditCard,
                "Сбережения" => AccountType.Savings,
                "Инвестиции" => AccountType.Investment,
                _ => AccountType.Cash
            };
            System.Diagnostics.Debug.WriteLine($"Converted AccountType: {accountType}");
            var account = new Account
            {
                Name = NewAccountName.Trim(),
                AccountType = accountType,
                Balance = InitialBalance,
                Color = GetDefaultColor(accountType),
                CreatedDate = DateTime.UtcNow
            };
            System.Diagnostics.Debug.WriteLine($"Создан аккаунт: {account.Name}, {account.AccountType}, {account.Balance}, {account.CreatedDate}");
            Accounts.Add(account);
            System.Diagnostics.Debug.WriteLine($"Аккаунт добавлен в коллекцию. Всего счетов: {Accounts.Count}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка добавления счета: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
        }
    }


    [RelayCommand]
    private void DeleteAccount()
    {
        if (SelectedAccount == null) return;
        try
        {
            _context.Accounts.Remove(SelectedAccount);
            _context.SaveChanges();
            Accounts.Remove(SelectedAccount);
            SelectedAccount = null;
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