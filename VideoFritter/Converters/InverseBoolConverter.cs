using System;
using System.Globalization;
using System.Windows.Data;

namespace VideoFritter.Converters
{
    internal class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            bool boolValue = (bool)value;
            return !boolValue;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            bool boolValue = (bool)value;
            return !boolValue;
        }
    }
}
