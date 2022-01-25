using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Converters
{
    [ValueConversion(typeof(string), typeof(Brush))]
    public class HexStringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Brush))
                throw new ArgumentException("Target type must be Brush.");

            string valueStr = value?.ToString() ?? "#000000";

            Color color = (Color)ColorConverter.ConvertFromString(valueStr);

            return new SolidColorBrush(color);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
