using CommunityToolkit.Mvvm.ComponentModel;
using Contact.UI.Models;
using System.Collections.ObjectModel;

namespace Contact.UI.ViewModels;

public partial class ContactDetailModel : BaseViewModel
{
    [ObservableProperty]
    public ObservableCollection<PhoneModel> phones = new ObservableCollection<PhoneModel>();

    public ContactDetailModel()
    {
        LoadContact();
    }

    public void LoadContact()
    {
        Phones = new ObservableCollection<PhoneModel>
        {
            new PhoneModel{Id = 1, ContactId = 1, UserId = 1, Mobile = "18861734844" },
            new PhoneModel{Id = 2, ContactId = 1, UserId = 1, Mobile = "18861734814" },
        };
    }
}
