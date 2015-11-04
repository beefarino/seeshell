using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using CodeOwls.SeeShell.Common.Attributes;
using CodeOwls.SeeShell.Common.Triggers;

namespace CodeOwls.SeeShell.PowerShell.Providers.DataSources
{
    public class DataSourceNewItemParameters
    {
        [Parameter]
        [PathArgumentTransformation]
        public ITrigger Trigger { get; set; }

        [Parameter]
        [Alias("Size", "SampleSize")]
        public int? MaxItemCount { get; set; }

        [Parameter]
        public Hashtable Scales { get; set; }

        [Parameter]
        public SwitchParameter NoDispatcher { get; set; }

        [Parameter(ValueFromRemainingArguments = true)]
        public object[] Args { get; set; }
    }
}
