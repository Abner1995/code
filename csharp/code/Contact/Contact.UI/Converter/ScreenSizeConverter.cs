using System.Globalization;

namespace Contact.UI.Converter;

public class ScreenSizeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double width)
        {
            if (parameter is string type)
            {
                if (type == "Width")
                    return width;
                if (type == "Height")
                    return width * 1.5; // 根据需要调整高度
            }
        }
        return 300;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
