using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace CodeOwls.SeeShell.Common.DataSources
{
    public static class StringExtensions
    {
        public static T Shift<T>( this IList<T> items )
        {
            if( 0 == items.Count )
            {
                return default(T);
            }

            T item = items[0];
            items.RemoveAt(0);
            return item;
        }

        public static Color? ToColor( this object value )
        {
            if( null == value )
            {
                return null;
            }

            if( value is Color )
            {
                return (Color) value;
            }

            if (value is string)
            {
                Type colorType = typeof (Colors);
                var colorProperty = (from p in colorType.GetProperties(BindingFlags.Public | BindingFlags.Static)
                                     where StringComparer.InvariantCultureIgnoreCase.Equals(p.Name, value.ToString())
                                     select p).FirstOrDefault();


                if (null != colorProperty)
                {
                    return (Color) colorProperty.GetValue(null, null);
                }

                var s = value.ToString();
                Regex re = new Regex(@"^\s*#?(?<alpha>[a-fA-F0-9]{2})?(?<red>[a-fA-F0-9]{2})(?<green>[a-fA-F0-9]{2})(?<blue>[a-fA-F0-9]{2})\s*$");
                var match = re.Match(s);
                if( match.Success )
                {
                    byte alpha = 0xFF;
                    byte red;
                    byte green;
                    byte blue;

                    if( match.Groups["alpha"].Success )
                    {
                        Byte.TryParse(match.Groups["alpha"].Value, NumberStyles.HexNumber, null, out alpha);
                    }
                    Byte.TryParse(match.Groups["red"].Value, NumberStyles.HexNumber, null, out red);
                    Byte.TryParse(match.Groups["green"].Value, NumberStyles.HexNumber, null, out green);
                    Byte.TryParse(match.Groups["blue"].Value, NumberStyles.HexNumber, null, out blue);

                    return Color.FromArgb(alpha, red, green, blue);
                }
            }

            //if (value is int)
            //{
            //    var i = (int) value;
            //    var b = BitConverter.GetBytes(i);
            //    return Color.FromArgb(b[3], b[2], b[1], b[0]);               
            //}

            return null;
        }

        public static string ToCanonicalName( this string name )
        {
            if( String.IsNullOrEmpty( name ) )
            {
                return name;
            }

            var bytes = Encoding.ASCII.GetBytes(name);
            using (var sha = SHA1.Create())
            {
                bytes = sha.ComputeHash(bytes);
            }

            name = BitConverter.ToString(bytes).Replace("-", "");

            return "__" + name;
        }
        
    }
}
