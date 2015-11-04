using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace CodeOwls.SeeShell.PowerShell.Providers.Triggers
{
    public class TriggerNewItemParameters
    {
        [Parameter]
        [TimeSpanArgumentTransformation]
        public TimeSpan? Interval { get; set; }

        [Parameter]
        public SwitchParameter Manual { get; set; }
    }
}
