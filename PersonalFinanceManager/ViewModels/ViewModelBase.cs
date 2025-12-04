using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceManager.Services;

namespace PersonalFinanceManager.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    protected readonly DataService DataService;

    public ViewModelBase()
    {
        DataService = new DataService();
    }

    [RelayCommand]
    protected virtual void Loaded()
    {
    }
}