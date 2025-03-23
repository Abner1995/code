using CommunityToolkit.Maui.Views;

namespace Contact.UI.Views;

public partial class MePage : ContentPage
{
    public MePage()
    {
        InitializeComponent();
    }

    private async void Login_Register_Clicked(object sender, EventArgs e)
    {
        var loginPage = new LoginPage();
        loginPage.BindingContext = this;
        await this.ShowPopupAsync(loginPage);
    }

    private void Logout_Clicked(object sender, EventArgs e)
    {

    }
}