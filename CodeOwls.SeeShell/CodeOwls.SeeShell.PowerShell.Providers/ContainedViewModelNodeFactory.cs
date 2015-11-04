using System.Collections.Generic;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.SeeShell.Common.Utility;
using CodeOwls.SeeShell.Common.ViewModels;
using CodeOwls.SeeShell.PowerShell.Providers.Charts;

namespace CodeOwls.SeeShell.PowerShell.Providers
{
    public class ContainedViewModelNodeFactory<T> : ViewModelNodeFactory<T>,
                                                    IRemoveItem, IRenameItem 
    
        where T : ViewModelBase
    {
        private readonly ICollection<T> _collection;

        public ContainedViewModelNodeFactory(T viewModel, ICollection<T> collection) : base(viewModel)
        {
            _collection = new DispatchingCollectionDecorator<T>(collection);
        }

        public object RemoveItemParameters
        {
            get { return null; }
        }

        public void RemoveItem(IProviderContext context, string path, bool recurse)
        {
            _collection.Remove(ViewModel);
        }


        public object RenameItemParameters
        {
            get { return null; }
        }

        public void RenameItem(IProviderContext context, string path, string newName)
        {
            ViewModel.Name = newName;
        }
    }
}