using System;
using System.Globalization;
using System.Management.Automation.Host;
using System.Threading;

namespace CodeOwls.SeeShell.Providers.Tests
{
    public class Host : System.Management.Automation.Host.PSHost
    {
        public override void SetShouldExit(int exitCode)
        {
            
        }

        public override void EnterNestedPrompt()
        {
            
        }

        public override void ExitNestedPrompt()
        {
            
        }

        public override void NotifyBeginApplication()
        {
            
        }

        public override void NotifyEndApplication()
        {
            
        }

        public override string Name
        {
            get { return "NullHost"; }
        }

        public override Version Version
        {
            get { return new Version(1,0); }
        }

        public override Guid InstanceId
        {
            get { return Guid.NewGuid();  }
        }

        public override PSHostUserInterface UI
        {
            get { return new HostUI(); }
        }

        public override CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }

        public override CultureInfo CurrentUICulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
        }
    }
}