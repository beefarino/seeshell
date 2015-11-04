using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using CodeOwls.SeeShell.Common;

namespace CodeOwls.SeeShell.PowerShell.Cmdlets
{
    class MultiSeriesDynamicMemberSpecificationAdapter<TSeriesViewModel> : IDynamicMemberSpecificationViewModelAdapter<TSeriesViewModel>
    {
        private readonly Func<string,IEnumerable<string>, string, IPowerShellDataSource, TSeriesViewModel> _creator;

        public MultiSeriesDynamicMemberSpecificationAdapter(           
            Func<string,IEnumerable<string>, string,IPowerShellDataSource,TSeriesViewModel> creator )
        {
            _creator = creator;
        }

        public IEnumerable<TSeriesViewModel> CreateSeriesViewModelsFromDynamicMemberSpecification(DynamicMemberSpecification spec, IPowerShellDataSource dataSource)
        {
            var vms =
                (from prop in spec.PlotItemDescriptors
                 let propName = prop.MemberInfo.Name.StartsWith( "_" ) ? null : prop.MemberInfo.Name
                 let scriptName = prop.MemberInfo is PSScriptProperty ? (prop.MemberInfo as PSScriptProperty).GetterScript.ToString() : null
                 let indexName = prop.IndexValue 
                 let seriesName = (indexName??propName??scriptName).ToString()
                 select CreateSeriesViewModelAndScale(seriesName, prop.MemberInfo.Name, spec.AgainstPropertyName, dataSource)).ToList();
            return vms;
        }

        private TSeriesViewModel CreateSeriesViewModelAndScale(string seriesName, string plot, string by, IPowerShellDataSource dataSource)
        {
            dataSource.AddDynamicScaleForProperty(plot);
            return _creator(seriesName, new[] { plot }, by, dataSource);
        }
    }
}
