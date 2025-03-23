using Contact.UI.ViewModels;

namespace Contact.UI.Views;

public partial class ContactPage : ContentPage
{
    public ContactPage(ContactViewModel contactViewModel)
    {
        InitializeComponent();
        this.BindingContext = contactViewModel;
    }
}