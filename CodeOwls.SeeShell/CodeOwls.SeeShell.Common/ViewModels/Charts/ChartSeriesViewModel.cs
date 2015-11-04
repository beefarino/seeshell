using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using CodeOwls.SeeShell.Common.Attributes;
using CodeOwls.SeeShell.Common.Charts;
using CodeOwls.SeeShell.Common.DataSources;
using CodeOwls.SeeShell.Common.Exceptions;
using CodeOwls.SeeShell.Common.Utility;

namespace CodeOwls.SeeShell.Common.ViewModels.Charts
{
    public class ChartSeriesViewModel : SingleDataSourceViewModelBase
    {
        private static readonly Log Log = new Log( typeof( ChartSeriesViewModel));
        private ChartSeriesType _seriesType;
        private string _valueMemberPath;
        private string _highMemberPath;
        private string _lowMemberPath;
        private string _xMemberPath;
        private string _yMemberPath;
        private string _labelMemberPath;
        private string _fillMemberPath;
        private string _radiusMemberPath;
        private string _angleMemberPath;
        private ChartAxisViewModel _xAxis;
        private ChartAxisViewModel _yAxis;

        public ChartSeriesViewModel()
        {
            SeriesType = ChartSeriesType.Line;

            PropertyChanged += OnPropertyChanged;
        }

        public event EventHandler AxesUpdated;
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if( e.PropertyName.Contains("Axis") || (null != _xAxis && null != _yAxis ) )
            {
                return;
            }

            ConfigureAxes();
        }

        protected override void OnDataSourceChanged(IPowerShellDataSource oldDataSource, IPowerShellDataSource newDataSource)
        {
             ConfigureAxes( newDataSource );
 	         base.OnDataSourceChanged(oldDataSource, newDataSource);
        }

        private bool _enableConfigureAxes;
        public bool EnableConfigureAxes
        {
            get { return _enableConfigureAxes; }
            set
            {
                _enableConfigureAxes = value;
                ConfigureAxes();
            }
        }

        bool CanConfigureAxes
        {
            get { return EnableConfigureAxes && ( CanConfigureCategoricalValue || CanConfigureCategoricalRange || CanConfigureScatter || CanConfigureRadialOrPolar ); }
        }

        private bool CanConfigureCategoricalValue
        {
            get { return SeriesType.IsCategoricalValue() && null != this.ValueMemberPath && null != this.LabelMemberPath; }
        }

        private bool CanConfigureCategoricalRange
        {
            get { return SeriesType.IsCategoricalRange() && null != this.HighMemberPath && null != this.LowMemberPath && null != this.LabelMemberPath; }
        }

        private bool CanConfigureScatter
        {
            get { return SeriesType.IsScatter() && null != this.XMemberPath && null != this.YMemberPath; }
        }

        bool CanConfigureRadialOrPolar
        {
            get { return (SeriesType.IsPolar() || SeriesType.IsRadial() ) && null != this.AngleMemberPath && null != this.RadiusMemberPath; }
        }

        void ConfigureAxes()
        {
            ConfigureAxes(DataSource);
        }
        void ConfigureAxes( IPowerShellDataSource dataSource )
        {
                if (null != dataSource && null != dataSource.Data)
                {
                    bool configured = false;

                    if (0 != dataSource.Data.Count)
                    {
                        var o = dataSource.Data[0];
                        configured = TryConfigureAxes(o as SolidPSObjectBase);
                    }

                    if (!configured)
                    {
                        dataSource.Data.CollectionChanged += OnFirstDataItem;
                    }
                }
        }

        private void OnFirstDataItem(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add )
            {
                return;
            }

            DataSource.Data.CollectionChanged -= OnFirstDataItem;                       

            TryConfigureAxes(e.NewItems[0] as SolidPSObjectBase);            
        }

        bool TryConfigureAxes( SolidPSObjectBase o )
        {
            try
            {
                return ConfigureAxes(o);
            }
            catch (Exception e)
            {
                if (null != AllRecords && null != Dispatcher)
                {
                    Dispatcher.BeginInvoke(
                        (Action)
                        (() =>
                         AllRecords.Add(new ErrorRecord(e, "SeeShell.Charts.AxisConfiguration",
                                                        ErrorCategory.InvalidData, this))));
                }
                return true;
            }
        }

        private bool ConfigureAxes(SolidPSObjectBase o)
        {
            if (!CanConfigureAxes)
            {
                return false;
            }

            if( SeriesType.IsCategorical() )
            {
                ConfigureCategoricalAxes(o);
            }
            else if( SeriesType.IsCategoricalRange())
            {
                ConfigureCategoricalRange(o);
            }
            else if ( SeriesType.IsScatter() )
            {
                ConfigureScatterAxes(o);
            }
            else if( SeriesType.IsPolar())
            {
                ConfigurePolarAxis(o);
            }
            else if( SeriesType.IsRadial())
            {
                ConfigureRadialAxis(o);
            }
            else
            {
                throw new InvalidOperationException("Unanticipated series type encountered during axis determination: " + SeriesType);
            }

            VerifyAxisTypes(o);

            var sdx = GetScaleForProperty(XAxis.ValueMemberPath);
            var sdy = GetScaleForProperty(YAxis.ValueMemberPath);

            if( null == sdx )
            {
                sdx = new ScaleDescriptor( XAxis.ValueMemberPath, ColorManager.AssignColor() );
            }
                        
            XAxis.AxisScaleDescriptors.Add( new ScaleDescriptorAssignment{ PropertyName = sdx.Name, Scale = sdx } );
            
            if( null != sdy )
            {
                var sdym = new[]{ sdy }.Union( DataSource.Scales.Select(s=>s.Scale) )
                    .Where(s => s.Name == sdy.Name)                    
                    .ToList()
                    .ConvertAll(a=> new ScaleDescriptorAssignment{PropertyName = a.Name, Scale = a});
                sdym.ToList().ForEach( YAxis.AxisScaleDescriptors.Add );
            }

            var ev = AxesUpdated;
            if( null != ev )
            {
                ev(this, EventArgs.Empty);
            }

            return true;
        }

        private void VerifyAxisTypes(SolidPSObjectBase dataItem)
        {
            Log.InfoFormat( "verifying data type [{0}] for axis", dataItem.GetType().FullName );
            var validAxisTypes = SeriesType.ValidAxisTypes();
            if (!validAxisTypes.Contains(XAxis.AxisType))
            {
                throw new InvalidChartAxisTypeException(SeriesType, XAxis.AxisType);
            }
            if (!validAxisTypes.Contains(YAxis.AxisType))
            {
                throw new InvalidChartAxisTypeException(SeriesType, YAxis.AxisType);
            }

            VerifyAxisForData(XAxis, dataItem);
            VerifyAxisForData(YAxis, dataItem);
            VerifyPropertyForRadius(dataItem);
        }

        private void VerifyPropertyForRadius(SolidPSObjectBase dataItem)
        {
            if( String.IsNullOrEmpty( RadiusMemberPath) )
            {
                return;
            }

            var prop = dataItem.GetPropertyByName(RadiusMemberPath);
            if( ! IsNumericType( prop ))
            {
                throw new InvalidChartValueMemberException( SeriesType, prop, RadiusMemberPath, "Radius");
            }
        }

        private void VerifyAxisForData(ChartAxisViewModel xAxis, SolidPSObjectBase dataItem)
        {
            var prop = dataItem.GetPropertyByName(xAxis.ValueMemberPath);
            if( null == prop )
            {
                throw new ChartAxisValueMemberDoesNotExistException( SeriesType, xAxis.AxisType, xAxis.ValueMemberPath );
            }

            if( ! IsPropertyValidForAxis( prop, xAxis ))
            {
                throw new InvalidChartAxisValueMemberException(SeriesType, xAxis.AxisType, dataItem as SolidPSObjectBase, xAxis.ValueMemberPath);
            }
        
        }

        private bool IsPropertyValidForAxis(PSPropertyInfo prop, ChartAxisViewModel axis)
        {
            if( SeriesType == ChartSeriesType.Timeline )
            {
                return axis.AxisType != ChartAxisType.CategoryX;
            }

            switch( axis.AxisType )
            {
                case (ChartAxisType.NumericAngle):
                case( ChartAxisType.NumericRadius ):
                case( ChartAxisType.NumericX ):
                case( ChartAxisType.NumericY ):
                    {
                        return IsNumericType(prop);
                    }
                //case( ChartAxisType.CategoryDateTimeX ):
                //case( ChartAxisType.CategoryX ):
                //case (ChartAxisType.CategoryAngle):
                default:
                    return true;
            }
        }

        private static bool IsNumericType(PSPropertyInfo prop)
        {
            if( null == prop )
            {
                return false;
            }
            var type = Type.GetType(prop.TypeNameOfValue, false, true);
            if (null == type)
            {
                return false;
            }
            double d;
            using (DefaultRunspaceManager.ForCurrentThread)
            {
                try
                {
                    return Double.TryParse(prop.Value.ToString(), out d);
                }
                catch 
                {
                }
            }
            return false;
        }

        private void ConfigureRadialAxis(object o)
        {
            var i = o as SolidPSObjectBase;
            var value = i.GetPropValue<object>(AngleMemberPath);
            
            var r = new ChartAxisViewModel
            {
                AxisType = ChartAxisType.NumericRadius,
                AxisLocation = AxisLocation.InsideRight,

                Name = this.RadiusMemberPath,
                DataSource = this.DataSource,
                Dispatcher = this.Dispatcher,
                ValueMemberPath = RadiusMemberPath,
            };
            var a = new ChartAxisViewModel()
            {
                AxisType = ChartAxisType.CategoryAngle,
                //AxisLocation = AxisLocation.OutsideTop,
                AxisLocation = AxisLocation.OutsideBottom,
                
                Name = this.AngleMemberPath,
                DataSource = this.DataSource,
                ValueMemberPath = AngleMemberPath,
                LabelTemplate = "{" + LabelMemberPath + "}"
            };

            this.XAxis = a;
            this.YAxis = r;
        }

        private void ConfigurePolarAxis(object o)
        {
            var r = new ChartAxisViewModel
                        {
                            AxisType = ChartAxisType.NumericRadius,
                            Name = this.RadiusMemberPath,
                            DataSource = this.DataSource,
                            Dispatcher = this.Dispatcher,
                            ValueMemberPath = RadiusMemberPath,
                        };
            var a = new ChartAxisViewModel()
                        {
                            AxisType = ChartAxisType.NumericAngle,
                            AxisLocation=AxisLocation.OutsideBottom,
                            //AxisLocation=AxisLocation.OutsideTop,
                            Name = this.AngleMemberPath,
                            DataSource = this.DataSource,
                            ValueMemberPath = AngleMemberPath,
                            LabelTemplate = "{" + LabelMemberPath + "}"
                        };

            this.XAxis = a;
            this.YAxis = r;
        }

        private void ConfigureScatterAxes(object o)
        {
            var x = new ChartAxisViewModel
                        {
                            AxisType = ChartAxisType.NumericX,
                            Name = XMemberPath,
                            DataSource = DataSource,
                            ValueMemberPath = XMemberPath,
                            LabelTemplate = "{" + LabelMemberPath + "}",
                            AxisLocation = AxisLocation.OutsideBottom
                        };
            var y = new ChartAxisViewModel
                        {
                            AxisType = ChartAxisType.NumericY,
                            Name = YMemberPath,
                            DataSource = DataSource,
                            ValueMemberPath = YMemberPath,
                            AxisLocation = AxisLocation.OutsideLeft
                        };

            XAxis = x;
            YAxis = y;
        }

        private void ConfigureCategoricalRange(SolidPSObjectBase o)
        {
            var pso = o as SolidPSObjectBase;
            var xtype = GetPropertyType(pso, this.LabelMemberPath);

            ChartAxisType xaxistype = ChartAxisType.CategoryX;
            ChartAxisType yaxistype = ChartAxisType.NumericY;

            if (xtype == typeof(DateTime))
            {
                xaxistype = ChartAxisType.CategoryDateTimeX;
            }

            var x = new ChartAxisViewModel
            {
                AxisType = xaxistype,
                Name = LabelMemberPath,
                DataSource = DataSource,
                ValueMemberPath = LabelMemberPath,
                LabelTemplate = "{" + LabelMemberPath + "}",
                AxisLocation = AxisLocation.OutsideBottom,
            };
            var y = new ChartAxisViewModel
            {
                AxisType = yaxistype,
                Name = LowMemberPath ?? HighMemberPath,
                DataSource = DataSource,
                ValueMemberPath = LowMemberPath ?? HighMemberPath,
                AxisLocation = AxisLocation.OutsideLeft
            };

            XAxis = x;
            YAxis = y;
        }

        private void ConfigureCategoricalAxes(object o)
        {
            var pso = o as SolidPSObjectBase;
            var xtype = GetPropertyType(pso, this.LabelMemberPath);
            
            ChartAxisType xaxistype = ChartAxisType.CategoryX;
            ChartAxisType yaxistype = ChartAxisType.NumericY;
            
            if( xtype == typeof( DateTime ))
            {
                xaxistype = ChartAxisType.CategoryDateTimeX;
            }

            var x = new ChartAxisViewModel
                        {
                            AxisType = xaxistype,
                            Name = LabelMemberPath,
                            DataSource = DataSource,
                            ValueMemberPath = LabelMemberPath,
                            LabelTemplate = "{" + LabelMemberPath + "}",
                            AxisLocation = AxisLocation.OutsideBottom,
                        };
            var y = new ChartAxisViewModel
                        {
                            AxisType = yaxistype,
                            Name = ValueMemberPath,
                            DataSource = DataSource,
                            ValueMemberPath = ValueMemberPath,
                            AxisLocation = AxisLocation.OutsideLeft
                        };

            XAxis = x;
            YAxis = y;
        }

        private Type GetPropertyType(SolidPSObjectBase pso, string propertyName)
        {
            var value = pso.GetPropValue<object>(propertyName);
            if( null != value )
            {
                return value.GetType();
            }
            var xtypename = pso.GetPropTypeName(propertyName);
            if( null == xtypename )
            {
                return typeof (object);
            }

            var xtype = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                         from t in asm.GetTypes()
                         where t.FullName.ToLowerInvariant() == xtypename
                         select t).FirstOrDefault();
            return xtype;
        }

        [Parameter()]
        public ChartSeriesType SeriesType
        {
            get { return _seriesType; }
            set
            {
                _seriesType = value;
                NotifyOfPropertyChange(() => SeriesType);
            }
        }

        [Parameter()]
        public string ValueMemberPath
        {
            get { return _valueMemberPath; }
            set
            {
                _valueMemberPath = value;
                NotifyOfPropertyChange(() => ValueMemberPath);
            }
        }

        [Parameter()]
        [PathArgumentTransformation]
        public ChartAxisViewModel XAxis
        {
            get { return _xAxis; }
            set { _xAxis = value; NotifyOfPropertyChange(() => XAxis); }
        }

        [Parameter()]
        [PathArgumentTransformation]
        public ChartAxisViewModel YAxis
        {
            get { return _yAxis; }
            set { _yAxis = value; NotifyOfPropertyChange(() => YAxis); }
        }

        [Parameter()]
        public string HighMemberPath
        {
            get { return _highMemberPath; }
            set { _highMemberPath = value; NotifyOfPropertyChange(() => HighMemberPath); }
        }

        [Parameter()]
        public string LowMemberPath
        {
            get { return _lowMemberPath; }
            set { _lowMemberPath = value; NotifyOfPropertyChange(() => LowMemberPath); }
        }

        [Parameter()]
        public string XMemberPath
        {
            get { return _xMemberPath; }
            set { _xMemberPath = value; NotifyOfPropertyChange(() => XMemberPath); }
        }

        [Parameter()]
        public string YMemberPath
        {
            get { return _yMemberPath; }
            set { _yMemberPath = value; NotifyOfPropertyChange(() => YMemberPath); }
        }

        [Parameter()]
        public string LabelMemberPath
        {
            get { return _labelMemberPath; }
            set { _labelMemberPath = value; NotifyOfPropertyChange(() => LabelMemberPath); }
        }

        [Parameter()]
        public string FillMemberPath
        {
            get { return _fillMemberPath; }
            set { _fillMemberPath = value; NotifyOfPropertyChange(() => FillMemberPath); }
        }

        [Parameter()]
        public string RadiusMemberPath
        {
            get { return _radiusMemberPath; }
            set { _radiusMemberPath = value; NotifyOfPropertyChange(() => RadiusMemberPath); }
        }

        [Parameter()]
        public string AngleMemberPath
        {
            get { return _angleMemberPath; }
            set { _angleMemberPath = value; NotifyOfPropertyChange(() => AngleMemberPath); }
        }

        public object LiteralSeriesName { get; set; }
        public object LiteralByName { get; set; }
    }
}