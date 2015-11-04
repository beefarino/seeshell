using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeOwls.SeeShell.Common.Triggers;
using CodeOwls.SeeShell.PowerShell.Providers;
using Xunit;

namespace CodeOwls.SeeShell.Providers.Tests
{
    public class TriggerProviderTests : ProviderTestsBase
    {
        [Fact]
        public void CanLoadProvider()
        {
            var result = Execute<bool>("[bool](get-psprovider -psp '{0}')", ProviderNames.TriggerProvider);
            Assert.True( result, "Failed to load trigger provider");
        }
        
        [Fact]
        public void CanCreateNewImmediateTrigger()
        {
            var result = Execute<ITrigger>("new-item triggers:\\myTrigger");
            Assert.IsType<ImmediateTrigger>(result);
        }

        [Fact]
        public void CanCreateNewManualTrigger()
        {
            var result = Execute<ITrigger>("new-item triggers:\\myTrigger -manual");
            Assert.IsType<ManualTrigger>(result);
        }
 
        [Fact]
        public void CanCreateNewIntervalTrigger()
        {
            var result = Execute<ITrigger>("new-item triggers:\\myTrigger -interval ([timespan]::fromseconds(5))");
            Assert.IsType<IntervalTrigger>(result);
        }

        [Fact]
        public void CanCreateNewIntervalTriggerUsingMSConverter()
        {
            var result = Execute<ITrigger>("new-item triggers:\\myTrigger -interval 500ms");
            Assert.IsType<IntervalTrigger>(result);
            var trigger = result as IntervalTrigger;
            Assert.Equal( 500, trigger.Interval.TotalMilliseconds );
        }

        [Fact]
        public void CanCreateNewIntervalTriggerUsingSConverter()
        {
            var result = Execute<ITrigger>("new-item triggers:\\myTrigger -interval 0.5s");
            Assert.IsType<IntervalTrigger>(result);
            var trigger = result as IntervalTrigger;
            Assert.Equal(500, trigger.Interval.TotalMilliseconds);
        }

        [Fact]
        public void CanCreateNewIntervalTriggerUsingMConverter()
        {
            var result = Execute<ITrigger>("new-item triggers:\\myTrigger -interval 0.5min");
            Assert.IsType<IntervalTrigger>(result);
            var trigger = result as IntervalTrigger;
            Assert.Equal(30, trigger.Interval.TotalSeconds);
        }

        [Fact]
        public void CanRemoveTrigger()
        {
            var result = Execute<bool>(@"
                $t = new-item triggers:/trgr; 
                $r = test-path triggers:/trgr; 
                $t | remove-item | out-null; 
                [bool]($r -and -not( test-path triggers:/trgr))");
            Assert.True( result, "Failed to remove existing trigger");
        }
    }
}
