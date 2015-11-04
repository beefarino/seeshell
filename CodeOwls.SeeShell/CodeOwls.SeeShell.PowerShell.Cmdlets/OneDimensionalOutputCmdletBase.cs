using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Management.Automation;
using System.Text;
using CodeOwls.SeeShell.Common;
using CodeOwls.SeeShell.Common.Attributes;
using CodeOwls.SeeShell.Common.DataSources;
using CodeOwls.SeeShell.Visualizations.Dashboard;


namespace CodeOwls.SeeShell.PowerShell.Cmdlets
{
    public abstract class OneDimensionalOutputCmdletBase<TSeriesViewModel>: OutputCmdletBase
    {
        private DynamicMemberSpecification _spec;
        protected IDynamicMemberSpecificationViewModelAdapter<TSeriesViewModel> SpecAdapter;

        protected OneDimensionalOutputCmdletBase()
        {
            SpecAdapter = new MultiSeriesDynamicMemberSpecificationAdapter<TSeriesViewModel>(CreateSeriesViewModel);
        }

        [Parameter(Mandatory = true, ParameterSetName = "CommonPlotParameterSet")]
        [ScriptBlockDynamicPropertyArgumentTransformation]
        [Alias("Graph","Show")]
        public object[] Plot { get; set; }

        [Parameter()]
        [ScriptBlockDynamicPropertyArgumentTransformation]
        [PropertyNameToGroupByArgumentTransformation]
        [Alias( "ForValuesOf", "UsingIndex", "UsingKey", "IndexBy", "IndexOn", "KeyBy" )]
        public object KeyOn { get; set; }

        [Parameter()]
        [ScriptBlockDynamicPropertyArgumentTransformation]
        [PropertyNameToSelectPropertyArgumentTransformation]
        [Alias("Over")]
        public object Across { get; set; }

        
        private List<TSeriesViewModel> _scaleViewModels;
        
        protected List<TSeriesViewModel> SeriesViewModels
        {
            get { return _scaleViewModels; }
        }

        protected override void BeginProcessing()
        {            
            _scaleViewModels = new List<TSeriesViewModel>();
            base.BeginProcessing();
        }

        protected override void ProcessRecord( IPowerShellDataSource dataSource )
        {            
            ProcessRecord( dataSource, Plot, null, Across, KeyOn);
        }

        protected override void EndProcessing()
        {
            AddSeriesViewModelsToView(_scaleViewModels);
            base.EndProcessing();
        }
        
        protected void ProcessRecord( IPowerShellDataSource dataSource, object[] plot, object by, object across, object keyOn )
        {
            Dispatch(()=>DoProcessRecord( dataSource, plot, by,across,keyOn));
            //DoProcessRecord(dataSource, plot, by, across, keyOn);
        }

        void DoProcessRecord( IPowerShellDataSource dataSource, object[] plot, object by, object across, object keyOn )
        {
            IEnumerable<TSeriesViewModel> viewModels = null;
            try
            {
                
                dataSource.Dispatcher = Manager.Dispatcher;
                viewModels = CreatePlotSeriesViewModels(plot, by, across as PSScriptProperty, keyOn as PSScriptProperty);
            }
            catch (Exception)
            {
                viewModels = new TSeriesViewModel[] { CreateSeriesViewModel( String.Empty, new[]{String.Empty}, String.Empty, dataSource) };
                throw;
            }
            finally
            {
                if (null != viewModels)
                {
                    _scaleViewModels.AddRange(viewModels);
                }
            }
        }

        IEnumerable<TSeriesViewModel> CreatePlotSeriesViewModels(object[] plotItems, object againstItem, PSScriptProperty acrossItem, PSScriptProperty indexItem)
        {
            var dataSource = DataSource;

            _spec = new DynamicMemberSpecification(plotItems, againstItem, acrossItem, indexItem, ScaleDescriptorTable);
            dataSource.AddDynamicMemberSpecification(_spec);

            if (0 < dataSource.Data.Count || 0 < dataSource.AllRecords.Count )
            {
                CreateAndAddSeriesViewModels();
            }
            else
            {
                dataSource.Data.CollectionChanged += OnFirstDataItem;
                dataSource.AllRecords.CollectionChanged += OnFirstDataItem;
            }
            return null;
        }

        void OnFirstDataItem( object sender, NotifyCollectionChangedEventArgs args )
        {
            if( NotifyCollectionChangedAction.Add != args.Action)
            {
                return;
            }

            var dataSource = DataSource; 
            dataSource.Data.CollectionChanged -= OnFirstDataItem;
            dataSource.AllRecords.CollectionChanged -= OnFirstDataItem;
            CreateAndAddSeriesViewModels();
        }

        protected virtual IEnumerable<TSeriesViewModel> CreateSeriesViewModelsFromDynamicMemberSpecification( DynamicMemberSpecification spec )
        {
            var vms = SpecAdapter.CreateSeriesViewModelsFromDynamicMemberSpecification(spec, DataSource);
            return vms;
        }

        private void CreateAndAddSeriesViewModels()
        {
            var vms = CreateSeriesViewModelsFromDynamicMemberSpecification( _spec );
            if( ! vms.Any() )
            {
                vms = new List<TSeriesViewModel>
                {
                    CreateSeriesViewModel( String.Empty, new []{String.Empty}, String.Empty, DataSource )
                };
            }

            AddSeriesViewModelsToView(vms);
        }


        protected abstract TSeriesViewModel CreateSeriesViewModel(string seriesName, IEnumerable<string> plotPropertyNames, string againstPropertyName, IPowerShellDataSource dataSource);
        protected abstract void AddSeriesViewModelsToView( IEnumerable<TSeriesViewModel> viewModels );
    }
}
