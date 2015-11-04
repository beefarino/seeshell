using CodeOwls.SeeShell.Common.Triggers;
using Xunit;

namespace CodeOwls.SeeShell.Common.Tests
{
    public class ImmediateTriggerTests
    {
        [Fact]
        public void RaisesTriggerOnEventAttach()
        {
            ImmediateTrigger trigger = new ImmediateTrigger();
            int count = 0;
            trigger.Trigger += (s, a) => ++count;
            
            Assert.Equal(1, count);
        }
    }
}