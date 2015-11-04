using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace CodeOwls.SeeShell.Common.Utility
{
    public class DispatchingCollectionDecorator<T> : ICollection<T>
    {
        private readonly ICollection<T> _collection;
        private readonly Dispatcher _dispatcher;

        private void Invoke( Action action )
        {
            _dispatcher.Invoke(action);
        }

        public void Add(T item)
        {
            Invoke(() => _collection.Add(item));
        }

        public void Clear()
        {
            Invoke(() => _collection.Clear());
        }

        public bool Contains(T item)
        {
            return _collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _collection.CopyTo( array, arrayIndex);
        }

        public bool Remove(T item)
        {
            bool rt = false;
            Invoke(() => { rt = _collection.Remove(item); } );
            return rt;
        }

        public int Count
        {
            get { return _collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return _collection.IsReadOnly; }
        }

        public DispatchingCollectionDecorator( ICollection<T> collection ) : this( collection, Application.Current.Dispatcher )
        {
        }

        public DispatchingCollectionDecorator(ICollection<T> collection, Dispatcher dispatcher )
        {
            _collection = collection;
            _dispatcher = dispatcher;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
