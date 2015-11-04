using System.Collections.Generic;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.SeeShell.Common.ViewModels;

namespace CodeOwls.SeeShell.PowerShell.Providers
{
    public abstract class ContainerViewModelNodeFactory<T> : ViewModelNodeFactory<T>
        where T : ViewModelBase
    {
        protected ContainerViewModelNodeFactory(T viewModel) : base(viewModel, true)
        {
        }

        public abstract override IEnumerable<IPathNode> GetNodeChildren(IProviderContext context);
    }
}