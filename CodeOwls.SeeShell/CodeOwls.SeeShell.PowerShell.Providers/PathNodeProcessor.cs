using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.SeeShell.PowerShell.Providers
{
    public class PathResolver : PathResolverBase
    {
        private readonly IPathNode _root;

        public PathResolver(IPathNode root)
        {
            _root = root;
        }

        protected override IPathNode Root
        {
            get { return _root; }
        }
    }
}
