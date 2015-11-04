using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Management.Automation;
using System.Windows.Threading;
using CodeOwls.SeeShell.Common.Triggers;

namespace CodeOwls.SeeShell.Common
{
    public class NullPowerShellDataSource : IPowerShellDataSource
    {
        public NullPowerShellDataSource()
        {
            Data = new ObservableCollection<object>();
            ErrorRecords = new ObservableCollection<ErrorRecord>();
            WarningRecords = new ObservableCollection<WarningRecord>();
            DebugRecords = new ObservableCollection<DebugRecord>();
            VerboseRecords = new ObservableCollection<VerboseRecord>();
            ProgressRecords = new ObservableCollection<ProgressRecord>();
            Scales = new ScaleDescriptorAssignmentCollection();
            AllRecords = new ObservableCollection<object>();
        }

        public void Dispose()
        {
            
        }

        public ScaleDescriptorAssignmentCollection Scales { get; set; }
        public ObservableCollection<object> AllRecords { get; set; }
        public ITrigger Trigger
        {
            get; set; }

        public string Name
        {
            get;set;
        }

        public Dispatcher Dispatcher
        {
            get;
            set;
        }

        public ObservableCollection<WarningRecord> WarningRecords { get; set; }

        public ObservableCollection<ErrorRecord> ErrorRecords
        {
            get;
            set;
        }

        public ObservableCollection<VerboseRecord> VerboseRecords
        {
            get;
            set;
        }

        public ObservableCollection<DebugRecord> DebugRecords
        {
            get;
            set;
        }

        public ObservableCollection<ProgressRecord> ProgressRecords
        {
            get;
            set;
        }

        public ObservableCollection<object> Data
        {
            get;
            set;
        }

        public int DataCollectionMaxSize
        {
            get;set;
        }

        public void AddDynamicMember(PSMemberInfo m)
        {
            
        }

        public IScaleDescriptorAssignment AddDynamicScaleForProperty(string name)
        {
            return null;
        }

        public void AddDynamicMemberSpecification(DynamicMemberSpecification spec)
        {

        }

        public void AddDataObject(PSObject inputObject)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyOfPropertyChange(string propertyName)
        {
            
        }

        public void Refresh()
        {
            
        }

        public bool IsNotifying { get; set; }
    }
}