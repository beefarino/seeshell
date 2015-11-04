using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeOwls.SeeShell.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DriveInfoAttribute : Attribute
    {
        private readonly string _driveName;
        private readonly string _providerDriveRoot;
        private readonly string _providerDescription;

        public DriveInfoAttribute( string driveName, string providerDescription )
            : this( driveName, String.Empty, providerDescription )
        {            
        }
            
        public DriveInfoAttribute( string driveName, string providerDriveRoot, string providerDescription )
        {
            _driveName = driveName;
            _providerDriveRoot = providerDriveRoot;
            _providerDescription = providerDescription;
        }

        public string ProviderDescription
        {
            get { return _providerDescription; }
        }

        public string ProviderDriveRoot
        {
            get { return _providerDriveRoot; }
        }

        public string DriveName
        {
            get { return _driveName; }
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ContainerAttribute : Attribute
    {
        public string Name { get; private set; }

        public ContainerAttribute()
        {            
        }

        public ContainerAttribute( string name )
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ChildAttribute : Attribute
    {
        public string Name { get; private set; }

        public ChildAttribute()
        {
        }

        public ChildAttribute(string name)
        {
            Name = name;
        }
    }
}
