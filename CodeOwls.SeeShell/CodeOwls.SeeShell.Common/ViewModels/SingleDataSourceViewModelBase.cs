using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using CodeOwls.SeeShell.Common.Attributes;

namespace CodeOwls.SeeShell.Common.ViewModels
{
    public abstract class SingleDataSourceViewModelBase : DataViewModelBase
    {
        private IPowerShellDataSource _dataSource;
        private IDictionary<Regex, IScaleDescriptor> _scaleDescriptors;

        public SingleDataSourceViewModelBase()
        {
            _scaleDescriptors = new Dictionary<Regex, IScaleDescriptor>();
            DispatcherUpdated += OnDispatcherUpdated;
        }

        public override System.Collections.ObjectModel.ObservableCollection<object> AllRecords
        {
            get
            {
                if( null == DataSource )
                {
                    return null;
                }
                return DataSource.AllRecords;
            }
        }
        void OnDispatcherUpdated( object sender, EventArgs args)
        {
            if( null == DataSource)
            {
                return;
            }

            DataSource.Dispatcher = Dispatcher;
        }

        protected virtual void OnDataSourceChanged( IPowerShellDataSource oldDataSource, IPowerShellDataSource newDataSource )
        {            
        }


        [ScriptBlockDataSourceArgumentTransformation]        
        [PathArgumentTransformation]
        [Parameter(ValueFromPipeline = true)]
        public IPowerShellDataSource DataSource
        {
            get { return _dataSource; }
            set
            {
                var oldDataSource = _dataSource;
                try
                {
                    AllRecords.CollectionChanged -= OnAllRecordsCollectionChanged;
                }
                catch 
                {
                }
                _dataSource = value;
                OnDataSourceChanged( oldDataSource, _dataSource );
                AllRecords.CollectionChanged += OnAllRecordsCollectionChanged;
                NotifyOfPropertyChange(() => DataSource);
                NotifyOfPropertyChange(() => AllRecords);
                NotifyOfPropertyChange(() => AllRecordsCount);
            }
        }

        public IScaleDescriptor GetScaleForProperty( string propertyName )
        {
            var scale = (from sd in _scaleDescriptors
                         where sd.Key.IsMatch(propertyName)
                         select sd.Value).FirstOrDefault();   

            if( null != scale )
            {
                //System.Diagnostics.Debugger.Launch();
                return scale.ForPropertyName(propertyName);
            }

            var scaleAssignment = _dataSource.Scales.ForProperty(propertyName);
            if( null != scaleAssignment)
            {
                scale = scaleAssignment.Scale;
            }
            return scale;
        }



        public IDictionary<Regex, IScaleDescriptor> ScaleDescriptors
        {
            get { return _scaleDescriptors; }
            set { _scaleDescriptors = value; }
        }

        private void OnAllRecordsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyOfPropertyChange(()=>AllRecordsCount);
        }
    }
}
