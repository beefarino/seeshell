using System.Collections;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.SeeShell.PowerShell.Providers
{
    public class ContainerPathNode : IPathValue
    {
        private readonly ShellContainer _container;
        public ContainerPathNode( IEnumerable collection, string name )
        {            
            _container = new ShellContainer( collection, name );
        }

        public object Item
        {
            get { return _container; }
        }

        public string Name
        {
            get { return _container.Name; }
        }

        public bool IsCollection
        {
            get { return true; }
        }
    }
}