using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace CodeOwls.SeeShell.Common.ViewModels
{
    public class VisualizationViewModel : ViewModelBase, IDisposable
    {
        public VisualizationViewModel( DataViewModelBase dataViewModel )
        {
            _dataViewModel = dataViewModel;
        }

        private DataViewModelBase _dataViewModel;
        public DataViewModelBase DataViewModel
        {
            get { return _dataViewModel; }
            set
            {
                _dataViewModel = value;

                NotifyOfPropertyChange(() => DataViewModel);
                NotifyOfPropertyChange(() => Items);

            }
        }

        public ObservableCollection<object> Items
        {
            get { return new ObservableCollection<object> { DataViewModel }; }
        }

        public event EventHandler OnDispose;

        public void Dispose()
        {
            var ev = OnDispose;
            if( null != ev )
            {
                ev(this, EventArgs.Empty);
            }
        }
    }
}