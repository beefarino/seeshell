using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.SeeShell.Common;
using CodeOwls.SeeShell.Common.Triggers;
using CodeOwls.SeeShell.Common.Utility;

namespace CodeOwls.SeeShell.PowerShell.Providers.DataSources
{
    class DataSourceRootNodeFactory : PathNodeBase, INewItem
    {
        private readonly DataSourceDrive _drive;

        public DataSourceRootNodeFactory(DataSourceDrive drive)
        {
            _drive = drive;           
        }

        public override IEnumerable<IPathNode> GetNodeChildren(IProviderContext context)
        {
            foreach( var dataSource in _drive.DataSources)
            {
                yield return new DataSourceNodeFactory( dataSource );
            }
        }
        public override IPathValue GetNodeValue()
        {
            return new PathValue( _drive, Name, true );
        }

        public override string Name
        {
            get { return "DataSourceRoot"; }
        }

        public IEnumerable<string> NewItemTypeNames
        {
            get { return null; }
        }

        public object NewItemParameters
        {
            get
            {
                
                return new DataSourceNewItemParameters();
            }
        }

        public IPathValue NewItem(IProviderContext context, string path, string itemTypeName, object newItemValue)
        {
            var parameters = context.DynamicParameters as DataSourceNewItemParameters;
            var name = path;
            var script = ConvertToScript(newItemValue);

            if( null == script )
            {
                throw new ArgumentException( "new item value must be a script block or string", "newItemValue");
            }

            var dataSource = new PowerShellDataSource {Name = name, ScriptBlock = script, UseDispatcher = true};
            
            if (null != parameters )
            {
                if (parameters.MaxItemCount.HasValue)
                {
                    dataSource.DataCollectionMaxSize = parameters.MaxItemCount.Value;
                }

                if( parameters.NoDispatcher.IsPresent)
                {
                    dataSource.UseDispatcher = false;
                }

                
                if( null != parameters.Args )
                {                   
                    string propertyName = null;
                    
                    var specs = parameters.Args.ToHashtable();

                    foreach (object key in specs.Keys)
                    {
                        var value = specs[key];
                        IScaleDescriptor descriptor = value as IScaleDescriptor;


                        if (null == descriptor)
                        {
                            if (value is Array)
                            {
                                descriptor = new ScaleDescriptor((object[])value);
                            }
                            else
                            {
                                descriptor = new ScaleDescriptor(value.ToString());
                            }
                        }

                        dataSource.Scales.Add(
                                new ScaleDescriptorAssignment
                                    {
                                        Scale = descriptor,
                                        PropertyName = key.ToString()
                                    }
                                );
                        
                    }
                }
            }

            ITrigger trigger = Singleton<ImmediateTrigger>.Instance;
            if( null != parameters && null != parameters.Trigger )
            {
                trigger = parameters.Trigger;
            }
            dataSource.Trigger = trigger;
            _drive.Add( dataSource );

            return new PathValue( dataSource, name, true );
        }

        private string ConvertToScript(object newItemValue)
        {
            if( null != newItemValue )
            {
                return newItemValue.ToString();
            }
            return null;
        }
    }
}