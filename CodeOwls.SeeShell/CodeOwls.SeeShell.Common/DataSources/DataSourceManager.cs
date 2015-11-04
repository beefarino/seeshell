using System.Collections.Generic;

namespace CodeOwls.SeeShell.Common
{
    public static class DataSourceManager
    {
        static readonly List<IPowerShellDataSource> DataSourceList = new List<IPowerShellDataSource>();
        public static IEnumerable<IPowerShellDataSource>  DataSources
        {
            get { return DataSourceList.ToArray(); }
        }

        public static void Add( IPowerShellDataSource dataSource )
        {
            DataSourceList.Add( dataSource);
        }

        public static void Remove(IPowerShellDataSource powerShellDataSource)
        {
            DataSourceList.Remove(powerShellDataSource);
        }
    }
}