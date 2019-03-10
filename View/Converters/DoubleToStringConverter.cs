using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace View.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class DoubleToStringConverter : IValueConverter
    {
        public static DoubleToStringConverter Instance = new DoubleToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return (string) value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value;

        }
    }
}