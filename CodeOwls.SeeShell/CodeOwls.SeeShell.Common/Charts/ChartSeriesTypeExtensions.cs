using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeOwls.SeeShell.Common.Charts
{
    public static class ChartSeriesTypeExtensions
    {
        private static readonly ChartSeriesType[] CategorialValueSeries = new[]
                                                                              {
                                                                                  ChartSeriesType.Area,
                                                                                  ChartSeriesType.Column,
                                                                                  ChartSeriesType.Line,
                                                                                  ChartSeriesType.Spline,
                                                                                  ChartSeriesType.SplineArea,
                                                                                  ChartSeriesType.StepArea,
                                                                                  ChartSeriesType.StepLine,
                                                                                  ChartSeriesType.Waterfall,
                                                                                  ChartSeriesType.Timeline,
                                                                              };

        private static readonly ChartSeriesType[] CategoricalRangeSeries = new[]
                                                                               {
                                                                                   ChartSeriesType.RangeArea,
                                                                                   ChartSeriesType.RangeColumn,
                                                                               };
        private static readonly ChartSeriesType[] PolarSeries = new[]
                                                                    {
                                                                        ChartSeriesType.PolarArea,
                                                                        ChartSeriesType.PolarLine,
                                                                        ChartSeriesType.PolarScatter,
                                                                        ChartSeriesType.PolarSpline,
                                                                        ChartSeriesType.PolarSplineArea,
                                                                    };

        private static readonly ChartSeriesType[] RadialSeries = new[]
                                                                     {
                                                                         ChartSeriesType.RadialArea,
                                                                         ChartSeriesType.RadialLine,
                                                                         ChartSeriesType.RadialPie,
                                                                         ChartSeriesType.RadialColumn,
                                                                     };

        private static readonly ChartSeriesType[] ScatterSeries = new[]
                                                                      {
                                                                          ChartSeriesType.Bubble,
                                                                          ChartSeriesType.Scatter,
                                                                          ChartSeriesType.ScatterLine,
                                                                          ChartSeriesType.ScatterSpline,
                                                                      };

        public static readonly ChartAxisType[] CategorialAxes = new[]
                                                                    {
                                                                        ChartAxisType.CategoryDateTimeX,
                                                                        ChartAxisType.CategoryX,
                                                                        ChartAxisType.NumericY,
                                                                    };
        public static readonly ChartAxisType[] ScatterAxes = new[]
                                                                 {
                                                                     ChartAxisType.NumericX,
                                                                     ChartAxisType.NumericY,
                                                                 };

        public static readonly ChartAxisType[] PolarAxes = new[]
                                                               {
                                                                   ChartAxisType.NumericRadius,
                                                                   ChartAxisType.NumericAngle,
                                                               };
        public static readonly ChartAxisType[] RadialAxes = new[]
                                                                {
                                                                    ChartAxisType.NumericRadius,
                                                                    ChartAxisType.CategoryAngle,
                                                                };


        public static bool IsCategorical(this ChartSeriesType seriesType)
        {
            return seriesType.IsCategoricalValue() || seriesType.IsCategoricalRange();
        }
        public static bool IsCategoricalValue(this ChartSeriesType seriesType)
        {
            return CategorialValueSeries.Contains(seriesType);
        }
        public static bool IsCategoricalRange(this ChartSeriesType seriesType)
        {
            return CategoricalRangeSeries.Contains(seriesType);
        }

        public static bool IsScatter(this ChartSeriesType seriesType)
        {
            return ScatterSeries.Contains(seriesType);
        }
        public static bool IsPolar(this ChartSeriesType seriesType)
        {
            return PolarSeries.Contains(seriesType);
        }
        public static bool IsRadial(this ChartSeriesType seriesType)
        {
            return RadialSeries.Contains(seriesType);
        }

        public static ChartAxisType[] ValidAxisTypes(this ChartSeriesType seriesType)
        {
            if (seriesType.IsCategorical())
            {
                return CategorialAxes;
            }
            if (seriesType.IsScatter())
            {
                return ScatterAxes;
            }
            if (seriesType.IsPolar())
            {
                return PolarAxes;
            }
            if (seriesType.IsRadial())
            {
                return RadialAxes;
            }
            return new ChartAxisType[] { };
        }         
    }
}
