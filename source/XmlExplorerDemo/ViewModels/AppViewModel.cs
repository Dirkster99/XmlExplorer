namespace XmlExplorerDemo.ViewModels
{
    using Base;
    using MLib.Interfaces;
    using Settings.Interfaces;
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using XmlExplorerDemo.Interfaces;
    using XmlExplorerDemo.ViewModels.Themes;

    /// <summary>
    /// Main ViewModel vlass that manages session start-up, life span, and shutdown
    /// of the application.
    /// </summary>
    internal class AppViewModel : Base.BaseViewModel, IDisposable, IAppViewModel
    {
        #region private fields
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private bool mDisposed = false;

        private bool _isInitialized = false;       // application should be initialized through one method ONLY!
        private object _lockObject = new object(); // thread lock semaphore

        private ICommand _ThemeSelectionChangedCommand = null;

        private readonly IAppLifeCycleViewModel _AppLifeCycle = null;
        private readonly ISettingsManager _SettingsManager;
        private readonly IThemesManagerViewModel _AppTheme = null;
        private readonly XmlExplorerVMLib.Interfaces.IDemoAppViewModel _demo;
        #endregion private fields

        #region constructors
        /// <summary>
        /// Standard Constructor
        /// </summary>
        public AppViewModel(IAppLifeCycleViewModel lifecycle,
                            IThemesManagerViewModel themesManager,
                            ISettingsManager settingsManager,
                            XmlExplorerVMLib.Interfaces.IDemoAppViewModel demo)
            : this()
        {
            _AppLifeCycle = lifecycle;

            _SettingsManager = settingsManager;
            _AppTheme = themesManager;
            ApplicationThemes = themesManager;

            _demo = demo;
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected AppViewModel()
        {
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Gets an object that Implements application life cycle relevant properties
        /// and methods, such as: state for shutdown, shutdown_cancel,
        /// command for shutdown, and methods for save and load application configuration.
        /// </summary>
        public IAppLifeCycleViewModel AppLifeCycle
        {
            get
            {
                return _AppLifeCycle;
            }
        }

        /// <summary>
        /// Gets an instance of the current application theme manager.
        /// </summary>
        public IThemesManagerViewModel ApplicationThemes { get; }

        #region app theme
        /// <summary>
        /// Command executes when the user has selected
        /// a different UI theme to display.
        /// 
        /// Command Parameter is the <seealso cref="ThemeDefinitionViewModel"/> object
        /// that should be selected next. This object can be handed over as:
        /// 1> an object[] array at object[0] or as simple object
        /// 2> <seealso cref="ThemeDefinitionViewModel"/> p
        /// </summary>
        public ICommand ThemeSelectionChangedCommand
        {
            get
            {
                if (_ThemeSelectionChangedCommand == null)
                {
                    _ThemeSelectionChangedCommand = new RelayCommand<object>((p) =>
                    {
                        if (this.mDisposed == true)
                            return;

                        string newThemeName = p as string;

                        if (newThemeName == null)
                            return;

                        try
                        {
                            var oldTheme = ApplicationThemes.DefaultTheme;

                            // The Work to perform on another thread
                            ThreadStart start = delegate
                            {
                                // This works in the UI tread using the dispatcher with highest Priority
                                Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Send,
                                (Action)(() =>
                                {
                                    try
                                    {
                                        if (ApplicationThemes.ApplyTheme(newThemeName) == false)
                                            return;

                                        _SettingsManager.Options.SetOptionValue("Appearance", "ThemeDisplayName", newThemeName);
//                                        ResetTheme();                        // Initialize theme in process
                                    }
                                    catch (Exception exp)
                                    {
                                        logger.Error(exp.Message, exp);
                                    }
                                }));
                            };

                            // Create the thread and kick it started!
                            Thread thread = new Thread(start);

                            thread.Start();
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    });
                }

                return _ThemeSelectionChangedCommand;
            }
        }

        /// <summary>
        /// Gets the currently selected application theme object.
        /// </summary>
        public IThemesManagerViewModel AppTheme
        {
            get { return _AppTheme; }
        }
        #endregion app theme

        /// <summary>
        /// Gets the demo viewmodel and all its properties and commands
        /// </summary>
        public XmlExplorerVMLib.Interfaces.IDemoAppViewModel Demo
        {
            get
            {
                return _demo;
            }
        }
        #endregion properties

        #region methods
        #region Get/set Session Application Data
        internal void GetSessionData(IProfile sessionData)
        {
/***
            if (sessionData.LastActiveTargetFile != TargetFile.FileName)
                sessionData.LastActiveTargetFile = TargetFile.FileName;

            sessionData.LastActiveSourceFiles = new List<SettingsModel.Models.FileReference>();
            if (SourceFiles != null)
            {
                foreach (var item in SourceFiles)
                    sessionData.LastActiveSourceFiles.Add(new SettingsModel.Models.FileReference()
                    { path = item.FileName }
                                                         );
            }
***/
        }

        internal void SetSessionData(IProfile sessionData)
        {
/***
            TargetFile.FileName = sessionData.LastActiveTargetFile;

            _SourceFiles = new ObservableCollection<FileInfoViewModel>();
            if (sessionData.LastActiveSourceFiles != null)
            {
                foreach (var item in sessionData.LastActiveSourceFiles)
                    _SourceFiles.Add(new FileInfoViewModel(item.path));
            }
***/
        }
        #endregion Get/set Session Application Data

        /// <summary>
        /// Call this method if you want to initialize a headless
        /// (command line) application. This method will initialize only
        /// Non-WPF related items.
        /// 
        /// Method should not be called after <seealso cref="InitForMainWindow"/>
        /// </summary>
        public void InitWithoutMainWindow()
        {
            lock (_lockObject)
            {
                if (_isInitialized == true)
                    throw new Exception("AppViewModel initizialized twice.");

                _isInitialized = true;
            }
        }

        /// <summary>
        /// Call this to initialize application specific items that should be initialized
        /// before loading and display of mainWindow.
        /// 
        /// Invocation of This method is REQUIRED if UI is used in this application instance.
        /// 
        /// Method should not be called after <seealso cref="InitWithoutMainWindow"/>
        /// </summary>
        public void InitForMainWindow(IAppearanceManager appearance
                                      , string themeDisplayName)
        {
            // Initialize base that does not require UI
            InitWithoutMainWindow();

            appearance.AccentColorChanged += Appearance_AccentColorChanged;

            // Initialize UI specific stuff here
            this.AppTheme.ApplyTheme(themeDisplayName);
        }

        /// <summary>
        /// Standard dispose method of the <seealso cref="IDisposable" /> interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed == false)
            {
                if (disposing == true)
                {
                    // Dispose of the curently displayed content
                    ////mContent.Dispose();
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            mDisposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            ////base.Dispose(disposing);
        }

        /// <summary>
        /// Method is invoked when theme manager is asked
        /// to change the accent color and has actually changed it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Appearance_AccentColorChanged(object sender, MLib.Events.ColorChangedEventArgs e)
        {

        }
        #endregion methods
    }
}
