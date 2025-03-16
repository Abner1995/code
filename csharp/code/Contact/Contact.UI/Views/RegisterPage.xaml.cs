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
        if (BindingContext is Page parentPage) // ��ȡ��ҳ��ʵ��
        {
            var loginPage = new LoginPage();
            loginPage.BindingContext = parentPage;
            await parentPage.ShowPopupAsync(loginPage); // ͨ����ҳ����µ� Popup
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }
}