namespace XmlExplorerDemo.Interfaces
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using MLib.Interfaces;
    using Settings.Interfaces;
    using Settings.UserProfile;
    using XmlExplorerVMLib.Interfaces;

    internal interface IAppViewModel
    {
        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        event EventHandler RequestClose;

        #region properties
        /// <summary>
        /// Gets an object that Implements application life cycle relevant properties
        /// and methods, such as: state for shutdown, shutdown_cancel,
        /// command for shutdown, and methods for save and load application configuration.
        /// </summary>
        IAppLifeCycleViewModel AppLifeCycle { get; }

        /// <summary>
        /// Command executes when the user has selected
        /// a different UI theme to display.
        /// 
        /// Command Parameter is the <seealso cref="ThemeDefinitionViewModel"/> object
        /// that should be selected next. This object can be handed over as:
        /// 1> an object[] array at object[0] or as simple object
        /// 2> <seealso cref="ThemeDefinitionViewModel"/> p
        /// </summary>
        ICommand ThemeSelectionChangedCommand { get; }

        /// <summary>
        /// Gets the currently selected application theme object.
        /// </summary>
        IThemesManagerViewModel AppTheme { get; }

        /// <summary>
        /// Gets the demo viewmodel and all its properties and commands
        /// </summary>
        IDocumentViewModel XmlDoc { get; }
        #endregion properties

        #region methods
        void GetSessionData(IProfile sessionData, IViewSize window);

        void SetSessionData(IProfile sessionData, IViewSize window);

        /// <summary>
        /// Standard dispose method of the <seealso cref="IDisposable" /> interface.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Call this to initialize application specific items that should be initialized
        /// before loading and display of mainWindow.
        /// 
        /// Invocation of This method is REQUIRED if UI is used in this application instance.
        /// 
        /// Method should not be called after <seealso cref="InitWithoutMainWindow"/>
        /// </summary>
        void InitForMainWindow(IAppearanceManager appearance, string themeDisplayName);

        /// <summary>
        /// Call this method if you want to initialize a headless
        /// (command line) application. This method will initialize only
        /// Non-WPF related items.
        /// 
        /// Method should not be called after <seealso cref="InitForMainWindow"/>
        /// </summary>
        void InitWithoutMainWindow();

        /// <summary>
        /// Save session data on closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClosing(object sender,
                       System.ComponentModel.CancelEventArgs e);

        /// <summary>
        /// Execute closing function and persist session data to be reloaded on next restart
        /// </summary>
        /// <param name="win"></param>
        void OnClosed(Window window);
        #endregion methods
    }
}