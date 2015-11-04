using System;
using System.Management.Automation;
using System.Windows.Threading;
using Caliburn.Micro;

namespace CodeOwls.SeeShell.Common.ViewModels
{
    public abstract class ViewModelBase : PropertyChangedBase
    {
        private string _name;
        private Dispatcher _dispatcher;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }
              
        protected event EventHandler DispatcherUpdated;

        void OnDispatcherUpdated()
        {
            EventHandler handler = DispatcherUpdated;
            if (handler != null) handler(this, System.EventArgs.Empty);
        }

        public Dispatcher Dispatcher
        {
            get { return _dispatcher; }
            set { _dispatcher = value; OnDispatcherUpdated();}
        }
    }
}