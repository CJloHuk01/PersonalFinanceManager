using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceManager.Enum;
using PersonalFinanceManager.Models;
using PersonalFinanceManager.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace PersonalFinanceManager.ViewModels;

public partial class CategoriesViewModel : ViewModelBase
{
    [ObservableProperty]
    private ObservableCollection<Category> _categories = new();

    [ObservableProperty]
    private ObservableCollection<Category> _incomeCategories = new();

    [ObservableProperty]
    private ObservableCollection<Category> _expenseCategories = new();

    [ObservableProperty]
    private string _newCategoryName = string.Empty;
    [ObservableProperty]
    private string _filterButtonText = "Все категории";

    private int _filterState = 0;

    [ObservableProperty]
    private TransactionType? _selectedFilterType = null;
    public ICollectionView CategoriesView { get; }

    public ObservableCollection<TransactionType> TransactionTypes { get; } =
    new ObservableCollection<TransactionType>
    {
        TransactionType.Доход,
        TransactionType.Расход
    };
    [ObservableProperty]
    private TransactionType _selectedTransactionType = TransactionType.Расход;

    public CategoriesViewModel()
    {
        Title = "Категории";
        CategoriesView = CollectionViewSource.GetDefaultView(Categories);
        CategoriesView.Filter = CategoryFilter;
        LoadCategories();
    }
    private bool CategoryFilter(object obj)
    {
        if (obj is not Category c)
            return false;

        if (SelectedFilterType == null)
            return true;

        return c.CategoryType == SelectedFilterType;
    }

    private void LoadCategories()
    {
        Categories.Clear();
        IncomeCategories.Clear();
        ExpenseCategories.Clear();

        var categories = DataService.GetCategories();

        foreach (var category in categories)
        {
            Categories.Add(category);

            if (category.CategoryType == TransactionType.Доход)
                IncomeCategories.Add(category);
            else
                ExpenseCategories.Add(category);
        }
    }


    [RelayCommand]
    private void AddCategory()
    {
        if (string.IsNullOrWhiteSpace(NewCategoryName))
            return;

        var category = new Category
        {
            Name = NewCategoryName.Trim(),
            CategoryType = SelectedTransactionType,
            Color = GetDefaultColor(SelectedTransactionType)
        };

        DataService.AddCategory(category);
        Reload();

        NewCategoryName = string.Empty;
    }



    [RelayCommand]
    private void DeleteCategory(Category category)
    {
        if (category == null) return;

        DataService.DeleteCategory(category);
        Reload();
    }

    public void Reload()
    {
        LoadCategories();
    }
    private string GetDefaultColor(TransactionType categoryType)
    {
        return categoryType switch
        {
            TransactionType.Доход => "#4CAF50",
            TransactionType.Расход => "#F44336",
            _ => "#607D8B"
        };
    }



    [RelayCommand]
    private void ToggleCategoryFilter()
    {
        _filterState = (_filterState + 1) % 3;

        SelectedFilterType = _filterState switch
        {
            1 => TransactionType.Доход,
            2 => TransactionType.Расход,
            _ => null
        };

        FilterButtonText = _filterState switch
        {
            1 => "Доходы",
            2 => "Расходы",
            _ => "Все категории"
        };

        CategoriesView?.Refresh();
    }

}