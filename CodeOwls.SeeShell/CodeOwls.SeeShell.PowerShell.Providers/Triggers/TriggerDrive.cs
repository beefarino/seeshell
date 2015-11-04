using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using CodeOwls.PowerShell.Provider;
using CodeOwls.SeeShell.Common.Triggers;

namespace CodeOwls.SeeShell.PowerShell.Providers.Triggers
{
    public class TriggerDrive : Drive
    {
        private List<ITrigger> _triggers;
 
        public TriggerDrive(PSDriveInfo driveInfo) : base(driveInfo)
        {
            _triggers = new List<ITrigger>();
        }

        public IEnumerable<ITrigger> Triggers
        {
            get { return _triggers; }
        }

        public void Add(ITrigger trigger)
        {
            _triggers.Add(trigger);
        }
        public void Remove(ITrigger trigger)
        {
            _triggers.Remove(trigger);
        }
    }
}
