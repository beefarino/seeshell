using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using CodeOwls.SeeShell.Common;
using CodeOwls.SeeShell.Common.Support;
using CodeOwls.SeeShell.Common.ViewModels;
using CodeOwls.SeeShell.Visualizations.ViewModels;
using CodeOwls.SeeShell.Visualizations.Views;


namespace CodeOwls.SeeShell.Visualizations.Dashboard
{    
    public static class Manager
    {
        private static Log Log = new Log( typeof(Manager));
        private static Application _application;
        private static bool _keepApplicationAlive;
        private static long _windowCount = 0;

        static public void Shutdown()
        {
            Interlocked.Exchange(ref _windowCount, 0);
            var app = Interlocked.Exchange(ref _application, null);
            if( null == app )
            {
                return;
            }

            Log.Info("initiating shutdown procedure on dispatch thread");
            app.Dispatcher.Invoke((Action)(() =>
                                                      {
                                                          Log.Info( "closing open windows");
                                                          foreach (Window window in app.Windows)
                                                          {
                                                              window.Close();
                                                          }
                                                          Log.Info("shutting down application");
                                                          app.Shutdown();
                                                          Log.Info("shutdown complete");
                                                      }));
            
            
        }

        static void Initialize()
        {
            if (null != _application)
            {
                return;
            }

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            _application = Application.Current;
            if( null != _application )
            {
                _keepApplicationAlive = false;
                Application.Current.Dispatcher.Invoke( (Action)InitializeSeeShellApplication );
                return;
            }

            using (var wait = new ManualResetEvent(false))
            {
                Log.Info( "initializaing application thread");
                Thread thread = new Thread(RunApplication);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Name = "WPF Application Thread";
                thread.Start(wait);
                Log.Debug("waiting on application initialization event");
                wait.WaitOne();
                wait.Close();
                Log.Debug("application initialization event received");
            }
        }

        public static void BootstrapVisualizationAssemblies()
        {
            //var p = typeof(Infragistics.Controls.Charts.XamDataChart);
            
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var msg = String.Format("SeeShell has experienced a {0}terminating exception.",
                                        e.IsTerminating ? "" : "non-");
                Log.Error(
                    msg,
                    e.ExceptionObject as Exception
                    );

                //IssueReporter.ReportIssue(e.ExceptionObject.ToString(), null, msg);
            }
            catch
            {                
            }
        }

        public static Dispatcher Dispatcher
        {
            get 
            { 
                Initialize();
                return Application.Current.Dispatcher;
            }
        }

        public static object Create(VisualizationWindowViewModel viewModel)
        {
            var view = new VisualizationControl();

            view.DataContext = viewModel;

            return view;
        }

        public static object Show( VisualizationWindowViewModel viewModel )
        {
            Initialize();
            VisualizationWindow view = null;
            
            try
            {
                Dispatcher.Invoke(
                    (Action) (() =>
                        {
                            view = new VisualizationWindow();

                            var oxyModel = new OxyPlotVisualizationViewModel(viewModel.Visualization);
                            viewModel.Visualization = oxyModel;

                            viewModel.Dispatcher = Dispatcher;
                            view.DataContext = viewModel;
                            view.Show();

                            view.Closing += OnViewClosing;

                            if (_keepApplicationAlive)
                            {
                                Interlocked.Increment(ref _windowCount);
                            }
                        }));

            }
            catch
            {
                
            }

            return view;
        }

        private static void OnViewClosing(object sender, CancelEventArgs e)
        {
            var view = sender as VisualizationWindow;
            var viewModel = view.DataContext as VisualizationWindowViewModel;
            viewModel.Visualization.Dispose();

            if (_keepApplicationAlive)
            {
                Interlocked.Decrement(ref _windowCount);
            }
        }


        static void RunApplication( object o )
        {
            try
            {
                _keepApplicationAlive = true;
                _application = new Application(); 
            
                var wait = o as ManualResetEvent;
                InitializeSeeShellApplication();
                wait.Set();

                _application.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                _application.Run();
                _application = null;

            }
            catch (Exception e)
            {
                var msg = "An unhandled exception has occurred in SeeShell on the main user interface thread.";
                Log.Error(msg, e);
                IssueReporter.ReportIssue(e.ToString(), null, msg);
                throw;
            }
        }

        private static void InitializeSeeShellApplication()
        {
            Log.Debug("bootstrapping visualization assemblies");
            BootstrapVisualizationAssemblies();

            //Application.ResourceAssembly = Assembly.GetExecutingAssembly();
            LoadApplicationResources();
        }

        public static void LoadApplicationResources()
        {
            Uri uri = new Uri("pack://application:,,,/CodeOwls.SeeShell.Visualizations;component/DataTemplates.xaml");
            

            try
            {
                var info = Application.GetResourceStream(uri);
                var reader = new XamlReader();

                var resources = (ResourceDictionary) reader.LoadAsync(info.Stream);

                Application.Current.Resources.MergedDictionaries.Add(resources);
            }
            catch
            {
            }
        }
    }
}
