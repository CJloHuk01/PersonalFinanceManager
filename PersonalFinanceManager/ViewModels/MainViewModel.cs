using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceManager.Views;

namespace PersonalFinanceManager.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private object _currentView = null!;

    private readonly AccountsViewModel _accountsViewModel;
    private readonly TransactionsViewModel _transactionsViewModel;
    private readonly CategoriesViewModel _categoriesViewModel;

    private readonly AccountsView _accountsView;
    private readonly TransactionsView _transactionsView;
    private readonly CategoriesView _categoriesView;

    public MainViewModel()
    {
        _accountsViewModel = new AccountsViewModel();
        _transactionsViewModel = new TransactionsViewModel();
        _categoriesViewModel = new CategoriesViewModel();

        _accountsView = new AccountsView
        {
            DataContext = _accountsViewModel
        };

        _transactionsView = new TransactionsView
        {
            DataContext = _transactionsViewModel
        };

        _categoriesView = new CategoriesView
        {
            DataContext = _categoriesViewModel
        };

        CurrentView = _accountsView;
        Title = "Управление счетами";
    }

    [RelayCommand]
    private void Navigate(string page)
    {
        CurrentView = page switch
        {
            "Accounts" => _accountsView,
            "Transactions" => _transactionsView,
            "Categories" => _categoriesView,
            _ => _accountsView
        };

        Title = page switch
        {
            "Accounts" => "Управление счетами",
            "Transactions" => "Транзакции",
            "Categories" => "Категории",
            _ => "Менеджер персональных финансов"
        };
    }
}
