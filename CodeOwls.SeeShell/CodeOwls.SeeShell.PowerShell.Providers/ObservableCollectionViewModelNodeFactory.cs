using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.SeeShell.Common.Utility;
using CodeOwls.SeeShell.Common.ViewModels;
using CodeOwls.SeeShell.PowerShell.Providers.Charts;

namespace CodeOwls.SeeShell.PowerShell.Providers
{
    public class ObservableCollectionViewModelNodeFactory<T> : CollectionViewModelNodeFactory<T>, INewItem
        where T : ViewModelBase, new()
    {
        private readonly DispatchingCollectionDecorator<T> _collection;

        public ObservableCollectionViewModelNodeFactory(ObservableCollection<T> collection, string name ) : base(collection, name)
        {
            _collection = new DispatchingCollectionDecorator<T>( collection );
        }

        public IEnumerable<string> NewItemTypeNames
        {
            get { return null; }
        }

        public object NewItemParameters
        {
            get { return new T(); }
        }

        public IPathValue NewItem(IProviderContext context, string path, string itemTypeName, object newItemValue)
        {
            var newItem = context.DynamicParameters as T;
            if( null == newItem )
            {
                throw new ParameterBindingException( "Dynamic parameter is not of the expected type");
            }
            
            newItem.Name = path;

            _collection.Add( newItem );

            var factory = Resolve(context, newItem.Name);
            if( null == factory || ! factory.Any() )
            {
                return null;
            }
            return factory.First().GetNodeValue();
        }

    }
}