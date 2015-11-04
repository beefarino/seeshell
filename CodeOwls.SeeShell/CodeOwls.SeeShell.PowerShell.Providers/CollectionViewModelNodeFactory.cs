using System;
using System.Collections.Generic;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.SeeShell.Common.ViewModels;

namespace CodeOwls.SeeShell.PowerShell.Providers
{
    public class CollectionViewModelNodeFactory<T> : PathNodeBase
        where T : ViewModelBase

    {
        private readonly string _name;
        protected internal IEnumerable<T> Collection { get; private set; }

        public CollectionViewModelNodeFactory(IEnumerable<T> collection, string name)
        {
            _name = name;
            Collection = collection;
        }

        public override IEnumerable<IPathNode> GetNodeChildren(IProviderContext context)
        {
            if (null != Collection)
            {
                foreach (T item in Collection)
                {
                    yield return new ViewModelNodeFactory<T>(item);
                }
            }
        }
        public override IPathValue GetNodeValue()
        {
            return new ContainerPathNode(Collection, Name);
        }

        public override string Name
        {
            get { return _name; }
        }

    }


}