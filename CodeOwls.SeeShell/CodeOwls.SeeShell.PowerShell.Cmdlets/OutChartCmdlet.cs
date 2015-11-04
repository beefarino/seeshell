using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using CodeOwls.SeeShell.Common;
using CodeOwls.SeeShell.Common.Attributes;
using CodeOwls.SeeShell.Common.Charts;
using CodeOwls.SeeShell.Common.ViewModels;
using CodeOwls.SeeShell.Common.ViewModels.Charts;


namespace CodeOwls.SeeShell.PowerShell.Cmdlets
{
    [Cmdlet(VerbsData.Out, Nouns.Chart, ConfirmImpact = ConfirmImpact.None)]
    public class OutChartCmdlet : TwoDimensionalOutputCmdletBase<ChartSeriesViewModel>
    {
        public OutChartCmdlet()
        {
            Type = ChartSeriesType.Area;
        }

        [Parameter()]
        public SwitchParameter AsControl { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "RangeSeriesParameterSet")]
        [ScriptBlockDynamicPropertyArgumentTransformation]
        public object HighFrom { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "RangeSeriesParameterSet")]
        [ScriptBlockDynamicPropertyArgumentTransformation]
        public object LowFrom { get; set; }

        [Parameter(ParameterSetName = "CommonPlotParameterSet")]
        [ScriptBlockDynamicPropertyArgumentTransformation]
        public object RadiusFrom { get; set; }

        [Parameter()]
        public SwitchParameter Legend { get; set; }

        [Parameter()]
        public ChartSeriesType Type { get; set; }

        private ChartViewModel _chartViewModel;

        private object _literalPlotValue;
        private object _literalByValue;

        protected override void BeginProcessing()
        {
            OutDebug("out-chart: ({0})", this.ParameterSetName);

            _literalByValue = By;

            _chartViewModel = GetExistingViewModel<ChartViewModel>("Charts", Name);
            ChartViewModel newViewModel = null;
            if (null == _chartViewModel)
            {
                newViewModel = new ChartViewModel
                {
                    Name = Name,
                    HasLegend = Legend.IsPresent
                };
                _chartViewModel = newViewModel;

                AddViewModelToDrive(_chartViewModel, "Charts", !AsControl.IsPresent);
            }

            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            switch (this.Type)
            {
                case (ChartSeriesType.RangeArea):
                case (ChartSeriesType.RangeColumn):
                    {
                        SpecAdapter =
                            new SingleSeriesDynamicMemberSpecificationAdapter<ChartSeriesViewModel>(
                                CreateSeriesViewModel);
                        Plot = new[] {HighFrom, LowFrom};
                        break;
                    }
                default:
                    SpecAdapter =
                        new MultiSeriesDynamicMemberSpecificationAdapter<ChartSeriesViewModel>(CreateSeriesViewModel);
                    break;
            }

            base.ProcessRecord();
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            var view = GetViewForItem<ChartViewModel>("Charts", _chartViewModel);
            if (null != view)
            {
                if (AsControl.IsPresent)
                {
                    WriteObject(view);
                }
                else
                {
                    
                }

            }
        }

        protected override ChartSeriesViewModel CreateSeriesViewModel(string seriesName, IEnumerable<string> plotPropertyName, string againstPropertyName, IPowerShellDataSource dataSource)
        {
            var series = new ChartSeriesViewModel() { Name = seriesName, EnableConfigureAxes = false };
            series.LiteralSeriesName = _literalPlotValue;
            series.LiteralByName = _literalByValue;
            series.SeriesType = Type;
            series.DataSource = dataSource;
            series.LabelMemberPath = series.XMemberPath = series.AngleMemberPath = againstPropertyName;
            series.ValueMemberPath = series.RadiusMemberPath = series.YMemberPath = plotPropertyName.FirstOrDefault();
            series.ScaleDescriptors = this.ScaleDescriptorTable;
            switch (series.SeriesType)
            {
                case (ChartSeriesType.Bubble):
                    {
                        if (null != RadiusFrom)
                        {
                            var spec = new DynamicMemberSpecification(new[] {RadiusFrom}, null,
                                                                      Across as PSScriptProperty,
                                                                      KeyOn as PSScriptProperty, 
                                                                      ScaleDescriptorTable);

                            series.DataSource.AddDynamicMemberSpecification(spec);

                            var prop = spec.PlotItemDescriptors.FirstOrDefault();
                            if (null != prop)
                            {
                                series.RadiusMemberPath = prop.MemberInfo.Name;
                            }
                        }
                        else
                        {
                            series.SeriesType = ChartSeriesType.Scatter;
                        }

                        break;
                    }

                case (ChartSeriesType.RangeArea):
                case (ChartSeriesType.RangeColumn):
                    {
                        if( 2 != plotPropertyName.Count())
                        {
                            //todo
                        }
                        series.HighMemberPath = plotPropertyName.First();
                        series.LowMemberPath = plotPropertyName.Last();
                        
                        break;
                    }
                default:
                    series.HighMemberPath = null;
                    series.LowMemberPath = null;
                    break;
            }

            series.EnableConfigureAxes = true;
            
            return series;
        }

        protected override void AddSeriesViewModelsToView(IEnumerable<ChartSeriesViewModel> viewModels)
        {
            if( null == viewModels || ! viewModels.Any())
            {
                return;
            }

            ////TODO: use member view model
            
            //var viewModel = GetExistingViewModel<ChartViewModel>("Charts", Name);
            //ChartViewModel newViewModel = null;
            //if (null == viewModel)
            //{
            //    newViewModel = new ChartViewModel
            //    {
            //        Name = Name,
            //        ChartSeries = new ObservableCollection<ChartSeriesViewModel>()
            //    };
            //    viewModel = newViewModel;
            //}
            
            Dispatch(() =>
            {
                var series = _chartViewModel.ChartSeries; //new ObservableCollection<ChartSeriesViewModel>( _chartViewModel.ChartSeries );
                
                foreach (var vm in viewModels)
                {
                    if (AsControl.IsPresent)
                    {
                        series.Where( s=>s.Name == vm.Name ).ToList().ForEach(s=>series.Remove(s));    
                    }
                    series.Add( vm );
                }
                //_chartViewModel.ChartSeries = series;
                
            });
        }
    }
}
