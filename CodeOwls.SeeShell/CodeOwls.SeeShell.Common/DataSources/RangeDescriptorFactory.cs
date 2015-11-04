using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using CodeOwls.SeeShell.Common.DataSources;
using CodeOwls.SeeShell.Common.Exceptions;
using CodeOwls.SeeShell.Common.Utility;

namespace CodeOwls.SeeShell.Common
{
    public class RangeDescriptorFactory
    {
        private readonly Color _rangeBaseColor;
        private readonly List<object> _values;
        private int _index;

        RangeDescriptorFactory( Color rangeBaseColor, IEnumerable<object> values )
        {
            _rangeBaseColor = rangeBaseColor;
            _index = 0;
            _values = values.ToList();
        }

        IRangeDescriptor Next()
        {
            /*
                int?, Color?, int?
                80, Red, 100
                Red, 100
                80, Red
                80, 100
             */

            var descriptor = new RangeDescriptor();
            double? v1 = null;
            double? v2 = null;
            Color? clr = null;

            if( _index >= _values.Count - 1 )
            {
                return null;
            }
            var first = _values[_index];
            clr = first.ToColor();
            if (null == clr)
            {
                if (first is int || first is double || first is string)
                {
                    v1 = Convert.ToDouble(first);
                }
                else
                {
                    throw new InvalidRangeDescriptorException("Expected number or color at value [" + first.ToString() + "]");
                }
            }

            ++_index;
            if (_index < _values.Count)
            {
                var second = _values[_index];
                if (null != second)
                {
                    Color? clr2 = second.ToColor();

                    if (null == clr2)
                    {

                        if ("?" == (second as string))
                        {
                            clr2 = ColorManager.AssignColor(_index, _values.Count, _rangeBaseColor);
                        }
                    }

                    if( null == clr2)
                    {
                        if (second is int || second is double || second is string)
                        {
                            v2 = Convert.ToDouble(second);
                        }
                        else
                        {
                            throw new InvalidRangeDescriptorException("Expected number or color at value [" +
                                                                      second.ToString() + "]");
                        }
                    }
                    else if( null != clr )
                    {
                        throw new InvalidRangeDescriptorException("Expected number at value [" +
                                                                  second.ToString() + "]");
                    }
                    else
                    {
                        clr = clr2;
                        ++_index;
                    }
                }
            }

            if( v1.HasValue && clr.HasValue && _index < _values.Count )
            {
                var third = _values[_index];
                if( ! (third is int || third is double || third is string) )
                {
                    throw new InvalidRangeDescriptorException( "Expected number at value [" + third.ToString() + "]");
                }

                v2 = Convert.ToDouble(third);
            }

            if( v1.HasValue && v2.HasValue )
            {
                if( v1.Value > v2.Value )
                {
                    double v = v1.Value;
                    v1 = v2;
                    v2 = v;
                }
            }

            descriptor.Color = clr;
            if (v1.HasValue)
            {
                descriptor.Minimum = v1.Value;
            }
            if (v2.HasValue)
            {
                descriptor.Maximum = v2.Value;
            }
            return descriptor;
        }

        public static IEnumerable<IRangeDescriptor> Create( Color baseColor, IEnumerable<object> values )
        {
            var factory = new RangeDescriptorFactory(baseColor, values);
            List<IRangeDescriptor> descriptors = new List<IRangeDescriptor>();
            IRangeDescriptor next = null;
            while( null != ( next = factory.Next() ) )
            {
                descriptors.Add( next );
            }

            return descriptors;
        }

        public static IEnumerable<IRangeDescriptor> Create(object[] values)
        {
            return Create(Colors.Transparent, values);
        }
    }
}