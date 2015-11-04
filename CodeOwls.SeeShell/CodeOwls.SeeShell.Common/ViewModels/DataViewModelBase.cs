using System.Collections.ObjectModel;

namespace CodeOwls.SeeShell.Common.ViewModels
{
    public abstract class DataViewModelBase : ViewModelBase
    {
        public int AllRecordsCount { get { return AllRecords.Count; } }
        public abstract ObservableCollection<object> AllRecords { get; }
    }
}