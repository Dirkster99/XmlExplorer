namespace XmlExplorerVMLib.Interfaces
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Defines an interface to a viewmodel object that keeps and manages all
    /// core states relevant to appliaction. This viewmodel is typically bound
    /// to the MainWindow.
    /// </summary>
    public interface IDemoAppViewModel
    {
        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        event EventHandler RequestClose;

        #region properties
        /// <summary>
        /// Gets an object tree that represents the nodes of an XML tree.
        /// </summary>
        object XmlTree { get; }

        /// <summary>
        /// Gets a command to load an XML file.
        /// </summary>
        ICommand LoadXMLFileCommand { get; }

        /// <summary>
        /// Gets a command to save an XML file with formatting in a different location.
        /// </summary>
        ICommand SaveAsXmlFileCommand { get; }

        /// <summary>
        /// Gets a command that will expand all currently visible XML nodes (if any).
        /// </summary>
        ICommand ExpandAllNodesCommand { get; }

        /// <summary>
        /// Gets a command that will collapse all currently visible XML nodes (if any).
        /// </summary>
        ICommand CollapseAllNodesCommand { get; }

        /// <summary>
        /// Gets a command that will collapse all currently visible XML nodes (if any).
        /// </summary>
        ICommand ApplicationExitCommand { get; }

        /// <summary>
        /// This can be used to close the attached view via ViewModel
        /// 
        /// Source: http://stackoverflow.com/questions/501886/wpf-mvvm-newbie-how-should-the-viewmodel-close-the-form
        /// </summary>
        bool? DialogCloseResult { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Method to be executed when user (or program) tries to close the application
        /// </summary>
        void OnRequestClose();

        /// <summary>
        /// Save session data on closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClosing(object sender, System.ComponentModel.CancelEventArgs e);

        /// <summary>
        /// Execute closing function and persist session data to be reloaded on next restart
        /// </summary>
        /// <param name="win"></param>
        void OnClosed(Window win);
        #endregion methods
    }
}