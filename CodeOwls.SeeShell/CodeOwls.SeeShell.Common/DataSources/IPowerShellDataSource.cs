using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Management.Automation;
using System.Text;
using System.Windows.Threading;
using Caliburn.Micro;
using CodeOwls.SeeShell.Common;
using CodeOwls.SeeShell.Common.Triggers;

namespace CodeOwls.SeeShell.Common
{

    public interface IPowerShellDataSource : INotifyPropertyChangedEx, IDisposable
    {
        ScaleDescriptorAssignmentCollection Scales { get; }
        ITrigger Trigger { get; set; }
        string Name { get; }
        Dispatcher Dispatcher { get; set; }
        //string ScriptBlock { get; set; }
        //PSInvocationStateInfo InvocationState { get; }
        //ObservableCollection<WarningRecord> WarningRecords { get; }
        //ObservableCollection<ErrorRecord> ErrorRecords { get; }
        //ObservableCollection<VerboseRecord> VerboseRecords { get; }
        //ObservableCollection<DebugRecord> DebugRecords { get; }
        ObservableCollection<ProgressRecord> ProgressRecords { get; }
        ObservableCollection<object> AllRecords { get; }
        ObservableCollection<object> Data { get; }
        int DataCollectionMaxSize { get; set; }
        void AddDynamicMember(PSMemberInfo plotMember);
        IScaleDescriptorAssignment AddDynamicScaleForProperty(string name);
        void AddDynamicMemberSpecification( DynamicMemberSpecification spec );
        void AddDataObject(PSObject inputObject);
    }
}