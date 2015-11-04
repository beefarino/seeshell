using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Windows;
using CodeOwls.SeeShell.Common.Attributes;
using CodeOwls.SeeShell.Common.Providers;
using CodeOwls.SeeShell.Common.Utility;

namespace CodeOwls.SeeShell.PowerShell.Cmdlets
{
    public abstract class ViewCmdletBase : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true, Mandatory = true)]
        [PathArgumentTransformation]
        public PSObject InputObject { get; set; }

        protected ImageFormat GetFormatFromFilePath(ref string fullPath)
        {
            var ext = Path.GetExtension(fullPath) ?? String.Empty;

            var map = new Dictionary<string, ImageFormat>
            {
                { ".png", ImageFormat.Png },
                { ".bmp", ImageFormat.Bmp },
                { ".emf", ImageFormat.Emf },
                { ".exif", ImageFormat.Exif },
                { ".xif", ImageFormat.Exif },
                { ".gif", ImageFormat.Gif },
                { ".ico", ImageFormat.Icon },
                { ".jpg", ImageFormat.Jpeg },
                { ".jpeg", ImageFormat.Jpeg },
                { ".tiff", ImageFormat.Tiff },
                { ".wmf", ImageFormat.Wmf },
            };

            ImageFormat format;
            if (!map.TryGetValue(ext.ToLowerInvariant(), out format))
            {
                fullPath += ".png";
                return ImageFormat.Png;
            }
            return format;
        }

        protected FrameworkElement GetViewFromInputObject()
        {
            var drive = (from prop in InputObject.Properties
                where prop.Name == "PSDrive"
                select prop.Value).FirstOrDefault();

            var viewDrive = drive as IViewMap;
            if (viewDrive == null)
            {
                return null;
            }

            var view = viewDrive.GetViewForItem(InputObject.BaseObject);
            return view as FrameworkElement;
        }

        protected Bitmap ConvertViewToBitmap()
        {
            var view = GetViewFromInputObject();
            var exporter = new VisualImageExporter();
            Bitmap bitmap = null;
            using (var ms = new MemoryStream())
            {
                view.Dispatcher.Invoke((Action)(() => exporter.Export(view, ms)));
                ms.Position = 0;
                bitmap = new Bitmap(ms);
            }

            return bitmap;
        }

        protected Bitmap ConvertBitmapToFormat(Bitmap bmp, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
                bmp.Save( ms, format );
                ms.Position = 0;
                var bitmap = new Bitmap(ms);
                return bitmap;
            }
        }
    }
}