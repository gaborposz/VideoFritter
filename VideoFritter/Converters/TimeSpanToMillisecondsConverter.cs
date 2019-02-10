using System;
using System.Globalization;
using System.Windows.Data;

namespace VideoFritter.Converters
{
    internal class TimeSpanToMillisecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            TimeSpan timeSpan = (TimeSpan)value;
            return timeSpan.TotalMilliseconds;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            double milliSeconds = (double)value;
            return TimeSpan.FromMilliseconds(milliSeconds);
        }
    }
}
