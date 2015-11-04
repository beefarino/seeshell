using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Windows.Input;
using CodeOwls.SeeShell.Common.DataSources;

namespace CodeOwls.SeeShell.Common
{
    public static class ObjectExtensions
    {
        public static PSObject ToPSObject(this object o)
        {
            return PSObject.AsPSObject(o);
        }

        public static T AsBaseObject<T>( this object o )
        {
            return (T)o.ToPSObject().BaseObject ;
        }
        
        public static T SafeAsBaseObject<T>(this object o) where T : class
        {
            return o.ToPSObject().BaseObject as T;
        }

        public static bool SafeAddDynamicProperty( this PSObject pso, PSPropertyInfo newProperty )
        {
            //if( null == pso.Properties[newProperty.Name] )
            {
                pso.Properties.Add( newProperty );
                return true;
            }

            return false;
        }
        
    }

}