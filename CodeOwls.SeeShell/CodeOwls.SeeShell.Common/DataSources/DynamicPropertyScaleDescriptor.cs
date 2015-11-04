using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Media;
using CodeOwls.SeeShell.Common.DataSources;
using CodeOwls.SeeShell.Common.Utility;

namespace CodeOwls.SeeShell.Common
{
    public class DynamicPropertyScaleDescriptor : Caliburn.Micro.PropertyChangedBase, IScaleDescriptor
    {
        private readonly IPowerShellDataSource _dataSource;
        private readonly string _name;

        private readonly ObservableCollection<IRangeDescriptor> _ranges;

        private double _minimum;

        private double _maximum;
        private const double Epsilon = 0.0001d;
        private bool _firstData = true;

        public DynamicPropertyScaleDescriptor( IPowerShellDataSource dataSource, string name )
        {
            _dataSource = dataSource;
            _name = name;
            Color = ColorManager.AssignColor();
            _dataSource.Data.CollectionChanged += OnData;
            _ranges = new ObservableCollection<IRangeDescriptor>( RangeDescriptorFactory.Create(Color, new object[]{_minimum,"?",_maximum}).ToList() );
        }

        private void OnData(object sender, NotifyCollectionChangedEventArgs e)
        {
            if( e.Action != NotifyCollectionChangedAction.Add )
            {
                return;
            }

            var oldMin = _minimum;
            var oldMax = _maximum;
            foreach (SolidPSObjectBase item in e.NewItems)
            {
                double value = 0;
                if( item.GetPropTypeName(_name).Contains( "DateTime") )
                {
                    value = item.GetPropValue(_name, (DateTime i)=>i.Ticks, ()=>value);
                }
                else if (item.GetPropTypeName(_name).Contains("TimeSpan"))
                {
                    value = item.GetPropValue(_name, (TimeSpan i) => i.Ticks, () => value);
                }
                else
                {
                    value = item.GetPropValue<double>(_name);
                }

                if (_firstData)
                {
                    _minimum = _maximum = value;
                    _firstData = false;
                }
                else
                {
                    _minimum = Math.Min(_minimum, value);
                    _maximum = Math.Max(_maximum, value);
                }
            }
            if (Math.Abs(oldMin - _minimum) > Epsilon)
            {
                ((RangeDescriptor)_ranges[0]).Minimum = _minimum;
                NotifyOfPropertyChange(() => Minimum);
            }
            if (Math.Abs(oldMax - _maximum) > Epsilon)
            {
                ((RangeDescriptor)_ranges[0]).Maximum = _maximum;
                NotifyOfPropertyChange(() => Maximum);
            }
        }

        public Color Color { get; private set; }

        public string Name
        {
            get { return _name; }
        }

        public ObservableCollection<IRangeDescriptor> Ranges
        {
            get { return _ranges; }
        }

        public double Minimum
        {
            get { return _minimum; }
        }

        public double Maximum
        {
            get { return _maximum; }
        }

        public IScaleDescriptor ForPropertyName(string propertyName)
        {
            return this;
        }
    }
}