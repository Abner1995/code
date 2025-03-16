using CommunityToolkit.Maui.Views;
using Contact.UI.Helper;

namespace Contact.UI.Views
{
    public partial class LoginPage : Popup
    {
        public LoginPage()
        {
            InitializeComponent();
            Size = PopupHelper.GetScreenSize();
        }

        private async void Register_Tapped(object sender, TappedEventArgs e)
        {
            await CloseAsync();
            if (BindingContext is Page parentPage) // ��ȡ��ҳ��ʵ��
            {
                var registerPage = new RegisterPage();
                registerPage.BindingContext = parentPage;
                await parentPage.ShowPopupAsync(registerPage); // ͨ����ҳ����µ� Popup
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await CloseAsync();
        }
    }
}