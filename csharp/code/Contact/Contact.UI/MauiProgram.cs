using Contact.UI.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Contact.UI.ViewModels;

namespace Contact.UI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<RegisterPage>();
        builder.Services.AddSingleton<MePage>();
        builder.Services.AddSingleton<ContactPage>();
        builder.Services.AddSingleton<AddContactPage>();
        builder.Services.AddSingleton<ContactDetailPage>();

        builder.Services.AddSingleton<ContactViewModel>();
        builder.Services.AddSingleton<ContactDetailModel>();

        return builder.Build();
    }
}
