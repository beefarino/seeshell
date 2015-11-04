using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CodeOwls.SeeShell.Common.ViewModels;

namespace CodeOwls.SeeShell.Visualizations.Views
{
    /// <summary>
    /// Interaction logic for VisualizationStateSummaryView.xaml
    /// </summary>
    public partial class VisualizationStateSummaryView : UserControl
    {
        public VisualizationStateSummaryView()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var @new = e.NewValue as VisualizationStateViewModel;
            if (null == @new)
            {
                return;
            }
            @new.Visualization = this;
        }
    }
}
