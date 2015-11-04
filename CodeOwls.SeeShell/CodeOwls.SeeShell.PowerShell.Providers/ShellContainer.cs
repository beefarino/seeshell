using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeOwls.SeeShell.PowerShell.Providers
{
    public class ShellContainer
    {
        private readonly ArrayList _collection;
        private readonly string _name;

        public ShellContainer( IEnumerable collection, string name )
        {
            _collection = new ArrayList();
            foreach( var item in collection )
            {
                _collection.Add(item);
            }
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public int Count
        {
            get { return _collection.Count; }
        }
    }
}
