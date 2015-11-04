using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using CodeOwls.SeeShell.Common.Charts;
using CodeOwls.SeeShell.Common.DataSources;
using CodeOwls.SeeShell.Common.ViewModels.Charts;
using Xunit;
namespace CodeOwls.SeeShell.Common.Tests
{
    public class ChartSeriesAxisSelectorTests
    {
        [Fact]
        public void CategorySeriesCreatesCategoryXAxisForString()
        {
            var dataSource = new NullPowerShellDataSource();
            dataSource.Data.Add(new SolidPSObjectBase(new PSObject(new {Label = "Test", Value = 123})));
            ChartSeriesViewModel viewModel = new ChartSeriesViewModel
                                                 {
                                                     LabelMemberPath = "Label",
                                                     ValueMemberPath = "Value",
                                                     DataSource = dataSource,
                                                     EnableConfigureAxes = true
                                                 };
            var axis = viewModel.XAxis;
            Assert.NotNull( axis );
            Assert.Equal( ChartAxisType.CategoryX, axis.AxisType );        
        }

        [Fact]
        public void CategorySeriesCreatesDateTimeXAxisForDateTime()
        {
            var dataSource = new NullPowerShellDataSource();
            dataSource.Data.Add(new SolidPSObjectBase(new PSObject(new { Label = DateTime.Now, Value = 123 })));
            ChartSeriesViewModel viewModel = new ChartSeriesViewModel
            {
                SeriesType = ChartSeriesType.Column,
                LabelMemberPath = "Label",
                ValueMemberPath = "Value",
                DataSource = dataSource,
                EnableConfigureAxes = true
            };
            var axis = viewModel.XAxis;
            Assert.NotNull(axis);
            Assert.Equal(ChartAxisType.CategoryDateTimeX, axis.AxisType);
        }

        [Fact]
        public void CategorySeriesCreatesCategoryXAxisForInteger()
        {
            var dataSource = new NullPowerShellDataSource();
            dataSource.Data.Add(new SolidPSObjectBase(new PSObject(new { Label = 5, Value = 123 })));
            ChartSeriesViewModel viewModel = new ChartSeriesViewModel
            {
                SeriesType = ChartSeriesType.Area,
                LabelMemberPath = "Label",
                ValueMemberPath = "Value",
                DataSource = dataSource,
                EnableConfigureAxes = true
            };
            var axis = viewModel.XAxis;
            Assert.NotNull(axis);
            Assert.Equal(ChartAxisType.CategoryX, axis.AxisType);
        }

        public enum Stuff
        {
            A,B,C
        }

        [Fact]
        public void CategorySeriesCreatesCategoryXAxisForEnum()
        {
            
            var dataSource = new NullPowerShellDataSource();
            dataSource.Data.Add(new SolidPSObjectBase(new PSObject(new { Label = Stuff.A, Value = 123 })));
            ChartSeriesViewModel viewModel = new ChartSeriesViewModel
            {
                SeriesType = ChartSeriesType.Area,
                LabelMemberPath = "Label",
                ValueMemberPath = "Value",
                DataSource = dataSource,
                EnableConfigureAxes = true
            };
            var axis = viewModel.XAxis;
            Assert.NotNull(axis);
            Assert.Equal(ChartAxisType.CategoryX, axis.AxisType);
        }

        [Fact]
        public void ScatterSeriesCreatesNumericXAxisForInteger()
        {
            var dataSource = new NullPowerShellDataSource();
            dataSource.Data.Add(new SolidPSObjectBase(new PSObject(new { Label = 5, Value = 123 })));
            ChartSeriesViewModel viewModel = new ChartSeriesViewModel
            {
                SeriesType = ChartSeriesType.Scatter,
                XMemberPath = "Label",
                YMemberPath = "Value",
                DataSource = dataSource,
                EnableConfigureAxes = true
            };
            var axis = viewModel.XAxis;
            Assert.NotNull(axis);
            Assert.Equal(ChartAxisType.NumericX, axis.AxisType);
        }

        [Fact]
        public void ScatterSeriesThrowsWhenXAxisIsString()
        {
            var dataSource = new NullPowerShellDataSource();
            dataSource.Data.Add(new SolidPSObjectBase(new PSObject(new {Label = "ASDF", Value = 123})));
            Assert.Throws<Exceptions.InvalidChartAxisTypeException>(() =>
                                                                        {
                                                                            ChartSeriesViewModel
                                                                                viewModel = new ChartSeriesViewModel
                                                                                                {
                                                                                                    SeriesType =
                                                                                                        ChartSeriesType
                                                                                                        .Scatter,
                                                                                                    XMemberPath =
                                                                                                        "Label",
                                                                                                    YMemberPath =
                                                                                                        "Value",
                                                                                                    DataSource =
                                                                                                        dataSource,
                                                                                                    EnableConfigureAxes = true
                                                                                                };
                                                                        }
                );
        }

        [Fact]
        public void PolarSeriesCreatesNumericAngleXAxisForInteger()
        {
            var dataSource = new NullPowerShellDataSource();
            dataSource.Data.Add(new SolidPSObjectBase(new PSObject(new { Label = 5, Value = 123 })));
            ChartSeriesViewModel viewModel = new ChartSeriesViewModel
            {
                SeriesType = ChartSeriesType.PolarArea,
                AngleMemberPath = "Label",
                RadiusMemberPath = "Value",
                
                DataSource = dataSource,
                EnableConfigureAxes = true
            };
            var axis = viewModel.XAxis;
            Assert.NotNull(axis);
            Assert.Equal(ChartAxisType.NumericAngle, axis.AxisType);
        }

        [Fact]
        public void PolarSeriesCreatesNumericRadiusYAxisForInteger()
        {
            var dataSource = new NullPowerShellDataSource();
            dataSource.Data.Add(new SolidPSObjectBase(new PSObject(new { Label = 5, Value = 123 })));
            ChartSeriesViewModel viewModel = new ChartSeriesViewModel
            {
                SeriesType = ChartSeriesType.PolarArea,
                AngleMemberPath = "Label",
                RadiusMemberPath = "Value",
                DataSource = dataSource,
                EnableConfigureAxes = true
            };
            var axis = viewModel.YAxis;
            Assert.NotNull(axis);
            Assert.Equal(ChartAxisType.NumericRadius, axis.AxisType);
        }
    }
}
