﻿namespace NeutronUI;

public static class NeutronServiceProvider
{
    public static TService GetService<TService>()
    => Current.GetService<TService>();

    public static IServiceProvider Current
        =>
#if WINDOWS10_0_17763_0_OR_GREATER
            MauiWinUIApplication.Current.Services;
#elif ANDROID
            MauiApplication.Current.Services;
#elif IOS || MACCATALYST
            MauiUIApplicationDelegate.Current.Services;
#else
            Application.Current.Handler.MauiContext.Services;
#endif
}
