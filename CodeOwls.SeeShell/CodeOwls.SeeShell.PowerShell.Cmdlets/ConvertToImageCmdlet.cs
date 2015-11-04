using System.Drawing.Imaging;
using System.Management.Automation;

namespace CodeOwls.SeeShell.PowerShell.Cmdlets
{
    [Cmdlet(VerbsData.ConvertTo, "Image", ConfirmImpact = ConfirmImpact.None)]
    public class ConvertToImageCmdlet : ViewCmdletBase
    {
        [Parameter(Mandatory = false)]
        public ImageFormat Format { get; set; }

        public ConvertToImageCmdlet()
        {
            Format = ImageFormat.Png;
        }

        protected override void ProcessRecord()
        {
            var bitmap = ConvertViewToBitmap();

            if(! bitmap.RawFormat.Equals( Format ) )
            {
                bitmap = ConvertBitmapToFormat(bitmap, Format);
            }

            WriteObject( bitmap );
        }

    }
}