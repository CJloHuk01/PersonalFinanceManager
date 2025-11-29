using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace PersonalFinanceManager.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ViewModelBase _currentViewModel = null!;

        private readonly AccountsViewModel _accountsViewModel;
        private readonly TransactionsViewModel _transactionsViewModel;
        private readonly CategoriesViewModel _categoriesViewModel;

        public MainViewModel()
        {
            _accountsViewModel = new AccountsViewModel();
            _transactionsViewModel = new TransactionsViewModel();
            _categoriesViewModel = new CategoriesViewModel();

            CurrentViewModel = _accountsViewModel;
            Title = "Управление счетами";
        }

        [RelayCommand]
        private void Navigate(string page)
        {
            CurrentViewModel = page switch
            {
                "Accounts" => _accountsViewModel,
                "Transactions" => _transactionsViewModel,
                "Categories" => _categoriesViewModel,
                _ => _accountsViewModel
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
}
