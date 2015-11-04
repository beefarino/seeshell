using System.Linq;
using System.Management.Automation;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.SeeShell.Common;

namespace CodeOwls.SeeShell.PowerShell.Providers.DataSources
{
    class ScaleAssignmentNodeFactory : PathNodeBase
    {
        private readonly IScaleDescriptorAssignment _scale;

        public ScaleAssignmentNodeFactory(IScaleDescriptorAssignment scale)
        {
            _scale = scale;
        }

        public override System.Collections.Generic.IEnumerable<IPathNode> GetNodeChildren(CodeOwls.PowerShell.Provider.PathNodeProcessors.IProviderContext context)
        {
            var children = _scale.Scale.Ranges;
            foreach( var child in children )
            {
                yield return new RangeNodeFactory(_scale.Scale, child);
            }
        }

        public override IPathValue GetNodeValue()
        {
            return new PathValue(_scale, Name, true);
        }

        public override string Name
        {
            get
            {
                return _scale.PropertyName;
            }
        }
    }
}