namespace Settings.Interfaces
{
    public interface IAppCore
    {
        #region properties
        /// <summary>
        /// Get the name of the executing assembly (usually name of *.exe file)
        /// </summary>
        string AssemblyTitle { get; }

        //
        // Summary:
        //     Gets the path or UNC location of the loaded file that contains the manifest.
        //
        // Returns:
        //     The location of the loaded file that contains the manifest. If the loaded
        //     file was shadow-copied, the location is that of the file after being shadow-copied.
        //     If the assembly is loaded from a byte array, such as when using the System.Reflection.Assembly.Load(System.Byte[])
        //     method overload, the value returned is an empty string ("").
        string AssemblyEntryLocation { get; }

        /// <summary>
        /// Get a path to the directory where the user store his documents
        /// </summary>
        string MyDocumentsUserDir { get; }

        string Company { get; }

        string Application_Title { get; }

        /// <summary>
        /// Get a path to the directory where the application
        /// can persist/load user data on session exit and re-start.
        /// </summary>
        string DirAppData { get; }

        ////        /// <summary>
        ////        /// Get path and file name to application specific settings file
        ////        /// </summary>
        ////        public static string DirFileAppSettingsData
        ////        {
        ////            get
        ////            {
        ////                return System.IO.Path.Combine(AppCore.DirAppData,
        ////                                              string.Format(CultureInfo.InvariantCulture, "{0}.App.settings", AppCore.AssemblyTitle));
        ////            }
        ////        }

        /// <summary>
        /// Get path and file name to application specific session file
        /// </summary>
        string DirFileAppSessionData { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Create a dedicated directory to store program settings and session data
        /// </summary>
        /// <returns></returns>
        bool CreateAppDataFolder();
        #endregion methods
    }
}
