using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.SeeShell.Common.ViewModels.Charts;

namespace CodeOwls.SeeShell.PowerShell.Providers.Charts
{
    public class ChartRootNodeFactory : RootNodeFactory<ChartViewModel>
    {
        public ChartRootNodeFactory( Drive<ChartViewModel> drive ) : base( drive, "Charts")
        {        
        }

        public override IEnumerable<IPathNode> GetNodeChildren(IProviderContext context)
        {
            foreach( var item in Drive.Items )
            {
                yield return new ChartNodeFactory( item );
            }
        }
    }
}
