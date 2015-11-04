using System;
using System.Management.Automation;

namespace CodeOwls.SeeShell.PowerShell.Providers
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ScriptTransformationAttribute : ArgumentTransformationAttribute
    {
        private readonly ScriptBlock _script;

        public ScriptTransformationAttribute( ScriptBlock script )
        {
            _script = script;
        }

        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            var results = engineIntrinsics.InvokeCommand.InvokeScript(
                engineIntrinsics.SessionState,
                _script,
                inputData
                );

            if( null == results || 0 == results.Count )
            {
                return inputData;
            }

            return results[0];
        }
    }
}