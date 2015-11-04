namespace CodeOwls.SeeShell.Common.ViewModels
{
    public class DashboardPaneViewModel : ViewModelBase
    {
        public DashboardPaneViewModel()
        {
            DispatcherUpdated += (s, a) =>
            {
                if (null != _visualization)
                {
                    _visualization.Dispatcher = Dispatcher;
                }
            };
        }

        private VisualizationStateViewModel _visualizationState;

        public VisualizationStateViewModel VisualizationState
        {
            get { return _visualizationState; }
            set
            {
                _visualizationState = value;
                NotifyOfPropertyChange(() => VisualizationState);
            }
        }

        private VisualizationViewModel _visualization;

        public VisualizationViewModel Visualization
        {
            get { return _visualization; }
            set
            {
                _visualization = value;
                NotifyOfPropertyChange(() => Visualization);
            }
        }
    }
}