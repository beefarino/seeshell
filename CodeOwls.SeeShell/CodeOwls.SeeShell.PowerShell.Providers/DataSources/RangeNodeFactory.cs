using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.SeeShell.Common;

namespace CodeOwls.SeeShell.PowerShell.Providers.DataSources
{
    class RangeNodeFactory : PathNodeBase, IRemoveItem
    {
        private readonly IScaleDescriptor _owner;
        private readonly IRangeDescriptor _range;

        public RangeNodeFactory( IScaleDescriptor owner, IRangeDescriptor range )
        {
            _owner = owner;
            _range = range;
        }

        public override IPathValue GetNodeValue()
        {
            return new PathValue ( _range, Name, false );
        }

        public override string Name
        {
            get { return _range.ToString(); }
        }

        public object RemoveItemParameters
        {
            get { return null; }
        }

        public void RemoveItem(IProviderContext context, string path, bool recurse)
        {
            _owner.Ranges.Remove(_range);
        }
    }
}