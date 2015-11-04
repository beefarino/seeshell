using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CodeOwls.SeeShell.Common.Attributes;
using CodeOwls.SeeShell.Common.DataSources;

namespace CodeOwls.SeeShell.Common.ViewModels.Grids
{
    [DriveInfo("Grids", "Grids:/", "SeeShell Grids Repository")]
    public class GridViewModel : SingleDataSourceViewModelBase
    {
        public GridViewModel()
        {
            DataSource = new CompositePowerShellDataSource();
        }
    }

    
}
