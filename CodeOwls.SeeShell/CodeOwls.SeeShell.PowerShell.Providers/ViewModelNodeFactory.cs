using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.SeeShell.Common.Attributes;
using CodeOwls.SeeShell.Common.ViewModels;

namespace CodeOwls.SeeShell.PowerShell.Providers
{
    public class ViewModelNodeFactory<T> : PathNodeBase where T : ViewModelBase
    {
        protected internal readonly T ViewModel;
        private readonly bool _isContainer;
        private readonly Type _type;

        public ViewModelNodeFactory(T viewModel ) : this( viewModel, false)
        {
        }

        public ViewModelNodeFactory(T viewModel, bool isContainer)
        {
            _type = viewModel.GetType();
            ViewModel = viewModel;
            _isContainer = isContainer;
        }

        public override IEnumerable<IPathNode> GetNodeChildren(IProviderContext providerContext)
        {
            List<IPathNode> factories = new List<IPathNode>();
            var properties = _type.GetProperties();

            foreach( var prop in properties )
            {
                var pt = prop.PropertyType;
                var pa =
                    prop.GetCustomAttributes(typeof (ContainerAttribute), true).FirstOrDefault() as ContainerAttribute;
                var containerName = null == pa || null == pa.Name ? prop.Name : pa.Name;
                var value = prop.GetValue(ViewModel, null);

            }
            
            var props = (from prop in properties 
                         let pt = prop.PropertyType
                         let pa = prop.GetCustomAttributes(typeof (ContainerAttribute), true).FirstOrDefault() as ContainerAttribute
                         let containerName = null == pa || null == pa.Name ? prop.Name : pa.Name
                         where ( pt.IsGenericType &&
                                 pt.GetGenericTypeDefinition() == typeof (ObservableCollection<>)) ||
                               typeof( ViewModelBase ).IsAssignableFrom( pt )
                         select new
                                    {
                                        Property = prop,
                                        Value = prop.GetValue( ViewModel, null),
                                        IsContainer = null != pa,
                                        ContainerName = containerName,
                                        CollectionGenericTypeParameter = pt.GetGenericArguments()[0]
                                    } ).ToList();

            var containers = from prop in props
                             where prop.IsContainer
                             select CreateClosedCollectionViewModelNodeFactory(
                                 prop.CollectionGenericTypeParameter,
                                 prop.Value,
                                 prop.Property.Name 
                                 );

            var children = from prop in props
                           where !prop.IsContainer
                           let value = prop.Property.GetValue(ViewModel, null) 
                           let e = value as IEnumerable<ViewModelBase> ?? new[]{(ViewModelBase)value}
                           where null != e
                           from i in e
                           select new ViewModelNodeFactory<ViewModelBase>(i);

            factories.AddRange( containers );
            factories.AddRange( children );
            return factories;
        }

        IPathNode CreateClosedCollectionViewModelNodeFactory( Type type, object value, string name )
        {
            var collectionType = typeof (CollectionViewModelNodeFactory<>).MakeGenericType(type);
            var factory = Activator.CreateInstance(collectionType, value, name );
            return factory as IPathNode;
        }
        public override IPathValue GetNodeValue()
        {
            return new PathValue(ViewModel, Name, _isContainer);
        }
        
        public override string Name
        {
            get { return ViewModel.Name; }
        }
    }
}