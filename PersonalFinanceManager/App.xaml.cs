using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;
using PersonalFinanceManager.Views;
using System.Threading.Tasks;
using System.Windows;

namespace PersonalFinanceManager;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        await InitializeDatabaseAsync();
        var login = new LoginWindow();
        login.Show();
    }

    private async Task InitializeDatabaseAsync()
    {
        try
        {
            using var context = new ApplicationDbContext();
            await context.Database.EnsureCreatedAsync();

            System.Diagnostics.Debug.WriteLine("База данных успешно инициализирована");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка инициализации базы данных: {ex.Message}",
                          "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}