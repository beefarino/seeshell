using System;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;
using Xunit;

namespace CodeOwls.SeeShell.Providers.Tests
{
    public class ProviderTestsBase
    {
        protected System.Management.Automation.PowerShell Create()
        {           
            var powershell = System.Management.Automation.PowerShell.Create();
            powershell.Runspace = RunspaceFactory.CreateRunspace(new Host() );
            powershell.Runspace.ApartmentState = ApartmentState.STA;
            
            powershell.Runspace.Open();
            powershell.AddScript(String.Format("cd '{0}'; ls ./codeowls.seeshell.powershell.*.dll | import-module;", Environment.CurrentDirectory));
            return powershell;
        }

        protected T Execute<T>( string script, params object[] args )
        {
            using (var ps = Create())
            {
                if (null != args && args.Any())
                {
                    script = String.Format(script, args);
                }

                ps.AddScript(script);
                var results = ps.Invoke();
                //if( ps.Streams.Debug.Any())
                //{
                //    var b = new StringBuilder();
                //    ps.Streams.Debug.ToList().ForEach(d=>b.AppendLine(d.Message));
                //    var debug = b.ToString();
                //}
                if (ps.Streams.Error.Any())
                {
                    Assert.False(ps.Streams.Error.Any(),
                                 "script {" + script + "} raised errors: " + ps.Streams.Error[0].Exception.ToString());
                }

                if (!results.Any())
                {
                    return default(T);
                }

                return (T) results.First().BaseObject;
            }
        }
    }
}