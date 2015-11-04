using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CodeOwls.SeeShell.Common.Triggers;
using Xunit;

namespace CodeOwls.SeeShell.Common.Tests
{
    public class IntervalTriggerTests
    {
        [Fact]
        public void RaisesTriggerOnSetInterval()
        {
            IntervalTrigger trigger = new IntervalTrigger();
            trigger.Interval = TimeSpan.FromMilliseconds(100);
            int count = 0;
            trigger.Trigger += (s, a) => ++count;
            Thread.Sleep( 200 );
            Assert.NotEqual( 0, count );
        }
    }
}
