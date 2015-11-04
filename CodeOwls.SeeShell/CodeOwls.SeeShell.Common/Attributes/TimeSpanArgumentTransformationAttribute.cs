using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text.RegularExpressions;
using CodeOwls.SeeShell.Common;

namespace CodeOwls.SeeShell.PowerShell.Providers
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class TimeSpanArgumentTransformationAttribute : ArgumentTransformationAttribute
    {
        public override object Transform(EngineIntrinsics engineIntrinsics, object inputData)
        {
            if (null == inputData)
            {
                return null;
            }

            var inputType = inputData.ToPSObject().BaseObject.GetType();

            if( inputType.IsAssignableFrom( typeof( short )) )
            {
                return TimeSpan.FromMilliseconds((double) inputData);
            }

            if (typeof(string) == inputType)
            {
                TimeSpan result = TimeSpan.MinValue;
                var s = inputData.ToString();
                if( TimeSpan.TryParse( s, out result ) )
                {
                    return result;
                }

                Regex re = new Regex(@"(?<n>[\.0-9]+)((?<ticks>t.*)|(?<milliseconds>ms.*)|(?<seconds>s.*)|(?<minutes>m.*)|(?<hours>h.*)||(?<days>d.*))", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                var match = re.Match(s);
                if( match.Success )
                {
                    var l = match.Groups["l"].Value;
                    var n = Double.Parse(match.Groups["n"].Value);

                    var map = new Dictionary<string, Func<double,TimeSpan>>
                                  {
                                      {"milliseconds", d => TimeSpan.FromMilliseconds(d)},
                                      {"seconds", d => TimeSpan.FromSeconds(d)},
                                      {"minutes", d => TimeSpan.FromMinutes(d)},
                                      {"hours", d => TimeSpan.FromHours(d)},
                                      {"days", d => TimeSpan.FromDays(d)},
                                      {"ticks", d => TimeSpan.FromTicks((int) d)}
                                  };

                    foreach( var pair in map )
                    {
                        if( match.Groups[pair.Key].Success)
                        {
                            return pair.Value(n);
                        }
                    }
                }
            }
            return inputData;
        }
    }
}