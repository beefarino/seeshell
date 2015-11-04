using System;
using System.Windows.Media;

namespace CodeOwls.SeeShell.Common
{
    public interface IRangeDescriptor
    {
        int ScaleIndex { get; set; }
        double Minimum { get; }
        double Maximum { get; }
        Color? Color {get; }
    }
}