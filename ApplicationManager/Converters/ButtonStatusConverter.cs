using System;
using System.Globalization;
using System.Windows.Data;

namespace ApplicationManager.Converters
{
    public class ButtonStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return true;
            }

            var isDisconnected = (bool)value;
            return !isDisconnected;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}