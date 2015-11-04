using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.SeeShell.Common.Triggers;

namespace CodeOwls.SeeShell.PowerShell.Providers.Triggers
{
    class TriggerRootNodeFactory : PathNodeBase, INewItem
    {
        private readonly TriggerDrive _drive;

        public TriggerRootNodeFactory(TriggerDrive drive)
        {
            _drive = drive;
        }

        public override IEnumerable<IPathNode> GetNodeChildren(IProviderContext context)
        {
            foreach( var trigger in _drive.Triggers )
            {
                yield return new TriggerNodeFactory( _drive, trigger );
            }
        }

        public override IEnumerable<IPathNode> Resolve(IProviderContext context, string nodeName)
        {
            var trigger =
                (from t in _drive.Triggers
                 where StringComparer.InvariantCultureIgnoreCase.Equals(nodeName, t.Name)
                 select t).FirstOrDefault();
            if( null == trigger)
            {
                return null;
            }
            return new [] { new TriggerNodeFactory( _drive, trigger ) };
        }

        public override IPathValue GetNodeValue()
        {
            return new PathValue(_drive, Name, true);
        }

        public override string Name
        {
            get { return "Triggers"; }
        }

        public IEnumerable<string> NewItemTypeNames
        {
            get { return null; }
        }

        public object NewItemParameters
        {
            get { return new TriggerNewItemParameters(); }
        }

        public IPathValue NewItem(IProviderContext context, string path, string itemTypeName, object newItemValue)
        {
            ITrigger trigger = null;
            var parameter = context.DynamicParameters as TriggerNewItemParameters;
            if (null != parameter)
            {
                if (parameter.Interval.HasValue)
                {
                    trigger = new IntervalTrigger {Name = path, Interval = parameter.Interval.Value};
                }
                else if (parameter.Manual)
                {
                    trigger = new ManualTrigger {Name = path};
                }
            }

            if( null == trigger )
            {
                trigger = new ImmediateTrigger { Name = path };
            }

            _drive.Add(trigger);

            return Resolve(context, path).First().GetNodeValue();
        }
    }
}
