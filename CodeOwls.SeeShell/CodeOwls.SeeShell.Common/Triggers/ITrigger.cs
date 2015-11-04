using System;

namespace CodeOwls.SeeShell.Common.Triggers
{
    public interface ITrigger : IDisposable
    {
        event EventHandler Trigger;
        string Name { get; }
    }
}