using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using CodeOwls.SeeShell.Common.Triggers;

namespace CodeOwls.SeeShell.Common.DataSources
{
    public class AcrossByPowerShellDataSource : PowerShellDataSource
    {
        private readonly IPowerShellDataSource _rootDataSource;
        private readonly object _byItem;
        private readonly object _across;
        private PSDataCollection<object> _currentInput;

        public AcrossByPowerShellDataSource(IPowerShellDataSource rootDataSource, object acrossItem, object byItem)
        {
            _rootDataSource = rootDataSource;
            _byItem = byItem;
            _across = acrossItem;
            _byItem = byItem;
            
            CreateScriptBlock();

            rootDataSource.Data.CollectionChanged += OnData;
        }

        private void OnData(object sender, NotifyCollectionChangedEventArgs e)
        {
            if( e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }

            PSDataCollection<object> input = new PSDataCollection<object>();
            foreach (var ci in e.NewItems)
            {
                input.Add(ci);
            } 
            
            Interlocked.Exchange( ref _currentInput, input );
            RaiseTrigger();
        }

        private void RaiseTrigger()
        {
            var mt = Trigger as ManualTrigger;
            
            if (null == mt)
            {
                mt = new ManualTrigger();
                Trigger = mt;
            }

            mt.RaiseTrigger();           
        }

        private void CreateScriptBlock()
        {
            var acrossScript = CreateSelectExpandScript(_across);
            var byScript = CreateGetFromByItemScript(_byItem);
            ScriptBlock = 
                String.Format(
                    "$input | foreach-object {{ $_.{0} }} | {1}",
                    acrossScript,
                    byScript
                    );
        }

        private string CreateGetFromByItemScript(object byItem)
        {
            var byMember = byItem as PSScriptProperty;
            if( null != byMember )
            {
                return byMember.GetterScript.ToString();
            }

            return "foreach-object {$_}";
        }

        private string CreateSelectExpandScript(object across)
        {
            var acrossMember = across as PSMemberInfo;
            if( null != acrossMember )
            {
                _rootDataSource.AddDynamicMember( acrossMember );
                return acrossMember.Name;
            }

            return null;
        }

        protected override PSDataCollection<object> CurrentScriptBlockInput
        {
            get
            {               
                return _currentInput;
            }
        }
    }
}
