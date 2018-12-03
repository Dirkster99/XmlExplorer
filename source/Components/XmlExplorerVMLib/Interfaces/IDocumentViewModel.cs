namespace XmlExplorerVMLib.Interfaces
{
    using System.Windows.Input;

    /// <summary>
    /// Defines an interface to a viewmodel object that keeps and manages all
    /// core states relevant to appliaction. This viewmodel is typically bound
    /// to the MainWindow.
    /// </summary>
    public interface IDocumentViewModel
    {
        #region properties
        /// <summary>
        /// Gets an object tree that represents the nodes of an XML tree.
        /// </summary>
        object XmlTree { get; }

        /// <summary>
        /// Gets the complete path of the currently open file.
        /// </summary>
        string CurrentXmlFile { get; }

        /// <summary>
        /// Gets a command that will expand all currently visible XML nodes (if any).
        /// </summary>
        ICommand ExpandAllNodesCommand { get; }

        /// <summary>
        /// Gets a command that will collapse all currently visible XML nodes (if any).
        /// </summary>
        ICommand CollapseAllNodesCommand { get; }
        #endregion properties

        #region methods

        /// <summary>
        /// Method opens an XML file and attempts to load the XML
        /// into the internal viewmodel representation.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        bool FileOpenXml(string file);

        /// <summary>
        /// Determines whether a Save Xml command can currently performed or not.
        /// </summary>
        /// <returns></returns>
        bool SaveXml_CanExecut();

        /// <summary>
        /// Method saves the current Xml content into an XML formated text file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        bool SaveXml(string file);

        /// <summary>
        /// Determines whether the application can currently close without problems or not.
        /// </summary>
        /// <returns>
        /// True if application is ready to close otherwise, false.
        /// </returns>
        bool Closing_CanExecute();
       #endregion methods
    }
}