namespace XmlExplorerDemo
{
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using log4net;
    using log4net.Config;
    using MLib.Interfaces;
    using Settings.Interfaces;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Threading;
    using XmlExplorerDemo.Interfaces;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region fields
        private IWindsorContainer _Container;
        protected static log4net.ILog Logger;
        #endregion fields

        #region constructors
        /// <summary>
        /// Static class constructor
        /// </summary>
        static App()
        {
            XmlConfigurator.Configure();
            Logger = LogManager.GetLogger("default");
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public App()
        {
            InitializeComponent();

            SessionEnding += App_SessionEnding;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }
        #endregion constructors

        #region methods
        /// <summary>
        /// Method executes as application entry point - that is -
        /// this bit of code executes before anything else in this
        /// class and application.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _Container = new WindsorContainer();

            // This allows castle to look at the current assembly and look for implementations
            // of the IWindsorInstaller interface
            _Container.Install(FromAssembly.This());                         // Register

            ISettingsManager settings = null;
            try
            {
                // Apply the selected theme (either default or reloaded from options)
                var themeManager = _Container.Resolve<IThemesManagerViewModel>();

                settings = _Container.Resolve<ISettingsManager>();
                var appearance = _Container.Resolve<IAppearanceManager>();

                var lifeCycle = _Container.Resolve<IAppLifeCycleViewModel>();
                lifeCycle.LoadConfigOnAppStartup(settings, appearance);

                themeManager.ApplyTheme(settings.Options.GetOptionValue<string>("Appearance", "ThemeDisplayName"));

                themeManager.ApplyTheme(themeManager.SelectedTheme.Model.Name);
            }
            catch (System.Exception exp)
            {
                Logger.Error(exp);
            }

            var window = new MainWindow();
            var appVM = _Container.Resolve<IAppViewModel>();
            appVM.SetSessionData(settings.SessionData, window);
            window.DataContext = appVM;

            // subscribe to close event messaging to application viewmodel
            window.Closing += appVM.OnClosing;

            window.Closed += delegate
            {
                // Save session data and close application
                appVM.OnClosed(window);
            };

            // When the ViewModel asks to be closed, close the window.
            // Source: http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
            appVM.RequestClose += delegate
            {
                // Save session data and close application
                appVM.OnClosed(window);
            };

            window.Show();
        }

        /// <summary>
        /// Handle unhandled exception here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string message = string.Empty;

            try
            {
                if (e.Exception != null)
                {
                    message = string.Format(CultureInfo.CurrentCulture, "{0}\n\n{1}", e.Exception.Message, e.Exception.ToString());
                }
                else
                    message = "An unknown error occurred.";

                MessageBox.Show(message, "An unexpected error occurred.");

                e.Handled = true;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Method executes event based when the user ends the Windows
        /// session by logging off or shutting down the operating system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
        }
        #endregion methods
    }
}
