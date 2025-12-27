using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceManager.Services;
using System.Windows;
using PersonalFinanceManager.Views;

namespace PersonalFinanceManager.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;

    private readonly DataService _dataService = DataService.Instance;

    [RelayCommand]
    private void Login()
    {
        if (!_dataService.Login(Email, Password))
        {
            ErrorMessage = "Неверный email или пароль";
            return;
        }

        var main = new MainWindow();
        Application.Current.Windows[0]?.Close();
        main.Show();
    }

    [RelayCommand]
    private void OpenRegister()
    {
        var reg = new RegisterWindow();
        Application.Current.Windows[0]?.Close();
        reg.Show();
    }
}
