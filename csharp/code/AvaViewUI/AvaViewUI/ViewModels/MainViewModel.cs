using AvaViewUI.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;

namespace AvaViewUI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private GalleryItem _selectedNavItem;

    [ObservableProperty]
    private object _currentDemoVM;

    [ObservableProperty]
    private ObservableCollection<GalleryItem> _navItems = new ObservableCollection<GalleryItem> { };

    public MainViewModel()
    {
        this.GetNavItems();
    }

    partial void OnSelectedNavItemChanged(GalleryItem value)
    {
        if (value == null)
            return;

        var type = Type.GetType(value.ViewModelType);

        if (type == null)
            return;

        CurrentDemoVM = Activator.CreateInstance(type);
    }

    private void GetNavItems()
    {
        NavItems.Add(new GalleryItem
        {
            Name = "Button",
            ViewModelType = "AvaViewUI.ViewModels.DemoPages.ButtonDemoViewModel"
        });
        NavItems.Add(new GalleryItem
        {
            Name = "TextBox",
            ViewModelType = "AvaViewUI.ViewModels.DemoPages.TextBoxDemoViewModel"
        });
    }
}
