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
    public class ScaleDescriptor : Caliburn.Micro.PropertyChangedBase, IScaleDescriptor
    {
        private Color? _color;

        public ScaleDescriptor()
        {
            
        }

        public ScaleDescriptor( string descriptor ) :
            this( descriptor.Split(',', ';') )
        {
            
        }

        public ScaleDescriptor( IEnumerable<object> descriptors )
        {
            /*
             * '<serieslabel>','<seriescolor>'(,<rangespec>)?
             * "<serieslabel>",$null(,<rangespec>)?
             * '<serieslabel>'
             */
            var items = descriptors.ToList();
            Name = items.Shift().ToString();
            if( ! items.Any() || null == items[0])
            {
                //Color = ColorManager.AssignColor();
            }

            Ranges = new ObservableCollection<IRangeDescriptor>();

            if( ! items.Any() )
            {
                return;
            }

            var tag = items[0];
            var color = tag.ToColor();
            
            if( null != color )
            {
                items.Shift();
                Color = color.Value;
            }
            else
            {
            //    Color = ColorManager.AssignColor();
            }
            Ranges = new ObservableCollection<IRangeDescriptor>(RangeDescriptorFactory.Create(Color, items).ToList());
        }

        public ScaleDescriptor(string name, Color color)
        {
            Name = name;
            Color = color;
            Ranges = new ObservableCollection<IRangeDescriptor>(RangeDescriptorFactory.Create(color, new object[]{} ).ToList());
        }

        ScaleDescriptor(IScaleDescriptor descriptors, Color color)
        {
            Name = descriptors.Name;
            Color = color;
            Ranges = descriptors.Ranges;

        }


        public override string ToString()
        {
            var items = new List<string> {Name};
            Ranges.ToList().ForEach( i=>items.Add( i.ToString() ) );
            return String.Join(",", items.ToArray());
        }

        public string Name { get; private set; }

        public ObservableCollection<IRangeDescriptor> Ranges { get; private set; }

        public Color Color
        {
            get
            {
                if (!_color.HasValue)
                {
                    _color = ColorManager.AssignColor();
                }
                return _color.Value;
            }
            private set { _color = value; }
        }

        public double Minimum
        {
            get
            {
                if( ! Ranges.Any())
                {
                    return 0;
                }
                return (from r in Ranges select r.Minimum).Min();
            }
        }

        public double Maximum
        {
            get
            {
                if (!Ranges.Any())
                {
                    return 0;
                }
                return (from r in Ranges select r.Maximum).Max();
            }
        }

        private readonly IDictionary<string, Color> _propertyColorMap = new Dictionary<string, Color>();
        public IScaleDescriptor ForPropertyName(string propertyName)
        {
            if (_color.HasValue)
            {
                return this;
            }

            if (!_propertyColorMap.ContainsKey(propertyName))
            {
                _propertyColorMap.Add( propertyName, ColorManager.AssignColor() );
            }

            var color = _propertyColorMap[propertyName];
            var descriptor = new ScaleDescriptor( this, color );
            

            return descriptor;
        }
    }
}