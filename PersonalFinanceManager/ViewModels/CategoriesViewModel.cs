using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceManager.Enum;
using PersonalFinanceManager.Models;
using System.Collections.ObjectModel;

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
    private string _newCategoryType = "Expense";

    public CategoriesViewModel()
    {
        Title = "Категории";
        LoadCategories();
    }

    private void LoadCategories()
    {
        try
        {
            Categories.Clear();
            IncomeCategories.Clear();
            ExpenseCategories.Clear();

            var categories = DataService.GetCategories();

            foreach (var category in categories)
            {
                Categories.Add(category);

                if (category.CategoryType == TransactionType.Income)
                    IncomeCategories.Add(category);
                else
                    ExpenseCategories.Add(category);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка загрузки категорий: {ex.Message}");
        }
    }

    [RelayCommand]
    private void AddCategory()
    {
        if (string.IsNullOrWhiteSpace(NewCategoryName))
            return;

        try
        {
            var categoryType = NewCategoryType == "Income" ? TransactionType.Income : TransactionType.Expense;

            var category = new Category
            {
                Name = NewCategoryName.Trim(),
                CategoryType = categoryType,
                Color = GetDefaultColor(categoryType)
            };

            DataService.AddCategory(category);
            Categories.Add(category);

            if (category.CategoryType == TransactionType.Income)
                IncomeCategories.Add(category);
            else
                ExpenseCategories.Add(category);

            NewCategoryName = string.Empty;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка добавления категории: {ex.Message}");
        }
    }

    [RelayCommand]
    private void DeleteCategory(Category category)
    {
        if (category == null) return;

        try
        {
            DataService.DeleteCategory(category);

            Categories.Remove(category);

            if (category.CategoryType == TransactionType.Income)
                IncomeCategories.Remove(category);
            else
                ExpenseCategories.Remove(category);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка удаления категории: {ex.Message}");
        }
    }

    private string GetDefaultColor(TransactionType categoryType)
    {
        return categoryType switch
        {
            TransactionType.Income => "#4CAF50",
            TransactionType.Expense => "#F44336",
            _ => "#607D8B"
        };
    }
}