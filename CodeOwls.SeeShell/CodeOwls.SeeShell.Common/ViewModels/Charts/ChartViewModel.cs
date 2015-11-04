using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using CodeOwls.SeeShell.Common.Attributes;

namespace CodeOwls.SeeShell.Common.ViewModels.Charts
{
    [DriveInfo( "Charts", "", "SeeShell Charts Repository")]
    public class ChartViewModel : AggragatingDataViewModelBase<ChartSeriesViewModel>
    {
        private ObservableCollection<ChartSeriesViewModel> _chartSeries;
        //private ObservableCollection<ChartAxisViewModel> _chartAxes;

        public ChartViewModel()
        {
            ChartSeries = new ObservableCollection<ChartSeriesViewModel>();
            //_chartAxes = new ObservableCollection<ChartAxisViewModel>();
            HasLegend = false;
        }

        public bool HasLegend { get; set; }

        [Container]
        public ObservableCollection<ChartSeriesViewModel> ChartSeries
        {
            get { return _chartSeries; }
            private set
            {
                Aggragate( value );
                if( null != _chartSeries )
                {
                    Unsubscribe();
                }

                _chartSeries = value;
                Subscribe();

                NotifyOfPropertyChange(() => ChartSeries);
            }
        }

        private void Subscribe()
        {
            _chartSeries.CollectionChanged += OnSeriesCollectionChanged;            
            _chartSeries.ToList().ForEach(Subscribe);
        }

        private void Subscribe(ChartSeriesViewModel series)
        {
            series.AxesUpdated += OnSeriesAxesUpdated;            
        }

        private void Unsubscribe()
        {
            _chartSeries.ToList().ForEach( Unsubscribe );
            _chartSeries.CollectionChanged -= OnSeriesCollectionChanged;
        }

        private void OnSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if( e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace )
            {
                ( from ChartSeriesViewModel vm in  e.NewItems
                  select vm ).ToList().ForEach( Subscribe );
            }

            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                (from ChartSeriesViewModel vm in e.OldItems
                 select vm).ToList().ForEach(Unsubscribe);
            }
        }

        private void Unsubscribe(ChartSeriesViewModel series)
        {
            series.AxesUpdated -= OnSeriesAxesUpdated;
        }

        private void OnSeriesAxesUpdated(object sender, EventArgs e)
        {
            var series = _chartSeries.LastOrDefault();
            if( null == series )
            {
                return;
            }

            _chartSeries.Remove(series);
            _chartSeries.Add(series);
        }

        [Container]
        public IEnumerable<ChartAxisViewModel> ChartAxes
        {
            get 
            {
                List<ChartAxisViewModel> axes = new List<ChartAxisViewModel>();
                axes.AddRange(
                    from series in this.ChartSeries
                    select series.XAxis
                    );
                axes.AddRange(
                    from series in this.ChartSeries
                    select series.YAxis);
                var axesDistinct = axes.Distinct(new ChartAxisViewModelNameComparer()).ToList();
                return axesDistinct;
            }
        }

        [Container]
        public IEnumerable<IPowerShellDataSource> DataSources
        {
            get
            {
                return (
                    from series in this.ChartSeries
                    select series.DataSource ).Distinct();
            }
        }
    }

    public class ChartAxisViewModelNameComparer : IEqualityComparer<ChartAxisViewModel>
    {
        public bool Equals(ChartAxisViewModel x, ChartAxisViewModel y)
        {
            if (null == x) return null == y;

            return x.AxisScaleDescriptors.First().Scale.Name == y.AxisScaleDescriptors.First().Scale.Name;
        }

        public int GetHashCode(ChartAxisViewModel obj)
        {
            if (null == obj) return 0;
            var descriptor = obj.AxisScaleDescriptors.First();
            if (null == descriptor)
            {
                return obj.Name.GetHashCode();
            }
            return descriptor.Scale.Name.GetHashCode();
        }
    }
}
