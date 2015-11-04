using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.SeeShell.Common.Triggers;

namespace CodeOwls.SeeShell.PowerShell.Providers.Triggers
{
    class TriggerNodeFactory : PathNodeBase, IRemoveItem
    {
        private readonly TriggerDrive _drive;
        private readonly ITrigger _trigger;

        public TriggerNodeFactory(TriggerDrive drive, ITrigger trigger)
        {
            _drive = drive;
            _trigger = trigger;
        }

        public override IPathValue GetNodeValue()
        {
            return new PathValue( _trigger, Name, false );
        }

        public override string Name
        {
            get { return _trigger.Name; }
        }

        public object RemoveItemParameters
        {
            get { return null; }
        }

        public void RemoveItem(IProviderContext context, string path, bool recurse)
        {
            _drive.Remove( _trigger );
        }
    }
}