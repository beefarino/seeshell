using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.SeeShell.PowerShell.Providers.Triggers
{
    class TriggerPathNodeProcessor : PathResolverBase
    {
        private readonly TriggerDrive _drive;

        public TriggerPathNodeProcessor( TriggerDrive drive )
        {
            _drive = drive;
        }

        protected override IPathNode Root
        {
            get { return new TriggerRootNodeFactory(_drive); }
        }
    }
}
