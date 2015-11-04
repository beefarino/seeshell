using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using CodeOwls.PowerShell.Provider;
using CodeOwls.SeeShell.Common.Attributes;
using CodeOwls.SeeShell.Common.ViewModels;
using CodeOwls.SeeShell.Common.ViewModels.Grids;
using CodeOwls.SeeShell.PowerShell.Providers.Charts;

namespace CodeOwls.SeeShell.PowerShell.Providers
{    
    public abstract class Provider<T> : Provider where T : ViewModelBase, new()
    {
        protected override System.Collections.ObjectModel.Collection<System.Management.Automation.PSDriveInfo>
            InitializeDefaultDrives()
        {
            var type = typeof (T);

            var attr = (from DriveInfoAttribute a in type.GetCustomAttributes(typeof (DriveInfoAttribute), true)
                        select a
                       ).FirstOrDefault();
            if (null == attr)
            {
                throw new InvalidOperationException("DriveInfoAttribute is not available on the specified model");
            }

            var drive =
                new Drive<T>(new PSDriveInfo(attr.DriveName, this.ProviderInfo, attr.ProviderDriveRoot,
                                             attr.ProviderDescription, null));
            return new Collection<PSDriveInfo> {drive};
        }

        //[Licensed]
        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            var d = drive as Drive<T>;
            if (null != d)
            {
                return d;
            }

            return new Drive<T>(drive);
        }

        protected Drive<T> Drive
        {
            get
            {
                Drive<T> drive = this.PSDriveInfo as Drive<T>;
                if (null == drive)
                {
                    drive = this.ProviderInfo.Drives.FirstOrDefault() as Drive<T>;
                }

                return drive;
            }
        }

        protected internal virtual RootNodeFactory<T> RootNodeFactory
        {
            get { return new RootNodeFactory<T>(Drive, Drive.RootNodeName); }
        }
        protected override CodeOwls.PowerShell.Paths.Processors.IPathResolver PathResolver
        {
            get
            {
                return new PathResolver( RootNodeFactory );
            }
        }
    }
}