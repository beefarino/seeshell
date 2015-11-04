using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CodeOwls.SeeShell.Common;
using CodeOwls.SeeShell.Visualizations.ViewModels;
using OxyPlot;

namespace CodeOwls.SeeShell.Visualizations.Views
{
    /// <summary>
    /// Interaction logic for VisualizationView.xaml
    /// </summary>
    public partial class VisualizationView : UserControl
    {
        private Timer _timer;
        private readonly Log _log;

        public VisualizationView()
        {
            InitializeComponent();
            _log = new Log(GetType());
            DataContextChanged += VisualizationView_DataContextChanged;
            //_timer = new Timer( OnTimer, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
        }

        private void OnTimer(object state)
        {
            UpdatePlot();
        }

        private void UpdatePlot()
        {
            if (Dispatcher.CheckAccess())
            {
                var model = DataContext as OxyPlotVisualizationViewModel;
                if (null != model)
                {
                    using (_log.PushContext("InvalidatePlot"))
                    {
                        model.PlotViewModel.InvalidatePlot(true);
                    }
                }
            }
            else
            {
                Dispatcher.Invoke((Action) UpdatePlot);
            }
        }

        void VisualizationView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var plotModel = e.NewValue as OxyPlotVisualizationViewModel;
            var oldPlotModel = e.OldValue as OxyPlotVisualizationViewModel;

            if (null != oldPlotModel)
            {
                try
                {
                    oldPlotModel.PlotDataRefresh -= OnPlotDataRefresh;
                }
                catch
                {
                }
            }

            if (null == plotModel)
            {
                return;
            }

            plotModel.PlotDataRefresh += OnPlotDataRefresh;
        }

        private void OnPlotDataRefresh(object s, EventArgs a)
        {
            this.Plot.InvalidatePlot(true);
        }
    }
}
