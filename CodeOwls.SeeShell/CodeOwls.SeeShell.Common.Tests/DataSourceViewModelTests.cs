using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeOwls.SeeShell.Common.Triggers;
using CodeOwls.SeeShell.Common.ViewModels;
using Xunit;

namespace CodeOwls.SeeShell.Common.Tests
{
    public class DataSourceViewModelTests
    {
        class ViewModel : SingleDataSourceViewModelBase
        {
        }

        [Fact]
        public void AllRecordsCountUpdatedInSingleDataSourceViewModel()
        {
            var vm = new ViewModel();
            var dataSource = new PowerShellDataSource
            {
                ScriptBlock = "1 | select -exp ASDF"
            };

            vm.DataSource = dataSource;
            Assert.Equal(0, vm.AllRecordsCount);
            Assert.Equal(dataSource.AllRecords.Count, vm.AllRecordsCount);

            vm.DataSource.Trigger = new ImmediateTrigger();
            dataSource.ScriptComplete.WaitOne();

            Assert.Equal(1, vm.AllRecordsCount);
            Assert.Equal(dataSource.AllRecords.Count, vm.AllRecordsCount);
        }
        
        [Fact]
        public void AllRecordsCountPropertyChangeNotifiedByViewModel()
        {
            var vm = new ViewModel();
            int called = 0;
            vm.PropertyChanged += (s, a) => { if (a.PropertyName == "AllRecordsCount") ++called;  };
            var dataSource = new PowerShellDataSource
                                {
                                    ScriptBlock = "1 | select -exp ASDF"
                                };

            vm.DataSource = dataSource; 
            Assert.Equal(0, vm.AllRecordsCount);
            
            called = 0;
            vm.DataSource.Trigger = new ImmediateTrigger(); 
            dataSource.ScriptComplete.WaitOne();

            Assert.Equal(1, vm.AllRecordsCount);
            Assert.True( 1 == called, "Failed to raise property notification change event for AllRecordsCount");
        }
    }
}
