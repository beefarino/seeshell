using System;
using System.Management.Automation;
using CodeOwls.SeeShell.Common.Providers;
using CodeOwls.SeeShell.Common.Triggers;

namespace CodeOwls.SeeShell.Common.Attributes
{    
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ScriptBlockDataSourceArgumentTransformationAttribute : ArgumentTransformationAttribute
    {
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (null == inputData)
            {
                return inputData;
            }

            var inputObject = inputData.ToPSObject().BaseObject;

            if (!(inputObject is ScriptBlock))
            {
                return inputData;
            }

            var script = inputObject as ScriptBlock;

            var dataSource = new PowerShellDataSource
            {
                Name = script.ToString(),
                ScriptBlock = script.ToString(),
                Trigger = new ImmediateTrigger()
            };

            //TODO: add datasource to datasources: drive
            var drive = engineIntrinsics.InvokeCommand.InvokeScript("get-psdrive datasources") as IDriveOf<IPowerShellDataSource>;
            drive.Add(dataSource);

            return dataSource;
        }
    }
}
