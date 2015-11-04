using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Management.Automation;
using System.Windows;
using CodeOwls.SeeShell.Common.Charts;

namespace CodeOwls.SeeShell.Common.ViewModels.Charts
{
    public class ChartAxisViewModel : SingleDataSourceViewModelBase
    {
        public ChartAxisViewModel()
        {
            _axisScaleDescriptors = new ObservableCollection<IScaleDescriptorAssignment>();
            _labelAlignment = TextAlignment.Center;
            _labelLocation = AxisLocation.OutsideBottom;            
        }

        private object _synchronizeTarget;

        public bool Synchronized
        {
            get { return null != SynchronizeTarget; }
        }

        [Parameter()]
        public object SynchronizeTarget
        {
            get { return _synchronizeTarget; }
            set
            {
                _synchronizeTarget = value;
                NotifyOfPropertyChange(() => SynchronizeTarget);
                NotifyOfPropertyChange(() => Synchronized);
            }
        }

        private ChartAxisType _axisType;
        [Parameter()]
        public ChartAxisType AxisType
        {
            get { return _axisType; }
            set
            {
                _axisType = value;
                NotifyOfPropertyChange(() => AxisType);
            }
        }

        private AxisLocation _axisLocation;
        [Parameter()]        
        public AxisLocation AxisLocation
        {
            get { return _axisLocation; }
            set
            {
                _axisLocation = value;
                NotifyOfPropertyChange(() => AxisLocation);
                
            }
        }

        [Parameter()]
        public double LabelAngle
        {
            get { return _axisLabelAngle; }
            set
            {
                _axisLabelAngle = value;
                NotifyOfPropertyChange(() => LabelAngle);
            }
        }

        private TextAlignment _labelAlignment;
        [Parameter()]        
        public TextAlignment LabelAlignment
        {
            get { return _labelAlignment; }
            set 
            { 
                _labelAlignment = value;
                NotifyOfPropertyChange(()=>LabelAlignment);
            }
        }

        private AxisLocation _labelLocation;

        public AxisLocation LabelLocation
        {
            get { return _labelLocation; }
            set
            {
                _labelLocation = value;
                NotifyOfPropertyChange(() => LabelLocation);
            }
        }

        private string _labelTemplate;
        [Parameter()]
        public string LabelTemplate
        {
            get { return _labelTemplate; }
            set
            {
                _labelTemplate = value;
                NotifyOfPropertyChange(() => LabelTemplate);
            }
        }

        private string _valuePath;
        private double _axisLabelAngle;
        private Visibility _visibility;

        [Parameter()]
        public string ValueMemberPath
        {
            get { return _valuePath; }
            set { _valuePath = value; NotifyOfPropertyChange(() => ValueMemberPath); }
        }

        [Parameter()]
        public Visibility Visibility
        {
            get { return _visibility; }
            set { _visibility = value; NotifyOfPropertyChange(()=>Visibility); }
        }

        //public double CurrentRangeMaximum
        //{
        //    get
        //    {
        //        return _axisScaleDescriptors.ToList().Max(sd => sd.Maximum);
        //    }
        //}

        //public double CurrentRangeMinimum
        //{
        //    get
        //    {
        //        return _axisScaleDescriptors.ToList().Min(sd => sd.Minimum);
        //    }
        //}

        private ObservableCollection<IScaleDescriptorAssignment> _axisScaleDescriptors;
        public ObservableCollection<IScaleDescriptorAssignment> AxisScaleDescriptors
        {
            get { return _axisScaleDescriptors; }
            set { 
                _axisScaleDescriptors = value;
                //_axisScaleDescriptors.CollectionChanged += OnScaleDescriptorsCollectionChanged;
                NotifyOfPropertyChange(()=>AxisScaleDescriptors); 
            }
        }

        private void OnScaleDescriptorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
        }
    }
}