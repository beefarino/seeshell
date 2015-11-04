using System.ComponentModel;
using System.Drawing.Imaging;
using System.Management.Automation;
using System.Text;
using CodeOwls.SeeShell.Common.ViewModels;

namespace CodeOwls.SeeShell.PowerShell.Cmdlets
{
    [Cmdlet(VerbsData.Export, "Image", ConfirmImpact = ConfirmImpact.None)]
    public class ExportImageCmdlet : ViewCmdletBase
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        [Alias("OutputPath")]
        public string FilePath { get; set; }

        protected override void ProcessRecord()
        {
            var fullPath = this.GetUnresolvedProviderPathFromPSPath(FilePath);
            ImageFormat format = GetFormatFromFilePath(ref fullPath);

            var bitmap = ConvertViewToBitmap(); 
            bitmap.Save(fullPath, format);

            var item = this.InvokeProvider.Item.Get(fullPath);
            WriteObject(item);
        }
    }
}
