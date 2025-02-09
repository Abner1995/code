using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiUI.ViewModels;

public partial class LoginViewModel: BaseViewModel
{
    [ObservableProperty]
    private string _userName;

    [ObservableProperty]
    private string _passWord;
}
