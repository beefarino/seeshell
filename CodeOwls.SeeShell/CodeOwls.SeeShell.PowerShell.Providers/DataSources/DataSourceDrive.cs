using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using CodeOwls.PowerShell.Provider;
using CodeOwls.SeeShell.Common;
using CodeOwls.SeeShell.Common.Providers;

namespace CodeOwls.SeeShell.PowerShell.Providers.DataSources
{
    public class DataSourceDrive : Drive, IDriveOf<IPowerShellDataSource>
    {
        public DataSourceDrive(PSDriveInfo driveInfo) : base(driveInfo)
        {
            _dataSources = new List<IPowerShellDataSource>();
        }
        
        private readonly List<IPowerShellDataSource> _dataSources;

        public void Add(IPowerShellDataSource dataSource, bool show)
        {
            throw new NotImplementedException();
        }
        public void Add( IPowerShellDataSource dataSource )
        {
            _dataSources.Add( dataSource );
        }

        public void Remove(IPowerShellDataSource dataSource)
        {
            if (_dataSources.Contains(dataSource))
            {
                _dataSources.Remove(dataSource);
            }
        }

        public IEnumerable<IPowerShellDataSource> Items
        {
            get { return _dataSources; }
        }

        public IEnumerable<IPowerShellDataSource> DataSources
        {
            get { return _dataSources; }
        }
    }
}
