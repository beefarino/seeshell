using System;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace CodeOwls.SeeShell.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ScriptBlockDynamicPropertyArgumentTransformationAttribute : ArgumentTransformationAttribute
    {
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (null == inputData)
            {
                return inputData;
            }

            var array = inputData as object[];
            if (null != array)
            {
                for( var c = 0; c < array.Length; ++c )
                {
                    array[c] = TransformItem(engineIntrinsics, array[c]);
                }
            }
            else
            {
                inputData = TransformItem(engineIntrinsics, inputData);
            }
            return inputData;
        }

        object TransformItem( EngineIntrinsics engineIntrinsics, object inputData )
        {
            var inputObject = inputData.ToPSObject().BaseObject;

            var scriptBlock = inputObject as ScriptBlock;
            if (null == scriptBlock)
            {
                return inputData;
            }

            scriptBlock = ScriptBlock.Create(  "if( -not($_) ){ $_ = $this; } " + scriptBlock );

            var property = new PSScriptProperty( "_" + Guid.NewGuid().ToString("N"), scriptBlock);
            
            return property;
        }

        public static string GetReadableName( PSScriptProperty prop )
        {
            var getter = prop.GetterScript;

            var name = getter.ToString().Replace("if( -not($_) ){ $_ = $this; } ", "");
            return String.Format("{{ {0} }}", name);
        }
    }
}