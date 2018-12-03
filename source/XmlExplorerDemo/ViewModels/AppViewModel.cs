namespace XmlExplorerDemo.ViewModels
{
    using Base;
    using Microsoft.Win32;
    using MLib.Interfaces;
    using Settings.Interfaces;
    using Settings.UserProfile;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using UnitComboLib.Models.Unit;
    using UnitComboLib.Models.Unit.Screen;
    using UnitComboLib.Models;
    using UnitComboLib.ViewModels;
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
        private ICommand _LoadXMLFileCommand;
        private ICommand _SaveAsXmlFileCommand;
        private ICommand _NavigateUriCommand;

        private bool _ShutDownInProgress = false;
        private bool? _DialogCloseResult = null;
        private ICommand _ApplicationExitCommand;
        private readonly IAppLifeCycleViewModel _AppLifeCycle = null;
        private readonly ISettingsManager _SettingsManager;
        private readonly IThemesManagerViewModel _AppTheme = null;
        private readonly XmlExplorerVMLib.Interfaces.IDocumentViewModel _demo;
        #endregion private fields

        #region constructors
        /// <summary>
        /// Standard Constructor
        /// </summary>
        public AppViewModel(IAppLifeCycleViewModel lifecycle,
                            IThemesManagerViewModel themesManager,
                            ISettingsManager settingsManager,
                            XmlExplorerVMLib.Interfaces.IDocumentViewModel demo)
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
            MRU = new MRUViewModel();
            this.SizeUnitLabel = UnitComboLib.UnitViewModeService.CreateInstance(
                this.GenerateScreenUnitList(), new ScreenConverter(),
                1,       // Default Unit 0 Percent, 1 ScreeFontPoints
                12      // Default Value
                , "###" // Placeholder to measure for maximum expected string len in textbox
            );
        }
        #endregion constructors

        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        #region properties
        /// <summary>
        /// Gets the MRU Viewmodel which drives the most recent list entries.
        /// </summary>
        public MRUViewModel MRU { get; }

        /// <summary>
        /// Gets the Unit ViewModel that drives
        /// the <seealso cref="UnitComboBox"/> control.
        /// </summary>
        public IUnitViewModel SizeUnitLabel { get; }

        /// <summary>
        /// Gets a command to load an XML file.
        /// </summary>
        public ICommand LoadXMLFileCommand
        {
            get
            {
                if (_LoadXMLFileCommand == null)
                {
                    _LoadXMLFileCommand = new RelayCommand<object>((p) =>
                    {
                        var file = Demo.CurrentXmlFile;
                        if (file == null)
                        {
                            // Gets the assembly entry location
                            var appDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                            file = appDir + @"/00_DataSamples/XmlDataSampleDemo.xml";
                        }

                        var newfile = OpenfileWithDialog(file, Demo.CurrentXmlFile);
                        if (string.IsNullOrEmpty(newfile) == false)
                            MRU.List.UpdateEntry(newfile);
                    });
                }

                return _LoadXMLFileCommand;
            }
        }

        /// <summary>
        /// Gets a command to save an XML file with formatting in a different location.
        /// </summary>
        public ICommand SaveAsXmlFileCommand
        {
            get
            {
                if (_SaveAsXmlFileCommand == null)
                {
                    _SaveAsXmlFileCommand = new RelayCommand<object>((p) =>
                    {
                        SaveWithDialog_Executed();
                    },
                    p => Demo.SaveXml_CanExecut());
                }

                return _SaveAsXmlFileCommand;
            }
        }

        private void SaveWithDialog_Executed()
        {
            if (Demo.SaveXml_CanExecut() == false)
                return;

            var dlg = new SaveFileDialog();
            dlg.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";

            dlg.FileName = Demo.CurrentXmlFile;

            if (dlg.ShowDialog() != true)
                return;

            if (Demo.SaveXml(dlg.FileName) == true)
            {
                MRU.List.UpdateEntry(dlg.FileName);
            }
        }

        /// <summary>
        /// Gets a command that opens an Xml file supplied as parameter
        /// and navigates the application towards this file.
        /// </summary>
        public ICommand NavigateUriCommand
        {
            get
            {
                if (_NavigateUriCommand == null)
                {
                    _NavigateUriCommand = new RelayCommand<object>((p) =>
                    {
                        var param = p as string;

                        if (string.IsNullOrEmpty(param))
                            return;

                        if (Demo.FileOpenXml(param) == true)
                            MRU.List.UpdateEntry(param);
                    });
                }

                return _NavigateUriCommand;
            }
        }

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
        public XmlExplorerVMLib.Interfaces.IDocumentViewModel Demo
        {
            get
            {
                return _demo;
            }
        }

        /// <summary>
        /// Gets a command that will collapse all currently visible XML nodes (if any).
        /// </summary>
        public ICommand ApplicationExitCommand
        {
            get
            {
                if (_ApplicationExitCommand == null)
                {
                    _ApplicationExitCommand = new RelayCommand<object>(
                        (p) => AppExit_CommandExecuted(),
                        (p) => Demo.Closing_CanExecute());
                }

                return _ApplicationExitCommand;
            }
        }

        /// <summary>
        /// This can be used to close the attached view via ViewModel
        /// 
        /// Source: http://stackoverflow.com/questions/501886/wpf-mvvm-newbie-how-should-the-viewmodel-close-the-form
        /// </summary>
        public bool? DialogCloseResult
        {
            get { return _DialogCloseResult; }

            private set
            {
                if (_DialogCloseResult == value)
                    return;

                _DialogCloseResult = value;
                NotifyPropertyChanged(() => DialogCloseResult);
            }
        }
        #endregion properties

        #region methods
        #region Get/set Session Application Data
        /// <summary>
        /// Method is called upon application exit to store session
        /// relevant information in persistance and restore on next start-up.
        /// </summary>
        /// <param name="sessionData"></param>
        /// <param name="window"></param>
        public void GetSessionData(IProfile sessionData, IViewSize window)
        {
            // Store session data from actual objects
            ViewPosSizeModel winModel = null;
            sessionData.WindowPosSz.TryGetValue(sessionData.MainWindowName, out winModel);

            winModel.Height = window.Height;
            winModel.Width = window.Width;
            winModel.X = window.Left;
            winModel.Y = window.Top;

            if (window.WindowState == WindowState.Maximized)
                winModel.IsMaximized = true;
            else
                winModel.IsMaximized = false;

            MRU.WriteMruToSession(sessionData);

            sessionData.FontSizeScreenPoints = SizeUnitLabel.ScreenPoints;
        }

        /// <summary>
        /// Method is called upon application start-up to retrieve session
        /// relevant information from persistance and restore them (or defaults)
        /// into their target objects properties.
        /// </summary>
        /// <param name="sessionData"></param>
        /// <param name="window"></param>
        public void SetSessionData(IProfile sessionData, IViewSize window)
        {
            ViewPosSizeModel winModel = null;
            sessionData.WindowPosSz.TryGetValue(sessionData.MainWindowName, out winModel);

            window.Height = winModel.Height;
            window.Width = winModel.Width;
            window.Left = winModel.X;
            window.Top = winModel.Y;

            if (winModel.IsMaximized == true)
                window.WindowState = WindowState.Maximized;
            else
                window.WindowState = WindowState.Normal;

            MRU.ReadMruFromSession(sessionData);

            if (SizeUnitLabel.ScreenPoints >= 6 && SizeUnitLabel.ScreenPoints <= 200)
                SizeUnitLabel.ScreenPoints = sessionData.FontSizeScreenPoints;
            else
                SizeUnitLabel.ScreenPoints = 12;
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
                                     ,string themeDisplayName)
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

        /// <summary>
        /// Save session data on closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnClosing(object sender,
                              System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                ////if (Exit_CheckConditions(sender))      // Close all open files and check whether application is ready to close
                ////{
                ////    OnRequestClose();                  // (other than exception and error handling)
                ////
                e.Cancel = false;
                ////    //if (wsVM != null)
                ////    //  wsVM.SaveConfigOnAppClosed(); // Save application layout
                ////}
                ////else
                ////    e.Cancel = ShutDownInProgressCancel = true;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Execute closing function and persist session data to be reloaded on next restart
        /// </summary>
        /// <param name="win"></param>
        public void OnClosed(Window win)
        {
            try
            {
                GetSessionData(_SettingsManager.SessionData, win as IViewSize);

                // Save/initialize program options that determine global programm behaviour
                _AppLifeCycle.SaveConfigOnAppClosed(win as IViewSize);
                ////
                ////DisposeResources();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Method to be executed when user (or program) tries to close the application
        /// </summary>
        public void OnRequestClose()
        {
            try
            {
                if (_ShutDownInProgress)
                    return;

                if (DialogCloseResult == null)
                    DialogCloseResult = true;      // Execute Close event via attached property

                ////if (_ShutDownInProgressCancel)
                ////{
                ////    _ShutDownInProgress = false;
                ////    _ShutDownInProgressCancel = false;
                ////    DialogCloseResult = null;
                ////}
                ////else
                {
                    _ShutDownInProgress = true;

                    CommandManager.InvalidateRequerySuggested();
                    RequestClose?.Invoke(this, EventArgs.Empty);
                }
            }
            catch { }
        }

        private string OpenfileWithDialog(string file, string currentfile)
        {
            var dlg = new OpenFileDialog();
            dlg.FileName = file;
            dlg.Filter = "Extensible Markup Language (*.xml)|*.xml"
                       + "|Extensible Application Markup Language (*.xaml)|*.xaml"
                       + "|All files (*.*)|*.*";
            dlg.Multiselect = false;

            try
            {
                if (string.IsNullOrEmpty(file) == false)
                {
                    dlg.InitialDirectory = Path.GetDirectoryName(file);
                    dlg.FileName = Path.GetFileName(file);
                }
                else
                    dlg.InitialDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            catch (Exception)
            {
                dlg.InitialDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }

            if (dlg.ShowDialog().GetValueOrDefault() != true)
                return currentfile;

            file = dlg.FileName;

            try
            {
                if (Demo.FileOpenXml(file) == true)
                    return file;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "An unexpected error occured");
            }

            return null;
        }

        private void AppExit_CommandExecuted()
        {
            try
            {
                if (Demo.Closing_CanExecute() == false)
                    return;

                ////_ShutDownInProgressCancel = false;
                OnRequestClose();
            }
            catch
            {
            }
        }

        private IList<ListItem> GenerateScreenUnitList()
        {
            IList<ListItem> unitList = new List<ListItem>();

            var percentDefaults = new ObservableCollection<string>() { "25", "50", "75", "100", "125", "150", "175", "200", "300", "400", "500" };
            var pointsDefaults = new ObservableCollection<string>() { "3", "6", "8", "9", "10", "12", "14", "16", "18", "20", "24", "26", "32", "48", "60" };

            unitList.Add(new UnitComboLib.Models.ListItem(Itemkey.ScreenPercent, UnitComboLib.Local.Strings.Percent_String, UnitComboLib.Local.Strings.Percent_String_Short, percentDefaults));
            unitList.Add(new UnitComboLib.Models.ListItem(Itemkey.ScreenFontPoints, UnitComboLib.Local.Strings.Point_String, UnitComboLib.Local.Strings.Point_String_Short, pointsDefaults));

            return unitList;
        }
        #endregion methods
    }
}
