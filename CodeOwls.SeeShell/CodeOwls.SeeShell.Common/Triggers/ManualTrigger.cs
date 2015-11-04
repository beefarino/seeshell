using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeOwls.SeeShell.Common.Triggers
{
    public class ManualTrigger : ITrigger
    {
        public void Dispose()
        {            
        }

        public void RaiseTrigger()
        {
            var ev = Trigger;
            if( null == ev )
            {
                return;
            }

            ev(this, EventArgs.Empty);
        }

        public string Name { get; set; }
        public event EventHandler Trigger;
    }
}
