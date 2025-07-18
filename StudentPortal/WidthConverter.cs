using System;
using System.Globalization;
using System.Windows.Data;

namespace StudentPortal
{
    public class WidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double listViewWidth)
            {
                double cardWidth = (listViewWidth - 20) / 2;
                return cardWidth > 0 ? cardWidth : 0;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}