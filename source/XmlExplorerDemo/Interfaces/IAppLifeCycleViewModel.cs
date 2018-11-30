namespace XmlExplorerDemo.Interfaces
{
    using System.Windows.Input;
    using MLib.Interfaces;
    using Settings.Interfaces;
    using Settings.UserProfile;

    /// <summary>
    /// Implements application life cycle relevant properties and methods,
    /// such as: state for shutdown, shutdown_cancel, command for shutdown,
    /// and methods for save and load application configuration.
    /// </summary>
    internal interface IAppLifeCycleViewModel
    {
        #region properties
        /// <summary>
        /// Gets a string for display of the application title.
        /// </summary>
        string Application_Title { get; }

        /// <summary>
        /// Get path and file name to application specific settings file
        /// </summary>
        string DirFileAppSettingsData { get; }

        /// <summary>
        /// This can be used to close the attached view via ViewModel
        /// 
        /// Source: http://stackoverflow.com/questions/501886/wpf-mvvm-newbie-how-should-the-viewmodel-close-the-form
        /// </summary>
        bool? DialogCloseResult { get; }

        /// <summary>
        /// Gets a command to exit (end) the application.
        /// </summary>
        ICommand ExitApp { get; }

        bool ShutDownInProgress_Cancel { get; set; }
        #endregion properties

        #region methods

        #region Save Load Application configuration
        /// <summary>
        /// Save application settings when the application is being closed down
        /// </summary>
        void SaveConfigOnAppClosed(IViewSize win);

        /// <summary>
        /// Load configuration from persistence on startup of application
        /// </summary>
        void LoadConfigOnAppStartup(ISettingsManager settings
                                   ,IAppearanceManager appearance);
        #endregion Save Load Application configuration

        #region StartUp/ShutDown
        /// <summary>
        /// Check if pre-requisites for closing application are available.
        /// Save session data on closing and cancel closing process if necessary.
        /// </summary>
        /// <returns>true if application is OK to proceed closing with closed, otherwise false.</returns>
        bool Exit_CheckConditions(object sender);

        #region RequestClose [event]
        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        ////public event EventHandler ApplicationClosed;

        /// <summary>
        /// Method to be executed when user (or program) tries to close the application
        /// </summary>
        void OnRequestClose(bool ShutDownAfterClosing = true);

        void CancelShutDown();
        #endregion // RequestClose [event]
        #endregion StartUp/ShutDown
        #endregion methods
    }
}