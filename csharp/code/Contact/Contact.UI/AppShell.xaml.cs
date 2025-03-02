using Contact.UI.Views;

namespace Contact.UI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(MePage), typeof(MePage));
            Routing.RegisterRoute(nameof(ContactPage), typeof(ContactPage));
        }
    }
}
