using System.Windows.Media;

namespace CodeOwls.SeeShell.Common
{
    public class RangeDescriptor : Caliburn.Micro.PropertyChangedBase, IRangeDescriptor
    {
        private int _scaleIndex;
        public int ScaleIndex
        {
            get { return _scaleIndex; }
            set { _scaleIndex = value;
                NotifyOfPropertyChange(() => ScaleIndex);
            }
        }

        private double _minimum;
        public double Minimum
        {
            get { return _minimum; }
            set { _minimum = value;
                NotifyOfPropertyChange(() => Minimum);
            }
        }

        private double _maximum;
        public double Maximum
        {
            get { return _maximum; }
            set { _maximum = value;
                NotifyOfPropertyChange(() => Maximum);
            }
        }

        private Color? _color;
        public Color? Color
        {
            get { return _color; }
            set { _color = value; NotifyOfPropertyChange(()=>Color);}
        }
    }
}