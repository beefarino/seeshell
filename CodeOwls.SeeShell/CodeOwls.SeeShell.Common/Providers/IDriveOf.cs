using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeOwls.SeeShell.Common.Providers
{
    public interface IDriveOf<T>
    {
        void Add(T item);
        void Add(T item, bool show);
        void Remove(T item);
        IEnumerable<T> Items { get; }
        
    }

    public interface IViewMap
    {
        object GetViewForItem(object item);
    }

}
