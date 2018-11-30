namespace XmlExplorerDemo.Models
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using XmlExplorerVMLib.Interfaces;

    /// <summary>
    /// Class supplies a set of common static helper methodes that help
    /// localizing application specific items such as setting folders etc.
    /// </summary>
    public class AppCore : IAppCore
    {
        #region properties
        /// <summary>
        /// Get the name of the executing assembly (usually name of *.exe file)
        /// </summary>
        public string AssemblyTitle
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Name;
            }
        }

        //
        // Summary:
        //     Gets the path or UNC location of the loaded file that contains the manifest.
        //
        // Returns:
        //     The location of the loaded file that contains the manifest. If the loaded
        //     file was shadow-copied, the location is that of the file after being shadow-copied.
        //     If the assembly is loaded from a byte array, such as when using the System.Reflection.Assembly.Load(System.Byte[])
        //     method overload, the value returned is an empty string ("").
        public string AssemblyEntryLocation
        {
            get
            {
                return System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
        }

        /// <summary>
        /// Get a path to the directory where the user store his documents
        /// </summary>
        public string MyDocumentsUserDir
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }

        public string Company
        {
            get
            {
                return "XmlExplorerDemo";
            }
        }

        public string Application_Title
        {
            get
            {
                return "ThemedDemo";
            }
        }

        /// <summary>
        /// Get a path to the directory where the application
        /// can persist/load user data on session exit and re-start.
        /// </summary>
        public string DirAppData
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                                 System.IO.Path.DirectorySeparatorChar +
                                                 Company;
            }
        }

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
        public string DirFileAppSessionData
        {
            get
            {
                return System.IO.Path.Combine(DirAppData,
                                              string.Format(CultureInfo.InvariantCulture, "{0}.App.session", AssemblyTitle));
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Create a dedicated directory to store program settings and session data
        /// </summary>
        /// <returns></returns>
        public bool CreateAppDataFolder()
        {
            try
            {
                if (System.IO.Directory.Exists(DirAppData) == false)
                    System.IO.Directory.CreateDirectory(DirAppData);
            }
            catch
            {
                return false;
            }

            return true;
        }
        #endregion methods
    }
}
