using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.SeeShell.Common;

namespace CodeOwls.SeeShell.PowerShell.Providers.DataSources
{
    class DataSourceNodeFactory : PathNodeBase, IRemoveItem, INewItem
    {
        private readonly IPowerShellDataSource _dataSource;

        public DataSourceNodeFactory(IPowerShellDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public override IEnumerable<IPathNode> GetNodeChildren(IProviderContext context)
        {
            var data = _dataSource.Scales;
            foreach( var item in data )
            {
                yield return new ScaleAssignmentNodeFactory( 
                    item
                );
            }
        }

        public override IPathValue GetNodeValue()
        {
            return new PathValue( _dataSource, Name, true );
        }

        public override string Name
        {
            get { return _dataSource.Name; }
        }

        public object RemoveItemParameters
        {
            get { return null; }
        }

        public void RemoveItem(IProviderContext context, string path, bool recurse)
        {
            var drive = context.Drive as DataSourceDrive;
            if( null == drive )
            {
                throw new InvalidOperationException( "the specified drive does not support the remove data source operation" );
            }
            drive.Remove( _dataSource );
        }

        public IEnumerable<string> NewItemTypeNames
        {
            get { return null; }
        }

        public object NewItemParameters
        {
            get { return null; }
        }

        public IPathValue NewItem(IProviderContext context, string path, string itemTypeName, object newItemValue)
        {
            IScaleDescriptor descriptor = null;
            if( null == newItemValue )
            {
                throw new ArgumentNullException( "newItemValue");
            }
            newItemValue = PSObject.AsPSObject(newItemValue).BaseObject;
            if( newItemValue is string )
            {
                descriptor = new ScaleDescriptor(newItemValue as string);
            }
            else if ( newItemValue is object[] )
            {
                descriptor = new ScaleDescriptor(newItemValue as object[] );          
            }
            else
            {
                throw new ArgumentException( "the new range value must be specified as a valid range descriptor", "newItemValue" );
            }

            _dataSource.Scales.Add( new ScaleDescriptorAssignment {PropertyName = path, Scale = descriptor } );

            return Resolve(context, path).First().GetNodeValue();
        }
    }
}