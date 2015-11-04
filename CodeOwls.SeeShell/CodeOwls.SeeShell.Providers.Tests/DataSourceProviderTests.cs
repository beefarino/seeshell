using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using CodeOwls.SeeShell.Common;
using CodeOwls.SeeShell.Common.Triggers;
using CodeOwls.SeeShell.PowerShell.Providers;
using CodeOwls.SeeShell.PowerShell.Providers.DataSources;
using Xunit;

namespace CodeOwls.SeeShell.Providers.Tests
{
    public class DataSourceProviderTests : ProviderTestsBase
    {
        [Fact]
        public void ProviderCanBeLoaded()
        {
            bool isLoaded = Execute<bool>("[bool] ( get-psprovider -ps '{0}' )", ProviderNames.DataSourceProvider);
            Assert.True(isLoaded, "the data source provider is not loaded");
        }

        [Fact]
        public void LoadingProviderCreatesDataSourcesDrive()
        {
            object drive = Execute<object>("get-psdrive DataSources");
            Assert.NotNull(drive);
        }

        [Fact]
        public void SupportsNewItem()
        {
            var s = @"new-item datasources:/test1 -value { @(1,2,3,4) }";
            PowerShellDataSource item = Execute<PowerShellDataSource>(s);
            Assert.NotNull(item);
        }

        [Fact]
        public void SupportsRemoveItem()
        {
            var s = @"
                $t = new-item datasources:/trgr -value {@(1,2,3,4)}; 
                $r = test-path datasources:/trgr; 
                $t | remove-item | out-null; 
                [bool]($r -and -not( test-path datasources:/trgr))";
            var result = ExecuteDebug<bool>(s);
            Assert.True(result, "Failed to remove data source");
        }

        protected T ExecuteDebug<T>(string s, params object[] args )
        {
            return Execute<T>("$debugpreference='continue'; " + s, args );
        }

        [Fact]
        public void DataSourceExecutesScriptOnImmediateTriggerAttach()
        {
            PowerShellDataSource item = new PowerShellDataSource {Name = "Item", ScriptBlock = "@(0,1,2,3)"};
            Assert.NotNull(item);
            Assert.Equal(0, item.Data.Count);
            item.Trigger = new ImmediateTrigger();
            item.ScriptComplete.WaitOne(500);
            Assert.Equal(4, item.Data.Count);
        }

        [Fact]
        public void DataSourceExecutesScriptOnManualTriggerPull()
        {
            PowerShellDataSource item = new PowerShellDataSource { Name = "Item", ScriptBlock = "@(0,1,2,3)" };
            Assert.NotNull(item);
            Assert.Equal(0, item.Data.Count);
            var trigger = new ManualTrigger();
            item.Trigger = trigger;
            Assert.Equal(0, item.Data.Count);
            trigger.RaiseTrigger();
            item.ScriptComplete.WaitOne(500);
            Assert.Equal(4, item.Data.Count);
        }

        [Fact]
        public void CanCreateNewDrives()
        {
            var s = @"new-psdrive -psp {0} -name dataSources2 -root ''";
            PSDriveInfo drive = Execute<PSDriveInfo>(s, ProviderNames.DataSourceProvider);
            Assert.NotNull(drive);
            Assert.IsType<DataSourceDrive>(drive);
            Assert.Equal("dataSources2", drive.Name);
        }
        
        [Fact]
        public void CanAssignTriggerByPath()
        {
            var s = @"new-item triggers:/trigger1 -manual | out-null; new-item datasources:/test1 -trigger triggers:/trigger1 -value { get-random }";
            PowerShellDataSource item = Execute<PowerShellDataSource>(s);
            Assert.NotNull(item);
            Assert.NotNull(item.Trigger);
            Assert.Equal("trigger1", item.Trigger.Name);
            Assert.Equal(0, item.Data.Count);
            Assert.IsType<ManualTrigger>(item.Trigger);
        }

        [Fact]
        public void CanAssignTriggerByObject()
        {
            var s = @"$trigger = new-item triggers:/trigger0 -manual; new-item datasources:/test1 -trigger $trigger -value { get-random }";
            PowerShellDataSource item = Execute<PowerShellDataSource>(s);
            Assert.NotNull(item);
            Assert.NotNull(item.Trigger);
            Assert.Equal("trigger0", item.Trigger.Name);
            Assert.Equal(0, item.Data.Count);
            Assert.IsType<ManualTrigger>(item.Trigger);
        }

        [Fact]
        public void DefaultTriggerForNewItemIsImmediate()
        {
            var s = @"new-item datasources:/test1 -value { @(1,2,3,4) }";
            PowerShellDataSource item = Execute<PowerShellDataSource>(s);
            Assert.NotNull(item);
            Assert.IsType<ImmediateTrigger>(item.Trigger);
        }

    }
}
