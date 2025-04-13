using Contact.UI.ViewModels;

namespace Contact.UI.Views;

public partial class ContactDetailPage : ContentPage
{
    public ContactDetailPage(ContactDetailModel contactDetailModel)
    {
        InitializeComponent();
        this.BindingContext = contactDetailModel;
    }
}