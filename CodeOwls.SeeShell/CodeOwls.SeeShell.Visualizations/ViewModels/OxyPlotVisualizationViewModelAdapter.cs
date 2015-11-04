using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using CodeOwls.SeeShell.Common;
using CodeOwls.SeeShell.Common.Charts;
using CodeOwls.SeeShell.Common.ViewModels;
using CodeOwls.SeeShell.Common.ViewModels.Charts;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using AreaSeries = OxyPlot.Series.AreaSeries;
using Axis = OxyPlot.Axes.Axis;
using CategoryAxis = OxyPlot.Axes.CategoryAxis;
using ColumnSeries = OxyPlot.Series.ColumnSeries;
using DateTimeAxis = OxyPlot.Axes.DateTimeAxis;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using LineSeries = OxyPlot.Series.LineSeries;
using ScatterSeries = OxyPlot.Series.ScatterSeries;
using Series = OxyPlot.Series.Series;
using StairStepSeries = OxyPlot.Series.StairStepSeries;

namespace CodeOwls.SeeShell.Visualizations.ViewModels
{
    public class OxyPlotVisualizationViewModelAdapter 
    {
        private ChartViewModel _viewModel;
        private PlotModel _plotModel;
        private readonly VisualizationViewModel _vizualizationViewModel;

        private readonly Log _log = new Log(typeof(OxyPlotVisualizationViewModelAdapter));

        public event EventHandler PlotModelChanged;
        public event EventHandler PlotDataUpdate;
        
        public OxyPlotVisualizationViewModelAdapter(VisualizationViewModel viewModel)
        {
            _plotModel = new PlotModel();
            _vizualizationViewModel = viewModel;
            
            _viewModel = viewModel.DataViewModel as ChartViewModel;

            _viewModel.ChartSeries.CollectionChanged += ChartSeries_CollectionChanged;
            
            _vizualizationViewModel.PropertyChanged += _viewModel_PropertyChanged;
        }

        void ChartSeries_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            using (_log.PushContext("ChartSeries_CollectionChanged [{0}]", e.Action))
            {                
                CreatePlotModel();
            }
        }

        void _viewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Items" || e.PropertyName == "DataViewModel")
            {
                _viewModel = _vizualizationViewModel.DataViewModel as ChartViewModel;
            
                CreatePlotModel();
            }
        }

        static bool _inPlotModelCreation = false;
        private void CreatePlotModel()
        {
            using (_log.PushContext("CreatePlotModel"))
            {
                if (_inPlotModelCreation) return;

                _log.Debug("creating new plot model");

                _inPlotModelCreation = true;
                _plotModel = new PlotModel();


                UpdateAxes();
                UpdateSeries();

                OnPlotModelChanged();
                _inPlotModelCreation = false;
            }
        }

        private void UpdateSeries()
        {
            foreach (var series in _viewModel.ChartSeries)
            {
                try
                {
                    series.DataSource.Data.CollectionChanged -= Data_CollectionChanged;
                }
                catch
                {
                }

                Series plotSeries = ConvertSeries(series);
                _plotModel.Series.Add( plotSeries );

                series.DataSource.Data.CollectionChanged += Data_CollectionChanged;
            }
        }

        void Data_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            using (_log.PushContext("Data_CollectionChanged"))
            {
                OnPlotDataUpdate();
            }
        }

        private Series ConvertSeries(ChartSeriesViewModel series)
        {
            Series newSeries = null;
            var valueSeries = series.GetScaleForProperty(series.ValueMemberPath);
            var labelSeries = series.GetScaleForProperty(series.LabelMemberPath);

            
            switch (series.SeriesType)
            {
                case( ChartSeriesType.Column):
                {
                    newSeries = new ColumnSeries
                    {
                        ValueField = series.ValueMemberPath,
                        ItemsSource = series.DataSource.Data,                        
                        FillColor = valueSeries.Color.ToOxyColor(),
                        StrokeColor = valueSeries.Color.ToOxyColor(),
                        YAxisKey = valueSeries.Name,
                        XAxisKey = labelSeries.Name,
                    };

                    break;
                }
                case( ChartSeriesType.Line ):
                {
                    newSeries = new LineSeries
                    {
                        ItemsSource = series.DataSource.Data,
                        DataFieldX = series.XMemberPath,
                        DataFieldY = series.YMemberPath,
                        MarkerType = MarkerType.Circle,
                        Color = valueSeries.Color.ToOxyColor(),
                        MarkerFill = valueSeries.Color.ToOxyColor(),
                        YAxisKey = valueSeries.Name,
                        XAxisKey = labelSeries.Name,
                    };
                    break;
                }
                case( ChartSeriesType.Spline):
                {
                    newSeries = new LineSeries
                    {
                        ItemsSource = series.DataSource.Data,
                        DataFieldX = series.XMemberPath,
                        DataFieldY = series.YMemberPath,
                        MarkerType = MarkerType.Circle,
                        Color = valueSeries.Color.ToOxyColor(),
                        MarkerFill = valueSeries.Color.ToOxyColor(),
                        Smooth = true,
                        YAxisKey = valueSeries.Name,
                        XAxisKey = labelSeries.Name,
                    };
                    break;
                }
                case(ChartSeriesType.Area):
                {
                    newSeries = new AreaSeries
                    {
                        ItemsSource = series.DataSource.Data,
                        DataFieldX = series.XMemberPath,
                        DataFieldY = series.YMemberPath,
                        Color = valueSeries.Color.ToOxyColor(),
                        Fill = valueSeries.Color.ToOxyColor(),
                        MarkerFill = valueSeries.Color.ToOxyColor(),
                        YAxisKey = valueSeries.Name,
                        XAxisKey = labelSeries.Name,
                    };
                    break;
                }
                case( ChartSeriesType.SplineArea):
                {
                    newSeries = new AreaSeries
                    {
                        ItemsSource = series.DataSource.Data,
                        DataFieldX = series.XMemberPath,
                        DataFieldY = series.YMemberPath,
                        Color = valueSeries.Color.ToOxyColor(),
                        Fill = valueSeries.Color.ToOxyColor(),
                        MarkerFill = valueSeries.Color.ToOxyColor(),
                        Smooth = true,
                        YAxisKey = valueSeries.Name,
                        XAxisKey = labelSeries.Name,
                    };
                    break;
                }
                case(ChartSeriesType.Bubble):
                {
                    newSeries = new ScatterSeries
                    {
                        ItemsSource = series.DataSource.Data,
                        DataFieldX = series.XMemberPath,
                        DataFieldY = series.YMemberPath,
                        DataFieldSize = series.RadiusMemberPath,
                        MarkerFill = valueSeries.Color.ToOxyColor(),
                        MarkerType = MarkerType.Circle,
                        YAxisKey = valueSeries.Name,
                        XAxisKey = labelSeries.Name,
                    };
                    break;
                }
                case( ChartSeriesType.StepLine):
                {
                    newSeries = new StairStepSeries
                    {
                        ItemsSource = series.DataSource.Data,
                        DataFieldX = series.XMemberPath,
                        DataFieldY = series.YMemberPath,
                        Color = valueSeries.Color.ToOxyColor(),
                        YAxisKey = valueSeries.Name,
                        XAxisKey = labelSeries.Name,
                    };
                    break;
                }
                default:
                {
                    return null;
                }
            }

            newSeries.Title = series.Name;
            
            return newSeries;
            
        }

        private void UpdateAxes()
        {
            foreach (var axis in _viewModel.ChartAxes)
            {
                Axis plotAxis = ConvertAxis(axis);
                _plotModel.Axes.Add( plotAxis );
            }
        }

        private Axis ConvertAxis(ChartAxisViewModel axis)
        {
            Axis newAxis = null;
            
            switch (axis.AxisType)
            {
                case (ChartAxisType.CategoryX):
                {
                    newAxis = new CategoryAxis
                    {
                        LabelField = axis.ValueMemberPath,
                        ItemsSource = axis.DataSource.Data
                    };
                    break;
                }
                case (ChartAxisType.NumericX):
                case (ChartAxisType.NumericY):
                {
                    newAxis = new LinearAxis
                    {
                    };
                    break;
                }
                case(ChartAxisType.CategoryDateTimeX):
                {
                    var dtaxis = new DateTimeAxis
                    {                        
                        IntervalType = DateTimeIntervalType.Auto,                        
                    };

                    var scale = axis.GetScaleForProperty(axis.ValueMemberPath);
                    if (null != scale)
                    {
                        scale.PropertyChanged += (s, a) =>
                        {
                            if( a.PropertyName != "Minimum" && a.PropertyName != "Maximum")
                            {
                                return;
                            }

                            var min = new DateTime((long)scale.Minimum);
                            var max = new DateTime((long)scale.Maximum);

                            var laxis = _plotModel.Axes.FirstOrDefault(x =>  x.Title == scale.Name) as DateTimeAxis;
                            if( null == laxis )
                            {
                                return;
                            }
                            
                            var delta = max - min;
                            
                            var lmax = 0.0d;
                            var lmin = 0.0d;

                            if (TimeSpan.FromSeconds(1) > delta)
                            {
                                laxis.IntervalType = DateTimeIntervalType.Seconds;
                            }
                            else if (TimeSpan.FromMinutes(1) > delta)
                            {
                                laxis.IntervalType = DateTimeIntervalType.Minutes;
                            }
                            else if (TimeSpan.FromHours(1) > delta)
                            {
                                laxis.IntervalType = DateTimeIntervalType.Hours;
                            }
                            else if (TimeSpan.FromDays(1) > delta)
                            {
                                laxis.IntervalType = DateTimeIntervalType.Days;
                            }
                            else if (TimeSpan.FromDays(30) > delta)
                            {
                                laxis.IntervalType = DateTimeIntervalType.Months;
                            }
                            else
                            {
                                laxis.IntervalType = DateTimeIntervalType.Auto;                            
                            }

                            //TODO: remove
                            laxis.IntervalType = DateTimeIntervalType.Seconds;

                            //laxis.Minimum = scale.Minimum;
                            //laxis.Maximum = scale.Maximum;

                            OnPlotModelChanged();
                        };
                    }
                    newAxis = dtaxis;
                    break;
                }
            }

            if (null == newAxis)
            {
                return null;
            }

            switch (axis.AxisLocation)
            {
                case(AxisLocation.InsideBottom):
                case(AxisLocation.OutsideBottom):
                {
                    newAxis.Position = AxisPosition.Bottom;
                    break;
                }
                case (AxisLocation.InsideTop):
                case (AxisLocation.OutsideTop):
                {
                    newAxis.Position = AxisPosition.Top;
                    break;
                }
                case (AxisLocation.InsideLeft):
                case (AxisLocation.OutsideLeft):
                {
                    newAxis.Position = AxisPosition.Left;
                    break;
                }
                case (AxisLocation.InsideRight):
                case (AxisLocation.OutsideRight):
                {
                    newAxis.Position = AxisPosition.Right;
                    break;
                }
                default:
                {
                    newAxis.Position = AxisPosition.None;
                    break;
                }
            }

            newAxis.Title = axis.Name;
            
            var series = axis.AxisScaleDescriptors.FirstOrDefault();
            if (null != series)
            {
                newAxis.Title = series.Scale.Name;
            }

            newAxis.Key = newAxis.Title;

            return newAxis;
        }

        public PlotModel PlotModel
        {
            get { return _plotModel; }
        }

        protected virtual void OnPlotModelChanged()
        {
            using (_log.PushContext("OnPlotModelChanged"))
            {
                var handler = PlotModelChanged;
                if (handler != null) handler(this, EventArgs.Empty);
            }
        }

        static bool _inPlotUpdate = false;
        protected virtual void OnPlotDataUpdate()
        {
            using (_log.PushContext("OnPlotDataUpdate"))
            {
                if (_inPlotUpdate) return;
                _log.Debug( "updateing plot data");
                _inPlotUpdate = true;
                var handler = PlotDataUpdate;
                if (handler != null) handler(this, EventArgs.Empty);

                _inPlotUpdate = false;
            }
        }
    }
}