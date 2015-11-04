using System.Collections.Generic;
using System.Management.Automation.Provider;
using System.Text;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.SeeShell.Common.Charts;
using CodeOwls.SeeShell.Common.ViewModels.Charts;

namespace CodeOwls.SeeShell.PowerShell.Providers.Charts
{
    [CmdletProvider(ProviderNames.ChartProvider, ProviderCapabilities.ShouldProcess)]
    public class ChartProvider : Provider<ChartViewModel>
    {
        protected override IPathResolver PathResolver
        {
            get { return new PathResolver( new ChartRootNodeFactory( this.Drive )); }
        }
    }
}
