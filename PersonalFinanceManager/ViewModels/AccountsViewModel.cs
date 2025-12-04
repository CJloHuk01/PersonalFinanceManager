using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Enum;
using PersonalFinanceManager.Models;
using System.Collections.ObjectModel;

namespace PersonalFinanceManager.ViewModels;

public partial class AccountsViewModel : ViewModelBase
{
    private readonly ApplicationDbContext _context;

    [ObservableProperty]
    private ObservableCollection<Account> _accounts = new();

    [ObservableProperty]
    private Account? _selectedAccount;

    [ObservableProperty]
    private string _newAccountName = string.Empty;

    [ObservableProperty]
    private AccountType _newAccountType = AccountType.BankCard;

    [ObservableProperty]
    private decimal _initialBalance;

    public AccountsViewModel()
    {
        Title = "Управление счетами";
        _context = new ApplicationDbContext();
        LoadAccounts();
    }

    private void LoadAccounts()
    {
        try
        {
            Accounts.Clear();
            var accounts = _context.Accounts.ToList();
            foreach (var account in accounts)
            {
                Accounts.Add(account);
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
        if (string.IsNullOrWhiteSpace(NewAccountName))
            return;

        try
        {
            var account = new Account
            {
                Name = NewAccountName.Trim(),
                AccountType = NewAccountType,
                Balance = InitialBalance,
                Color = GetDefaultColor(NewAccountType)
            };

            _context.Accounts.Add(account);
            _context.SaveChanges();

            Accounts.Add(account);
            NewAccountName = string.Empty;
            InitialBalance = 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка добавления счета: {ex.Message}");
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