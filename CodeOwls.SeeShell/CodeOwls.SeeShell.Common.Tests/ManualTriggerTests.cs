using CodeOwls.SeeShell.Common.Triggers;
using Xunit;

namespace CodeOwls.SeeShell.Common.Tests
{
    public class ManualTriggerTests
    {
        [Fact]
        public void RaisesTriggerOnDemand()
        {
            var trigger = new ManualTrigger();
            int count = 0;
            trigger.Trigger += (s, a) => ++count;

            Assert.Equal(0, count);
            trigger.RaiseTrigger();
            Assert.Equal(1, count);
            trigger.RaiseTrigger();
            Assert.Equal(2, count);
        }
    }
}