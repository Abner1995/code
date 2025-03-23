using CommunityToolkit.Mvvm.ComponentModel;
using Contact.UI.Models;
using System.Collections.ObjectModel;

namespace Contact.UI.ViewModels;

public partial class ContactViewModel : BaseViewModel
{
    [ObservableProperty]
    public ObservableCollection<ContactModel> contacts = new ObservableCollection<ContactModel>();

    public ContactViewModel()
    {
        LoadContact();
    }

    public void LoadContact()
    {
        Contacts = new ObservableCollection<ContactModel>
        {
            new ContactModel{Id = 1, UserName = "小许", UserId = 1 },
            new ContactModel{Id = 2, UserName = "小杜", UserId = 2 },
            new ContactModel{Id = 3, UserName = "小潘", UserId = 3 },
            new ContactModel{Id = 4, UserName = "小田", UserId = 4 },
            new ContactModel{Id = 5, UserName = "小荀", UserId = 5 },
            new ContactModel{Id = 6, UserName = "小王", UserId = 6 },
            new ContactModel{Id = 7, UserName = "小郑", UserId = 7 },
            new ContactModel{Id = 8, UserName = "小蕊", UserId = 8 },
            new ContactModel{Id = 9, UserName = "小凌", UserId = 9 },
            new ContactModel{Id = 10, UserName = "小新", UserId = 10 }
        };
    }
}