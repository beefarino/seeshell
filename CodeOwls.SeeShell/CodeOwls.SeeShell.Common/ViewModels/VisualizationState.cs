using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CodeOwls.SeeShell.Common.Utility;

namespace CodeOwls.SeeShell.Common.ViewModels
{
    
    public class VisualizationStateViewModel : ViewModelBase
    {
        public VisualizationStateViewModel( string name, ObservableCollection<object> allRecords  )
        {
            _name = name;
            _allRecords = allRecords;
            _recordsPaneVisiblity = Visibility.Collapsed;

            _showOrHideRecordsCommand = new DelegateCommand<object>(OnShowOrHideRecords, CanShowOrHideRecords);
            _exportImageCommand = new DelegateCommand<object>(OnExportImage, CanExportImage);

            _allRecords.CollectionChanged += 
                (s, a) =>
                    {
                        Action action = () =>
                                     {
                                         NotifyOfPropertyChange(() => HasRecords);
                                         _showOrHideRecordsCommand.RaiseCanExecuteChanged();
                                     };
                        if( null != Dispatcher && ! Dispatcher.CheckAccess() )
                        {
                            Dispatcher.BeginInvoke(action, DispatcherPriority.Normal, null);
                        }
                        else 
                        {
                            action(); 
                        }
                    };
            
        }

        private bool CanExportImage(object arg)
        {
            return true;
        }

        private void OnExportImage(object obj)
        {
            if (null == Visualization)
            {
                return;
            }

            VisualImageExporter exporter = new VisualImageExporter();
            
            using (Stream stm = File.Create( _name + ".png"))
            {
                exporter.Export( Visualization, stm );
            }
        }

        private bool CanShowOrHideRecords(object arg)
        {
            return HasRecords;
        }

        private void OnShowOrHideRecords(object obj)
        {
            if( RecordsPaneVisibility == Visibility.Visible )
            {
                RecordsPaneVisibility = Visibility.Collapsed;
            }
            else
            {
                RecordsPaneVisibility = Visibility.Visible;
            }
        }

        private readonly DelegateCommand<object> _showOrHideRecordsCommand;
        public ICommand ShowOrHideRecordsCommand
        {
            get { return _showOrHideRecordsCommand; }
        }

        private readonly DelegateCommand<object> _exportImageCommand;
        public ICommand ExportImageCommand
        {
            get { return _exportImageCommand; }
        }

        private Visibility _recordsPaneVisiblity;

        public FrameworkElement Visualization { get; set; }

        public Visibility RecordsPaneVisibility
        {
            get { return _recordsPaneVisiblity; }
            set
            {
                _recordsPaneVisiblity = value;
                NotifyOfPropertyChange(() => RecordsPaneVisibility);
            }
        }

        private ObservableCollection<object> _allRecords;
        public ObservableCollection<object> AllRecords
        {
            get { return _allRecords; }
            set
            {
                _allRecords = value;
                NotifyOfPropertyChange(() => AllRecords);
            }
        }

        public bool HasRecords
        {
            get { return _allRecords.Count != 0; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }
    }
}
