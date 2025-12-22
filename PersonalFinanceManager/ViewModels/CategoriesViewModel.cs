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
        System.Diagnostics.Debug.WriteLine($"=== ADD CATEGORY CLICKED ===");
        System.Diagnostics.Debug.WriteLine($"Name: {NewCategoryName}, Type: {NewCategoryType}");

        if (string.IsNullOrWhiteSpace(NewCategoryName))
        {
            System.Diagnostics.Debug.WriteLine("ОШИБКА: Пустое название категории");
            return;
        }

        try
        {
            var categoryType = NewCategoryType == "Доход" ? TransactionType.Income : TransactionType.Expense;

            System.Diagnostics.Debug.WriteLine($"Converted CategoryType: {categoryType}");

            var category = new Category
            {
                Name = NewCategoryName.Trim(),
                CategoryType = categoryType,
                Color = GetDefaultColor(categoryType)
            };

            System.Diagnostics.Debug.WriteLine($"Создана категория: {category.Name}, {category.CategoryType}, {category.Color}");

            DataService.AddCategory(category);

            Categories.Add(category);

            if (category.CategoryType == TransactionType.Income)
                IncomeCategories.Add(category);
            else
                ExpenseCategories.Add(category);

            System.Diagnostics.Debug.WriteLine($"Категория добавлена. Всего: {Categories.Count}");

            NewCategoryName = string.Empty;
            System.Diagnostics.Debug.WriteLine($"Форма сброшена");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка добавления категории: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
            System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
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
    public void Reload()
    {
        LoadCategories();
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