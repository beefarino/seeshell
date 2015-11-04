using System.Collections.Generic;
using System.Linq;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.SeeShell.Common.ViewModels;

namespace CodeOwls.SeeShell.PowerShell.Providers
{
    public class RootNodeFactory<T> : PathNodeBase, INewItem
        where T : ViewModelBase, new()
    {
        private readonly Drive<T> _drive;
        private readonly string _name;

        public RootNodeFactory(Drive<T> drive) : this( drive, drive.RootNodeName)
        {
        }

        public RootNodeFactory(Drive<T> drive, string name)
        {
            _drive = drive;
            _name = name;
        }

        public override IEnumerable<IPathNode> GetNodeChildren(IProviderContext context)
        {
            foreach( var item in Drive.Items )
            {
                yield return new ViewModelNodeFactory<T>(item);
            }
        }

        public override IPathValue GetNodeValue()
        {
            return new PathValue(_drive, Name, true);
        }

        public override string Name
        {
            get { return _name; }
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
            var item = context.DynamicParameters as T;
            item.Name = path;
            _drive.Add(item);
            return Resolve(context, path).First().GetNodeValue();
        }


        protected internal Drive<T> Drive { get { return _drive; } } 
    }
}