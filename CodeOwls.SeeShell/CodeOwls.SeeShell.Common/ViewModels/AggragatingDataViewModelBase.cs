using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CodeOwls.SeeShell.Common.ViewModels
{
    public abstract class AggragatingDataViewModelBase<T> : DataViewModelBase
        where T : DataViewModelBase
    {
        private readonly ObservableCollection<object> _allRecords;
        private ObservableCollection<T> _series;
        private bool _hasData;

        protected AggragatingDataViewModelBase()
        {
            _allRecords = new ObservableCollection<object>();
        }

        public override ObservableCollection<object> AllRecords
        {
            get { return _allRecords; }
        }

        protected void Aggragate( ObservableCollection<T> newSeries )
        {
            Unsubscribe(_series);
            Subscribe(newSeries);
            _series = newSeries;
        }

        private void Subscribe( ObservableCollection<T> series )
        {
            if( null == series )
            {
                return;
            }

            series.CollectionChanged += OnSeriesCollectionChanged;
        }

        private void OnSeriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }

            foreach (T item in e.NewItems)
            {
                foreach( var record in item.AllRecords )
                {
                    AllRecords.Add( record );
                }
                item.AllRecords.CollectionChanged += OnRecordCollectionChanged;
            }
        }

        private void OnRecordCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }

            foreach (var item in e.NewItems)
            {
                _allRecords.Add( item );
            }
            NotifyOfPropertyChange(()=>AllRecordsCount);
        }

        private void Unsubscribe(ObservableCollection<T> series)
        {
            if( null == series )
            {
                return;
            }
            
            AllRecords.Clear();

            foreach( T item in series )
            {
                try
                {
                    item.AllRecords.CollectionChanged -= OnRecordCollectionChanged;
                }
                catch 
                {
                }
            }
            
            try
            {
                series.CollectionChanged -= OnSeriesCollectionChanged;
            }
            catch
            {                
            }
        }

    }
}