using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeOwls.SeeShell.Common.ViewModels.Charts;
using CodeOwls.SeeShell.PowerShell.Providers;
using Xunit;

namespace CodeOwls.SeeShell.Providers.Tests
{
    public class ChartProviderTests : ProviderTestsBase
    {
        [Fact]
        public void CanLoadProvider()
        {
            var result = Execute<bool>("[bool](get-psprovider -psp '{0}')", ProviderNames.ChartProvider );
            Assert.True(result, "Failed to load chart provider");
        }

        [Fact]
        public void CanAddChart()
        {
            var result = Execute<ChartViewModel>("new-item charts:/test1");
            Assert.True( null != result, "failed to add new chart item");
            Assert.Equal( "test1", result.Name );
        }

        [Fact]
        public void CanAddChartSeries()
        {
            var result = Execute<ChartSeriesViewModel>(@"
$chart = new-item charts:/test1
new-item charts:/test1/series -name series1;
");
            Assert.True(null != result, "failed to add new chart series item");
            Assert.Equal("series1", result.Name);
        }

        [Fact]
        public void CanAddChartSeriesWithCustomParameters()
        {
            var result = Execute<ChartSeriesViewModel>(@"
$chart = new-item charts:/test1
new-item charts:/test1/series -name series1 -seriestype spline;
");
            Assert.True(null != result, "failed to add new chart series item");
            Assert.Equal("series1", result.Name);
        }
    }
}
