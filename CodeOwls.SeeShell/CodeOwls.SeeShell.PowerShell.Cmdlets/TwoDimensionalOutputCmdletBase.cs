using System.Management.Automation;
using CodeOwls.SeeShell.Common;
using CodeOwls.SeeShell.Common.Attributes;

namespace CodeOwls.SeeShell.PowerShell.Cmdlets
{
    public abstract class TwoDimensionalOutputCmdletBase<TSeriesViewModel>: OneDimensionalOutputCmdletBase< TSeriesViewModel >
    {
        [Parameter(Mandatory = true)]
        [ScriptBlockDynamicPropertyArgumentTransformation]        
        [Alias("Against")]
        public object By { get; set; }

        protected override void ProcessRecord( IPowerShellDataSource dataSource )
        {
            ProcessRecord( dataSource, Plot, By, Across, KeyOn);
        }
    }
}