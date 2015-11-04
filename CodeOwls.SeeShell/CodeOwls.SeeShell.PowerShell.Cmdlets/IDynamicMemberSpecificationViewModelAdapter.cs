using System.Collections.Generic;
using CodeOwls.SeeShell.Common;

namespace CodeOwls.SeeShell.PowerShell.Cmdlets
{
    public interface IDynamicMemberSpecificationViewModelAdapter<out TSeriesViewModel>
    {
        IEnumerable<TSeriesViewModel> CreateSeriesViewModelsFromDynamicMemberSpecification(
            DynamicMemberSpecification spec,
            IPowerShellDataSource dataSource
            );
    }
}