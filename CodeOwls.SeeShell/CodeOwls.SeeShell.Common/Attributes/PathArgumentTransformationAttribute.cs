using System;
using System.Linq;
using System.Management.Automation;

namespace CodeOwls.SeeShell.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class PathArgumentTransformationAttribute : ArgumentTransformationAttribute
    {
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (null == inputData)
            {
                return null;
            }

            var inputType = inputData.ToPSObject().BaseObject.GetType();

            if (typeof(string) == inputType)
            {
                var path = inputData.ToString();
                if (engineIntrinsics.SessionState.Path.IsValid(path))
                {
                    var pathInfo = engineIntrinsics.SessionState.Path.GetResolvedPSPathFromPSPath(path).First();
                    var result = engineIntrinsics.InvokeProvider.Item.Get(pathInfo.Path).FirstOrDefault();
                    return result;
                }
            }
            
            return inputData;
        }
    }
}