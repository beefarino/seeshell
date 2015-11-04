using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using CodeOwls.SeeShell.Common;
using CodeOwls.SeeShell.Common.ViewModels;
using CodeOwls.SeeShell.PowerShell.Cmdlets;

namespace CodeOwls.SeeShell.PowerShell.Cmdlets
{
    public class SolidObjectViewModel : DataViewModelBase
    {
        private readonly ObservableCollection<object> _allRecords;
        
        public string SeriesName { get; set; }
        public IEnumerable<string> PlotPropertyNames { get; set; }
        public string AgainstPropertyName { get;set; }

        public SolidObjectViewModel()
        {
            _allRecords = new ObservableCollection<object>();
        }

        public override ObservableCollection<object> AllRecords
        {
            get { return _allRecords; }
        }
    }

    [Cmdlet( VerbsData.ConvertTo, "SolidObject")]
    public class ConvertToSolidObjectCmdlet : OneDimensionalOutputCmdletBase<SolidObjectViewModel>
    {
        private readonly List<SolidObjectViewModel> _items = new List<SolidObjectViewModel>();

        protected override void EndProcessing()
        {
            base.EndProcessing();
            
            if (null == _items)
            {
                return;
            }

            WriteObject( _items, true );
        }

        protected override SolidObjectViewModel CreateSeriesViewModel(string seriesName, IEnumerable<string> plotPropertyNames, string againstPropertyName,
            IPowerShellDataSource dataSource)
        {
            return new SolidObjectViewModel
            {
                AgainstPropertyName = againstPropertyName,
                SeriesName = seriesName,
                PlotPropertyNames = new List<string>(plotPropertyNames)
            };
        }

        protected override void AddSeriesViewModelsToView(IEnumerable<SolidObjectViewModel> viewModels)
        {
            _items.AddRange( viewModels );
        }
    }
}
