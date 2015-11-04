using System;
using System.Collections.Generic;
using System.Text;
using CodeOwls.SeeShell.Common.ViewModels;
using OxyPlot;

namespace CodeOwls.SeeShell.Visualizations.ViewModels
{
    public class OxyPlotVisualizationViewModel : VisualizationViewModel
    {
        private readonly OxyPlotVisualizationViewModelAdapter _adapter;
        public event EventHandler PlotDataRefresh;

        public OxyPlotVisualizationViewModel(VisualizationViewModel viewModel) : base(viewModel.DataViewModel)
        {
            Dispatcher = viewModel.Dispatcher;
            Name = viewModel.Name;
            this.DataViewModel = viewModel.DataViewModel;

            OnDispose += (sender, args) => viewModel.Dispose();

            _adapter = new OxyPlotVisualizationViewModelAdapter( viewModel );
            _adapter.PlotModelChanged += _adapter_PlotModelChanged;
            _adapter.PlotDataUpdate += _adapter_PlotDataUpdate;
        }
        
        void _adapter_PlotDataUpdate(object sender, EventArgs e)
        {
            RaisePlotDataRefresh();
        }

        void _adapter_PlotModelChanged(object sender, EventArgs e)
        {
            NotifyPlotModelChange();
        }
        
        private void RaisePlotDataRefresh()
        {
            if( null == Dispatcher )
            {
                return;
            }

            var ev = PlotDataRefresh;
            if (null == ev)
            {
                return;
            }

            if (Dispatcher.CheckAccess())
            {
                ev(this, EventArgs.Empty);
            }
            else
            {
                Dispatcher.Invoke(((Action) (RaisePlotDataRefresh)));
            }
        }

        private void NotifyPlotModelChange()
        {
            if (null == Dispatcher)
            {
                return;
            }

            if (Dispatcher.CheckAccess())
            {
                NotifyOfPropertyChange(() => PlotViewModel);
            }
            else
            {
                Dispatcher.Invoke(((Action)(NotifyPlotModelChange)));
            }
        }

        public PlotModel PlotViewModel
        {
            get { return _adapter.PlotModel; }
        }
    }
}
