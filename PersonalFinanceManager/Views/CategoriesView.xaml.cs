using PersonalFinanceManager.ViewModels;
using System.Windows.Controls;

namespace PersonalFinanceManager.Views;

public partial class CategoriesView : UserControl
{
    public CategoriesView()
    {
        InitializeComponent();
        Loaded += (_, __) =>
        {
            (DataContext as CategoriesViewModel)?.Reload();
        };
    }
}