using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VideoFritter.Converters
{
    internal class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            bool boolValue = (bool)value;
            return boolValue ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Visibility visibilityValue = (Visibility)value;
            return visibilityValue != Visibility.Visible;
        }
    }
}
