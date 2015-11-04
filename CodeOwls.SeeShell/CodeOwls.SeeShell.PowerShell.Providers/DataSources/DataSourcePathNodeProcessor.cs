using System;
using System.Collections.Generic;
using System.Text;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.SeeShell.Common;

namespace CodeOwls.SeeShell.PowerShell.Providers.DataSources
{
    class DataSourcePathNodeProcessor : CodeOwls.PowerShell.Paths.Processors.PathResolverBase
    {
        private readonly DataSourceDrive _dataSources;

        public DataSourcePathNodeProcessor(DataSourceDrive dataSources)
        {
            _dataSources = dataSources;
        }

        protected override IPathNode Root
        {
            get { return new DataSourceRootNodeFactory( _dataSources ); }
        }
    }
}
