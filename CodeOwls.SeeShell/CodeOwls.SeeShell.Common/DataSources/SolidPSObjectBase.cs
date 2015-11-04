using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace CodeOwls.SeeShell.Common.DataSources
{
    public class SolidPSObjectBase : IEquatable<SolidPSObjectBase>
    {
        private readonly PSObject _psobject;
        private readonly Log _log;
        private IDictionary _memoizedPropertyValues;

        public PSObject PSObject
        {
            get { return _psobject; }
        }

        bool GetMemoizedPropertyValue<T>(string name, out T value)
        {
            value = default(T);
            if (_memoizedPropertyValues.Contains(name))
            {
                value = (T)_memoizedPropertyValues[name];
                return true;
            }
            return false;
        }

        public T GetPropValue<T>(string name)
        {
            T value = GetPropValue(name, () => default(T));
            return value;
        }

        public T GetPropValue<T>(string name, Func<T> defaultAction)
        {
            using (_log.PushContext("GetPropValue<{0}> [{1}]", typeof (T).Name, name))
            {
                try
                {
                    T value;
/*
                    if (GetMemoizedPropertyValue(name, out value))
                    {
                        return value;
                    }
*/

                    var prop = GetPropertyByName(name);
                    if (null == prop)
                    {
                        return defaultAction();
                    }

                    using (DefaultRunspaceManager.ForCurrentThread)
                    {
                        value = default(T);
                        if (prop.MemberType != PSMemberTypes.ScriptProperty)
                        {
                            //TODO: use convert methods based on typeof(T)
                            value = (T) PSObject.AsPSObject(prop.Value).BaseObject;
                            return value;
                        }

                        value = (T) PSObject.AsPSObject(prop.Value).BaseObject;
/*                        _memoizedPropertyValues[name] = value;*/
                        return value;
                    }

                }
                catch (Exception)
                {
                    return defaultAction();
                }
            }
        }

        public T GetPropValue<T, I>(string name, Func<I,T> adapter, Func<T> defaultAction)
        {
            try
            {
                T value;
                if (GetMemoizedPropertyValue(name, out value))
                {
                    return value;
                }
                
                var prop = GetPropertyByName(name);
                if (null == prop)
                {
                    return defaultAction();
                }

                using (DefaultRunspaceManager.ForCurrentThread)
                {
                    value = default(T);
                    if (prop.MemberType != PSMemberTypes.ScriptProperty)
                    {
                        //TODO: use convert methods based on typeof(T)
                        I ivalue = (I)PSObject.AsPSObject(prop.Value).BaseObject;
                        value = adapter(ivalue);
                        _memoizedPropertyValues[name] = value;
                        return value;
                    }

                    value = (T)PSObject.AsPSObject(prop.Value).BaseObject;
                    _memoizedPropertyValues[name] = value;
                    return value;
                }

            }
            catch (Exception)
            {
                return defaultAction();
            }
        }

        internal PSPropertyInfo GetPropertyByName(string name)
        {
            if( null == _psobject.Properties )
            {
                return null;
            }

            var prop =
                _psobject.Properties.FirstOrDefault(p => StringComparer.InvariantCultureIgnoreCase.Equals(name, p.Name));
            return prop;
        }

        public void SetPropValue(string name, object value)
        {
            var prop = GetPropertyByName(name);
            if (null == prop)
            {
                return;
            }
            prop.Value = value;
        }

        public SolidPSObjectBase(PSObject psobject)
        {
            _memoizedPropertyValues = new Dictionary<string, object>();
            _log = new Log(GetType());
            _psobject = psobject;
        }

        public SolidPSObjectBase()
        {
            _memoizedPropertyValues = new Dictionary<string, object>();
             _log = new Log(GetType());
        }

        public string GetPropTypeName(string name)
        {
            var psi = GetPropertyByName(name);
            if( null == psi )
            {
                return null;
            }
            return psi.TypeNameOfValue;
        }

        public bool Equals(SolidPSObjectBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._psobject, _psobject);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (SolidPSObjectBase)) return false;
            return Equals((SolidPSObjectBase) obj);
        }

        public override int GetHashCode()
        {
            return (_psobject != null ? _psobject.GetHashCode() : 0);
        }

        public static bool operator ==(SolidPSObjectBase left, SolidPSObjectBase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SolidPSObjectBase left, SolidPSObjectBase right)
        {
            return !Equals(left, right);
        }
    }
}