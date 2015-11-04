using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading;

namespace CodeOwls.SeeShell.Common.DataSources
{
    static class DefaultRunspaceManager
    {
        class RunspaceScope : IDisposable
        {
            private readonly Runspace _previousDefaultRunspace;

            public RunspaceScope(Runspace runspace)
            {
                _previousDefaultRunspace = Runspace.DefaultRunspace;
                Runspace.DefaultRunspace = runspace;
            }

            public void Dispose()
            {
                Runspace.DefaultRunspace = _previousDefaultRunspace;
            }
        }

        static readonly Dictionary< int, Runspace > Runspaces = new Dictionary<int, Runspace>();
        
        public static IDisposable ForCurrentThread
        {
            get
            {
                var id = Thread.CurrentThread.ManagedThreadId;
                Runspace runspace = null;
                if (! Runspaces.TryGetValue(id, out runspace))
                {
                    runspace = RunspaceFactory.CreateRunspace();
                    runspace.Open();
                    Runspaces.Add(id, runspace);
                }

                return new RunspaceScope(runspace);
            }
        }
    }
}
