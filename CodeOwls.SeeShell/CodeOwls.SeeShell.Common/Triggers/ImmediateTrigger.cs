using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeOwls.SeeShell.Common.Triggers
{
    public class ImmediateTrigger : ITrigger
    {
        public void Dispose()
        {
            
        }

        public string Name { get; set; }

        public event EventHandler Trigger
        {
            add
            {
                if( null == value )
                {
                    return;
                }
                value.Invoke( this, EventArgs.Empty );
            }
            remove
            {                
            }
        }
    }
}
