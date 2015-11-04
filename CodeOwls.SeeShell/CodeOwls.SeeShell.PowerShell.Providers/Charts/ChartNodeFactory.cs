using System.Collections.Generic;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using CodeOwls.SeeShell.Common.ViewModels.Charts;

namespace CodeOwls.SeeShell.PowerShell.Providers.Charts
{
    public class ChartNodeFactory : ViewModelNodeFactory<ChartViewModel>
// ObservableCollectionViewModelNodeFactory< ChartSeriesViewModel>
    {
        private readonly ChartViewModel _item;

        /*public ChartNodeFactory(ChartViewModel item) : base( item.ChartSeries, item.Name, 
            i=>new ContainedViewModelNodeFactory<ChartSeriesViewModel>( i, item.ChartSeries ) )*/
        public ChartNodeFactory(ChartViewModel item) : base( item, true )
        {
            _item = item;
        }

        public override IPathValue GetNodeValue()
        {
            return new ContainerPathValue(_item, Name);
        }
    }
}