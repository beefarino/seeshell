using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using CodeOwls.SeeShell.Common;

namespace CodeOwls.SeeShell.PowerShell.Cmdlets
{
    class SingleSeriesDynamicMemberSpecificationAdapter<TSeriesViewModel> : IDynamicMemberSpecificationViewModelAdapter<TSeriesViewModel>
    {
        private readonly Func<string, IEnumerable<string>, string, IPowerShellDataSource, TSeriesViewModel> _creator;

        public SingleSeriesDynamicMemberSpecificationAdapter(
            Func<string, IEnumerable<string>, string,IPowerShellDataSource,TSeriesViewModel> creator )
        {
            _creator = creator;
        }

        public IEnumerable<TSeriesViewModel> CreateSeriesViewModelsFromDynamicMemberSpecification(DynamicMemberSpecification spec, IPowerShellDataSource dataSource)
        {
            var vms = CreateSeriesViewModelAndScale(spec.PlotItemDescriptors, spec.AgainstPropertyName, dataSource);
            return new[]{vms};
        }

        private TSeriesViewModel CreateSeriesViewModelAndScale(IEnumerable<DynamicMemberDescriptor> plot, string by, IPowerShellDataSource dataSource)
        {
            var props = plot.ToList();
            props.ForEach(p=>dataSource.AddDynamicScaleForProperty(p.MemberInfo.Name));
            var names = from prop in props select prop.MemberInfo.Name;
            var seriesNames =
                (from prop in plot
                    let propName = prop.MemberInfo.Name.StartsWith("_") ? null : prop.MemberInfo.Name
                    let scriptName =
                        prop.MemberInfo is PSScriptProperty
                            ? (prop.MemberInfo as PSScriptProperty).GetterScript.ToString()
                            : null
                    let indexName = prop.IndexValue
                    select (indexName ?? propName ?? scriptName).ToString()).ToArray();
            var seriesName = String.Join(", ", seriesNames );
            return _creator( seriesName, names.ToList(), by, dataSource);
        }
    }
}