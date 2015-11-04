using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CodeOwls.SeeShell.Common
{
    public static class Singleton<T> where T : class, new()
    {
        private static T _t;

        public static T Instance
        {
            get
            {
                if( null == _t )
                {
                    var t = new T();
                    Interlocked.CompareExchange(ref _t, t, null);
                }

                return _t;
            }
        }
    }
}
