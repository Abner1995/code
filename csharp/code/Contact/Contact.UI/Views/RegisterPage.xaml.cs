using CommunityToolkit.Maui.Views;
using Contact.UI.Helper;

namespace Contact.UI.Views;

public partial class RegisterPage : Popup
{
    public RegisterPage()
    {
        InitializeComponent();
        Size = PopupHelper.GetScreenSize();
    }

    private async void Login_Tapped(object sender, TappedEventArgs e)
    {
        await CloseAsync();
        if (BindingContext is Page parentPage) // 获取父页面实例
        {
            var loginPage = new LoginPage();
            loginPage.BindingContext = parentPage;
            await parentPage.ShowPopupAsync(loginPage); // 通过父页面打开新的 Popup
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }
}