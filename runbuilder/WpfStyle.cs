using System;
using System.Windows.Data;
using System.Windows.Media;

namespace DataGrid.Style
{
    public class BgColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = (bool)value;
            if (result == false)
            {
                return new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                return new SolidColorBrush(Colors.Pink);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}