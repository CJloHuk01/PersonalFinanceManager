using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceManager.Services;
using PersonalFinanceManager.Views;
using System.Windows;

namespace PersonalFinanceManager.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string confirmPassword = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;

    private readonly DataService _dataService = DataService.Instance;

    [RelayCommand]
    private void Register()
    {
        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Пароли не совпадают";
            return;
        }

        try
        {
            var ok = _dataService.Register(Email, Password);

            if (!ok)
            {
                ErrorMessage = "Такой email уже зарегистрирован";
                return;
            }

            var main = new MainWindow();
            Application.Current.Windows[0]?.Close();
            main.Show();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void BackToLogin()
    {
        var login = new LoginWindow();
        Application.Current.Windows[0]?.Close();
        login.Show();
    }
}
