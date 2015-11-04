using System;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace CodeOwls.SeeShell.Visualizations.ViewModels
{
    public class BooleanToVerticalOrientationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if( null == value || value.GetType() != typeof(bool))
            {
                return Orientation.Horizontal;
            }

            bool isVertical = (bool) value;
            return isVertical ? Orientation.Vertical : Orientation.Horizontal;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
