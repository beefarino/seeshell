using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace CodeOwls.SeeShell.Common
{
    public interface IScaleDescriptor : INotifyPropertyChanged
    {
        Color Color { get; }
        string Name { get; }
        ObservableCollection<IRangeDescriptor> Ranges { get;  }

        double Minimum { get; }
        double Maximum { get; }
        IScaleDescriptor ForPropertyName(string propertyName);
    }

    public static class ScaleDescriptorExtensions
    {
        public static Color? GetColorOfScaleValue( this IScaleDescriptor _this, double value )
        {
            var colors = (from r in _this.Ranges
                    where r.Maximum >= value && r.Minimum <= value
                    select r.Color);
            return colors.FirstOrDefault();
        }

        public static Color GetColorOfScaleValueOrScaleDefault(this IScaleDescriptor _this, double value)
        {
            var color = (from r in _this.Ranges
                          where r.Maximum >= value && r.Minimum <= value
                          select r.Color).FirstOrDefault();

            if (! color.HasValue )
            {
                return _this.Color;
            }

            return color.Value;
        }
    }
}
