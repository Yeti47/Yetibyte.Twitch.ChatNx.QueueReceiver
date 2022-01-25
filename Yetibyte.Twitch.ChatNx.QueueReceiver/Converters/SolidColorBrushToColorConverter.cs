using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Converters
{
    [ValueConversion(typeof(SolidColorBrush), typeof(Color))]
    public class SolidColorBrushToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = value as SolidColorBrush;
            return brush?.Color ?? Color.FromArgb(255, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value is not Color color)
            {
                color = default;
            }
            
            return new SolidColorBrush(color);
        }
    }
}
