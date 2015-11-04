using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;

namespace CodeOwls.SeeShell.Providers.Tests
{
    public class HostUI : PSHostUserInterface
    {
        public override string ReadLine()
        {
            return String.Empty;
        }

        public override SecureString ReadLineAsSecureString()
        {
            return new SecureString();
        }

        public override void Write(string value)
        {
            
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            
        }

        public override void WriteLine(string value)
        {
            
        }

        public override void WriteErrorLine(string value)
        {
            
        }

        public override void WriteDebugLine(string message)
        {
            
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
            
        }

        public override void WriteVerboseLine(string message)
        {
            
        }

        public override void WriteWarningLine(string message)
        {
            
        }

        public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
        {
            return new Dictionary<string, PSObject>();   
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            return new PSCredential( "asdf", new SecureString() );
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
            return new PSCredential("asdf", new SecureString());
        }

        public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
        {
            return defaultChoice;
        }

        public override PSHostRawUserInterface RawUI
        {
            get { return null; }
        }
    }
}