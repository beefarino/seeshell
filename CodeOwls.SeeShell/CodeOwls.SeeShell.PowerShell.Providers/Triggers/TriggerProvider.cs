using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Text;
using CodeOwls.PowerShell.Paths.Processors;

namespace CodeOwls.SeeShell.PowerShell.Providers.Triggers
{
    //[CmdletProvider(ProviderNames.TriggerProvider, ProviderCapabilities.ShouldProcess)]
    public class TriggerProvider : CodeOwls.PowerShell.Provider.Provider
    {
        protected override System.Collections.ObjectModel.Collection<System.Management.Automation.PSDriveInfo> InitializeDefaultDrives()
        {
            var driveInfo = new PSDriveInfo(
                "Triggers",
                ProviderInfo,
                "",
                "Data Source Triggers",
                null
            );
            var drive = new TriggerDrive(driveInfo);
            return new Collection<PSDriveInfo> { drive };
        }

        protected override IPathResolver PathResolver
        {
            get { return new TriggerPathNodeProcessor( Drive );}
        }

        TriggerDrive Drive
        {
            get
            {
                var drive = this.PSDriveInfo as TriggerDrive;
                if (null == drive)
                {
                    drive = this.ProviderInfo.Drives.FirstOrDefault() as TriggerDrive;
                }

                return drive;
            }
        }
    }
}
