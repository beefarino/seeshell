using System;
using System.Management.Automation;

namespace CodeOwls.SeeShell.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PropertyNameToSelectPropertyArgumentTransformationAttribute : ArgumentTransformationAttribute
    {
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (null == inputData)
            {
                return inputData;
            }

            var inputObject = inputData.ToPSObject().BaseObject;

            var name = inputObject as string;
            if (null == name)
            {
                return inputData;
            }

            var scriptBlock = ScriptBlock.Create( "@($this | select-object -expand '" + name + "' )");

            var property = new PSScriptProperty("_SelectExpand" + Guid.NewGuid().ToString("N"), scriptBlock);

            return property;
        }
    }
}