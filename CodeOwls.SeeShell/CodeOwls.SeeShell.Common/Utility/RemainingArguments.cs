using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace CodeOwls.SeeShell.Common.Utility
{
    public static class RemainingArguments
    {
        public static Hashtable ToHashtable(this object[] seriesDefinitions)
        {
            if (null == seriesDefinitions)
            {
                return null;
            }

            Hashtable table = new Hashtable();
            string key = null;
            List<object> accumulator = new List<object>();

            foreach (var arg in seriesDefinitions.ToList().ConvertAll(o => o.AsBaseObject<object>()))
            {
                var strarg = arg as string;
                if (null != strarg && strarg.StartsWith("-"))
                {
                    if (null != key && accumulator.Any())
                    {
                        table[key] = accumulator.ToArray();
                        accumulator.Clear();
                    }
                    key = strarg.Substring(1);
                }
                else if (arg is object[])
                {
                    var args = arg as object[];
                    accumulator.AddRange(args.ToList().ConvertAll(p=>p.AsBaseObject<object>()));
                }
                else
                {
                    accumulator.Add(arg);
                }
            }

            if (null != key)
            {
                table[key] = accumulator.ToArray();
            }
            return table;
        }


    }
}
