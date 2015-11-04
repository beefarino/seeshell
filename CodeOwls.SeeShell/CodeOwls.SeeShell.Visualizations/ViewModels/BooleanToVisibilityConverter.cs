using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CodeOwls.SeeShell.Visualizations.ViewModels
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == value || value.GetType() != typeof(bool))
            {
                return Visibility.Collapsed;
            }

            bool isVisible = (bool)value;
            return isVisible ? Visibility.Visible : Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}