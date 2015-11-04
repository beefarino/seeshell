using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using CodeOwls.PowerShell.Provider;
using CodeOwls.SeeShell.Common.Providers;
using CodeOwls.SeeShell.Common.ViewModels;
using CodeOwls.SeeShell.Visualizations.Dashboard;


namespace CodeOwls.SeeShell.PowerShell.Providers
{
    public class Drive<T> : Drive, IDriveOf<T>, IViewMap
        where T : ViewModelBase
    {
        private readonly ObservableCollection<T> _items;
        private readonly IDictionary<T, object> _viewMap;

        public Drive(PSDriveInfo driveInfo ) : base(driveInfo)
        {
            _items = new ObservableCollection<T>();      
            _viewMap = new Dictionary<T, object>();
        }

        public object GetViewForItem(object item)
        {
            if (! _viewMap.ContainsKey(item as T))
            {
                return null;
            }

            return _viewMap[item as T];
        }

        public void Add(T item)
        {
            Add(item, true );
        }
        public void Add(T item, bool show)
        {
          
            _items.Add(item);
            var dataViewModel = item as DataViewModelBase;
            var vizViewModel = new VisualizationViewModel(dataViewModel);
            var vizualizationViewModel =
                new VisualizationWindowViewModel
                    {
                        Visualization = vizViewModel,
                        VisualizationState = new VisualizationStateViewModel(
                            item.Name, 
                            dataViewModel.AllRecords
                            )                       
                    };
            vizualizationViewModel.Visualization.OnDispose += (s,a) => Remove(item);
            var view = show ? Manager.Show( vizualizationViewModel) : Manager.Create(vizualizationViewModel);
            _viewMap.Add( item, view );
        }
        
        public void Remove(T item)
        {
            if (_items.Contains(item))
            {
                _items.Remove(item);
            }
        }

        public IEnumerable<T> Items
        {
            get { return _items; }
        }

        internal string RootNodeName
        {
            get { return this.Name; }
        }
    }
    
}
