using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeOwls.SeeShell.Common
{
    public static class TypeExtensions
    {
        public static bool IsNumeric( this Type type )
        {
            var ratypes = new[]
                              {
                                  typeof (Int16),
                                  typeof (Int32),
                                  typeof (Int64),
                                  typeof (IntPtr),

                                  typeof (UInt16),
                                  typeof (UInt32),
                                  typeof (UInt64),
                                  typeof (UIntPtr),

                                  typeof (Double),
                                  typeof (Single),
                                  typeof (Decimal),

                                  typeof (Byte),
                                  typeof (SByte)
                              };
            return ratypes.Contains(type);
        }
    }
}
