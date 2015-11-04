using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Text;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider;

namespace CodeOwls.SeeShell.PowerShell.Providers.DataSources
{
    [CmdletProvider( ProviderNames.DataSourceProvider, ProviderCapabilities.ShouldProcess )]
    public class DataSourceProvider : CodeOwls.PowerShell.Provider.Provider
    {
        protected override System.Collections.ObjectModel.Collection<System.Management.Automation.PSDriveInfo> InitializeDefaultDrives()
        {
            var drive =
                new DataSourceDrive(new PSDriveInfo("DataSources", this.ProviderInfo, "",
                                                    "SeeShell Data Source Repository", null));
            return new Collection<PSDriveInfo> { drive };
        }

        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            return new DataSourceDrive( drive );
        }

        protected override PSDriveInfo RemoveDrive(PSDriveInfo drive)
        {
            var ds = drive as DataSourceDrive;
            if( null != ds)
            {
                var sources = ds.DataSources.ToList();
                sources.ForEach(d=>d.Dispose());
                sources.ForEach( ds.Remove );
            }

            return base.RemoveDrive(drive);
        }

        DataSourceDrive Drive
        {
            get
            {
                DataSourceDrive drive = this.PSDriveInfo as DataSourceDrive;
                if (null == drive)
                {
                    drive = this.ProviderInfo.Drives.FirstOrDefault() as DataSourceDrive;
                }

                return drive;
            }
        }

        protected override IPathResolver PathResolver
        {
            get { return new DataSourcePathNodeProcessor( Drive ); }
        }
    }
}
