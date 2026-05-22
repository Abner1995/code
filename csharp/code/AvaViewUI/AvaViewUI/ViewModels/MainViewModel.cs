using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaViewUI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}
