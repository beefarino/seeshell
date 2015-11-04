using System;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Web;
using CodeOwls.SeeShell.Common.Support;

namespace CodeOwls.SeeShell.PowerShell.Cmdlets
{
    [Cmdlet(VerbsCommunications.Send, "SeeShellIncident", ConfirmImpact = ConfirmImpact.None)]
    public class SendBugReportCmdlet : PSCmdlet
    {
        [Parameter()]
        public string Message { get; set; }

        [Parameter()]
        public SwitchParameter IncludeCommandHistory { get; set; }

        [Parameter()]
        public SwitchParameter IncludeErrorHistory { get; set; }

        public SendBugReportCmdlet()
        {
            IncludeCommandHistory = SwitchParameter.Present;
            IncludeErrorHistory = SwitchParameter.Present;
        }

        protected override void EndProcessing()
        {                        
            string history = "user opt-out";
            string errors = "user opt-out";

            if( IncludeCommandHistory.IsPresent )
            {
                var o = this.SessionState.InvokeCommand.InvokeScript("get-history | convertto-xml | select -expand outerxml").FirstOrDefault();
                if( null == o)
                {
                    history = "unable to retrieve history";
                }
                else
                {
                    history = o.ToString();
                }
            }
            if (IncludeErrorHistory.IsPresent)
            {
                var o = this.SessionState.InvokeCommand.InvokeScript("$error | convertto-xml | select -expand outerxml").FirstOrDefault();
                if (null == o)
                {
                    errors = "unable to retrieve errors";
                }
                else
                {
                    errors = o.ToString();
                }
            }

            var message =
                String.Format(
                    "This issue was submitted via the Send-SeeShellIncident cmdlet.\nUser Notes: {0}",
                    Message);
            IssueReporter.ReportIssue( errors, history, message );
        }
    }
}