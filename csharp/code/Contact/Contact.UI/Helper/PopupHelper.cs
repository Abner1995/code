namespace Contact.UI.Helper;

public static class PopupHelper
{
    /// <summary>
    /// 获取屏幕宽度（逻辑像素）
    /// </summary>
    public static double GetScreenWidth()
    {
        var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
        return mainDisplayInfo.Width / mainDisplayInfo.Density;
    }

    /// <summary>
    /// 获取屏幕高度（逻辑像素）
    /// </summary>
    public static double GetScreenHeight()
    {
        var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
        return mainDisplayInfo.Height / mainDisplayInfo.Density;
    }

    /// <summary>
    /// 获取屏幕大小（逻辑像素）
    /// </summary>
    public static Size GetScreenSize()
    {
        return new Size(GetScreenWidth(), GetScreenHeight());
    }
}
