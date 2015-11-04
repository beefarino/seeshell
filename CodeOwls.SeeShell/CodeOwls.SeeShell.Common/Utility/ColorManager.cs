using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace CodeOwls.SeeShell.Common.Utility
{
    public static class ColorManager
    {
        static readonly Random Random = new Random();

        public static Color AssignColor()
        {
            var clr = new byte[3];
            Random.NextBytes(clr);
            return Color.FromRgb(clr[0], clr[1], clr[2]);
        }

        public static Color AssignColor(int index, int count, Color rangeBaseColor)
        {
            float coeff = (float) index/(float) count;
            return Color.Multiply(rangeBaseColor, coeff);
        }
    }
}
