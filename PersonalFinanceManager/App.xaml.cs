using System.Windows;
using System.Threading.Tasks;
using PersonalFinanceManager.Data;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceManager;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        await InitializeDatabaseAsync();
        var mainWindow = new MainWindow();
        mainWindow.Show();
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