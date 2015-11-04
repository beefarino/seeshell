using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Linq;
using CodeOwls.SeeShell.Common.DataSources;
using CodeOwls.SeeShell.Common.Exceptions;
using CodeOwls.SeeShell.Common.Triggers;
using CodeOwls.SeeShell.Common.ViewModels;
using CodeOwls.SeeShell.PowerShell.Cmdlets;

namespace CodeOwls.SeeShell.Common
{    
    
    //TODO: add prop change notifications
    public class PowerShellDataSource : DataViewModelBase, IPowerShellDataSource
    {
        private static readonly Log _log = new Log( typeof(PowerShellDataSource));
        private System.Management.Automation.PowerShell _powerShell;
        private IAsyncResult _asyncResult;
        private readonly ObservableCollection<ProgressRecord> _progressRecords;
        private ObservableCollection<object> _data;
        private string _scriptBlock;
        private ManualResetEvent _scriptCompleteEvent;
        private int _dataCollectionMaxSize;
        private ITrigger _trigger;
        private readonly ScaleDescriptorAssignmentCollection _scales;
        private readonly List<PSMemberInfo> _dynamicMembers;
        private readonly List<DynamicMemberSpecification> _specs;
        private readonly ObservableCollection<object> _allRecords;
        private readonly object _dataSyncObject = new object();
        private ObservableCollection<object> _transactionDataSet;

        public PowerShellDataSource()
        {
            _allRecords = new ObservableCollection<object>();
            _specs = new List<DynamicMemberSpecification>();
            _scales = new ScaleDescriptorAssignmentCollection();
            _scales.CollectionChanged += OnScaleDescriptorAssignmentCollectionChanged;
            
            _dataCollectionMaxSize = 20;
            _data = new ObservableCollection<object>();
            _progressRecords = new ObservableCollection<ProgressRecord>();
            _dynamicMembers = new List<PSMemberInfo>();

            _powerShell = System.Management.Automation.PowerShell.Create();
            var runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            runspace.SessionStateProxy.SetVariable( "seeShellDataSource", this );
            var tmp = System.Management.Automation.PowerShell.Create();
            tmp.Runspace = runspace;
            tmp.AddScript("function start-seeShellDataSet { $seeShellDataSource.StartDataSet(); }")
               .AddScript("function commit-seeShellDataSet { $seeShellDataSource.CommitDataSet(); }")
               .AddScript("function undo-seeShellDataSet { $seeShellDataSource.RollbackDataSet(); }");
            tmp.Invoke();

            _powerShell.Runspace = runspace;
            _powerShell.InvocationStateChanged += InvocationStateChanged;
            _powerShell.Streams.Debug.DataAdded += DebugRecordAdded;
            _powerShell.Streams.Verbose.DataAdded += VerboseRecordAdded;
            _powerShell.Streams.Progress.DataAdded += ProgressRecordAdded;
            _powerShell.Streams.Error.DataAdded += ErrorRecordAdded;
            _powerShell.Streams.Warning.DataAdded += WarningRecordAdded;
        }

        public void StartDataSet()
        {
            _transactionDataSet = new ObservableCollection<object>();

        }

        public void CommitDataSet()
        {
            _data = _transactionDataSet;

            Application.Current.Dispatcher.BeginInvoke((Action)(()=> NotifyOfPropertyChange(() => Data)) );
        }

        public void RollbackDataSet()
        {
            _transactionDataSet = null;
        }

        private void OnScaleDescriptorAssignmentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if( e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }

        }


        public WaitHandle ScriptComplete
        {
            get
            {
                if( null == _asyncResult )
                {
                    return null;
                }
                return _asyncResult.AsyncWaitHandle;                
            }
        }

        public ScaleDescriptorAssignmentCollection Scales
        {
            get { return _scales; }
        }

        public ITrigger Trigger
        {
            get { return _trigger; }
            set 
            { 
                if( null != _trigger )
                {
                    _trigger.Trigger -= OnTrigger;
                }
                _trigger = value;
                _trigger.Trigger += OnTrigger;
            }
        }

        public bool UseDispatcher { get; set; }

        public string ScriptBlock
        {
            get { return _scriptBlock; }
            set
            {
                EndCurrentAsyncResult();
                ClearPowerShellState();
                _scriptBlock = value;
                _powerShell.AddScript(_scriptBlock, false);

            }
        }

        private void ClearPowerShellState()
        {
            _powerShell.Commands.Clear();
            var e = _scriptCompleteEvent;
            if( null != e )
            {
                e.Close();
                e.Dispose();
            }
            _scriptCompleteEvent = new ManualResetEvent(false);
        }

        private void EndCurrentAsyncResult()
        {
            var asyncResult = Interlocked.Exchange(ref _asyncResult, null);

            if (null == asyncResult)
            {
                return;
            }

            _powerShell.EndInvoke(asyncResult);
        }

        public PSInvocationStateInfo InvocationState
        {
            get { return _powerShell.InvocationStateInfo; }
        }

        public ObservableCollection<ProgressRecord> ProgressRecords
        {
            get { return _progressRecords; }
        }

        public override ObservableCollection<object> AllRecords
        {
            get { return _allRecords; }
        }

        public ObservableCollection<object> Data
        {
            get { return _data; }
        }

        public void AddDynamicMember(PSMemberInfo plotMember)
        {
            AddDynamicMemberAndScale(plotMember);
        }

        private void AddDynamicMemberAndScale(PSMemberInfo plotMember)
        {
            if (null == plotMember)
            {
                return;
            }
            
            SafeAddDynamicMember(plotMember);
            
            var name = plotMember.Name;
            AddDynamicScaleForProperty(name);
        }

        private void SafeAddDynamicMember(PSMemberInfo plotMember)
        {
            using (_log.PushContext("SafeAddDynamicMember"))
            {
                if (null == plotMember)
                {
                    _log.Debug("no plot member specified");
                    return;
                }

                if (!(from dm in _dynamicMembers where Name == plotMember.Name select dm).Any())
                {
                    _log.DebugFormat( "adding dynamic member [{0}]", plotMember.Name );
                    _dynamicMembers.Add(plotMember);
                }
            }
        }

        public IScaleDescriptorAssignment AddDynamicScaleForProperty(string name)
        {
            var scaleAssignment = this.Scales.ForProperty(name);
            if (null == scaleAssignment)
            {
                var scale = new DynamicPropertyScaleDescriptor(this, name);
                scaleAssignment = new ScaleDescriptorAssignment {PropertyName = name, Scale = scale};
                Scales.Add(scaleAssignment);
            }
            return scaleAssignment;
        }

        public int DataCollectionMaxSize
        {
            get { return _dataCollectionMaxSize; }
            set { 
                _dataCollectionMaxSize = value;
                EnforceDataCollectionLimit();
                NotifyOfPropertyChange(()=>DataCollectionMaxSize);
            }
        }

        private void EnforceDataCollectionLimit()
        {
            if( -1 == _dataCollectionMaxSize || _data.Count <= _dataCollectionMaxSize )
            {
                return;
            }

            var overCount = _data.Count - _dataCollectionMaxSize;
            while( overCount-- > 0 )
            {
                _data.RemoveAt(0);
            }
        }

        private void WarningRecordAdded(object sender, DataAddedEventArgs e)
        {
            var record = _powerShell.Streams.Warning[e.Index];

            AllRecords.Add(record);
        }

        private void ErrorRecordAdded(object sender, DataAddedEventArgs e)
        {
            var record = _powerShell.Streams.Error[e.Index];

            AllRecords.Add(record);
        }

        private void ProgressRecordAdded(object sender, DataAddedEventArgs e)
        {
            var record = _powerShell.Streams.Progress[e.Index];
            ProgressRecords.Add(record);
        }

        private void VerboseRecordAdded(object sender, DataAddedEventArgs e)
        {
            var record = _powerShell.Streams.Verbose[e.Index];

            AllRecords.Add(record);
        }

        private void DebugRecordAdded(object sender, DataAddedEventArgs e)
        {
            var record = _powerShell.Streams.Debug[e.Index];

            AllRecords.Add(record);
        }

        private void InvocationStateChanged(object sender, PSInvocationStateChangedEventArgs e)
        {
            if (e.InvocationStateInfo.State != this.InvocationState.State ||
                e.InvocationStateInfo.Reason != this.InvocationState.Reason)
            {
                NotifyOfPropertyChange(() => InvocationState);
            }
        }

        public void Dispose()
        {
            EndCurrentAsyncResult();

            var ps = Interlocked.Exchange(ref _powerShell, null);
            if (null == ps)
            {
                return;
            }
            
            ps.Runspace.Close();
            ps.Runspace.Dispose();
            ps.Dispose();
        }

        protected virtual PSDataCollection<object> CurrentScriptBlockInput
        {
            get { return null; }
        }

        void ExecuteScriptBlock()
        {
            PSDataCollection<PSObject> data = new PSDataCollection<PSObject>();
            
            data.DataAdded += (s, a) =>
                                  {                                      
                                      PSObject ps = data[a.Index];
                                      AddDataObject(ps);
                                  };

            var input = CurrentScriptBlockInput;
            _asyncResult = _powerShell.BeginInvoke(input, data);           
        }

        public void AddDataObject(PSObject ps)
        {
            using (_log.PushContext("AddDataObject"))
            {
                var solidifier = new PSObjectSolidifier();
                UpdateDynamicMembers(ps);

                AddDynamicMembersToPSObject(ps);
                AddMagicPropertiesToPSObject(ps);

                object d = solidifier.AsConcreteType(ps);

                if (UseDispatcher &&
                    null != Application.Current &&
                    null != Application.Current.Dispatcher &&
                    !Application.Current.Dispatcher.CheckAccess())
                {
                    //TODO: reconcile begininvoke and just invoke
                    Application.Current.Dispatcher.BeginInvoke(new SendOrPostCallback(AddDataObjectToCollection), d);
                }
                else
                {
                    AddDataObjectToCollection(d);
                }
            }
        }

        private void AddDataObjectToCollection(object t)
        {
            if (null != _transactionDataSet)
            {
                _transactionDataSet.Add(t);
                return;
            }

            lock (_dataSyncObject)
            {
                try
                {
                    _data.Add(t);
                }
                catch (Exception x)
                {
                    AllRecords.Add(
                        new ErrorRecord(
                            x,
                            "SeeShell.DataSource.AddDataException",
                            ErrorCategory.InvalidOperation,
                            t
                            )
                        );
                }
                EnforceDataCollectionLimit();
            }
        }

        private void OnTrigger(object sender, EventArgs e)
        {
            ExecuteScriptBlock();
        }

        public void AddDynamicMemberSpecification( DynamicMemberSpecification spec )
        {
            lock (_specs)
            {
                _specs.Add(spec);

                ApplyDynamicMemberSpecifications();
            }
        }

        private void ApplyDynamicMemberSpecifications()
        {
            using (_log.PushContext("ApplyDynamicMemberSpecifications"))
            {
                if (0 < Data.Count)
                {
                    lock (_dataSyncObject)
                    {
                        var newData = Data.ToList().ConvertAll(o => ((SolidPSObjectBase) o).PSObject).ToList();

                        UpdateDynamicMembers(newData.First());
                        newData.ForEach(pso =>
                        {
                            AddDynamicMembersToPSObject(pso);
                            AddMagicPropertiesToPSObject(pso);
                        });

                        var solidifier = new PSObjectSolidifier();
                        for (int i = 0; i < newData.Count; ++i)
                        {
                            AddIndexToPSObject(newData[i], i);
                            Data[i] = solidifier.AsConcreteType(newData[i]);
                        }
                    }
                }
                else if (0 < AllRecordsCount)
                {
                    UpdateDynamicMembers(new PSObject());
                }
            }
        }

        private void AddIndexToPSObject(PSObject ps, int i)
        {
            var prop = ps.Properties.Match(MagicNames.DataItemIndexPropertyName).FirstOrDefault();
            if (null == prop)
            {
                var m = new PSNoteProperty(MagicNames.DataItemIndexPropertyName, i);
                ps.Properties.Add(m);
            }
            else
            {
                prop.Value = i;
            }
        }

        private void AddDynamicMembersToPSObject(PSObject item)
        {
            if (null == item)
            {
                return;
            }
            _dynamicMembers.ToList().ForEach((psm) =>
                                                 {
                                                     if (null == item.Members[psm.Name])
                                                     {
                                                         try
                                                         {
                                                             item.Members.Add(psm);
                                                         }
                                                         catch( Exception e )
                                                         {
                                                             _log.Error(
                                                                 String.Format(
                                                                     "an exception was raised while adding member [{0}] to data item",
                                                                     psm.Name),
                                                                 e);
                                                         }
                                                     }
                                                 }
                );
        }

        IScaleDescriptorAssignment GetOrCreateScaleAssignment(string propertyName, PSPropertyInfo member)
        {
            using (_log.PushContext("GetOrCreateScaleAssignment [{0}]", propertyName))
            {
                var scaleAssignment = (from s in Scales
                    where StringComparer.InvariantCultureIgnoreCase.Equals(s.PropertyName, propertyName)
                    select s).FirstOrDefault();

                if (null == scaleAssignment && null == member)
                {
                    return null;
                }

                if (null != scaleAssignment)
                {
                    Scales.Add(new ScaleDescriptorAssignment {PropertyName = member.Name, Scale = scaleAssignment.Scale});
                    return scaleAssignment;
                }

                return AddDynamicScaleForProperty(member.Name);
            }
        }

        void UpdateDynamicMembers(PSObject ps)
        {
            List<DynamicMemberSpecification> specs;
            lock (_specs)
            {                                   
                if (0 == _specs.Count)
                {
                    return;
                }
                specs = _specs.ToList();
                _specs.Clear();
            }

            var factory = new DynamicMemberFactory(this, specs, ps);
            var dynamicMembers = factory.CreateDynamicMembers();
            foreach( var m in dynamicMembers )
            {
                SafeAddScaleDescriptorAssignment(m);
                SafeAddDynamicMember( m.MemberInfo );
            }
        }

        void AddMagicPropertiesToPSObject( PSObject ps )
        {
            if (!ps.Properties.Match(MagicNames.DataSourcePropertyName).Any())
            {
                var m = new PSNoteProperty(MagicNames.DataSourcePropertyName, this);
                ps.Properties.Add(m);
            }
            //Scales.ToList().ForEach(sda =>
            //                            {
            //                                try
            //                                {
            //                                    var sm = new PSNoteProperty(sda.PropertyName + "_Scale", sda);
            //                                    ps.Properties.Add(sm);
            //                                }
            //                                catch
            //                                {                                                
            //                                }
            //                            });

        }
        private void SafeAddScaleDescriptorAssignment(DynamicMemberDescriptor m)
        {
            using (_log.PushContext("SafeAddScaleDescriptorAssignment"))
            {
                IScaleDescriptor sd = m.ScaleDescriptor;
                if (null == sd)
                {
                    _log.DebugFormat("getting/creating scale descriptor for {0}", m.MemberInfo.Name);
                    GetOrCreateScaleAssignment(m.MemberInfo.Name, m.MemberInfo);
                }
                else
                {
                    _log.DebugFormat("adding scale descriptor assignment for {0} to {1}", m.MemberInfo.Name,
                                     m.ScaleDescriptor.Name);
                    Scales.Add(new ScaleDescriptorAssignment
                                   {PropertyName = m.MemberInfo.Name, Scale = m.ScaleDescriptor});
                }
            }
        }
    }
}