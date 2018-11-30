namespace XmlExplorerVMLib.ViewModels
{
    using XmlExplorerVMLib.ViewModels.Base;
    using XmlExplorerVMLib.ViewModels.XML;
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Win32;
    using System.IO;
    using XmlExplorerVMLib.Interfaces;

    /// <summary>
    /// Implements a viewmodel that keeps and manages all core states relevant to
    /// the appliaction. This viewmodel is typically bound to the MainWindow.
    /// </summary>
    internal class DemoAppViewModel : Base.BaseViewModel, IDemoAppViewModel
    {
        #region fields
        readonly XPathNavigatorTreeViewModel _xmlTree = null;
        private string _currentXmlFile = null;

        private ICommand _LoadXMLFileCommand;
        private ICommand _ExpandAllNodesCommand;
        private ICommand _SaveAsXmlFileCommand;
        private ICommand _CollapseAllNodesCommand;
        private ICommand _ApplicationExitCommand;

        private bool? _DialogCloseResult = null;
        private bool _ShutDownInProgress = false;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DemoAppViewModel()
        {
            _xmlTree = new XPathNavigatorTreeViewModel();
        }
        #endregion ctors

        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        #region properties
        /// <summary>
        /// Gets an object tree that represents the nodes of an XML tree.
        /// </summary>
        public object XmlTree
        {
            get
            {
                return _xmlTree;
            }
        }

        /// <summary>
        /// Gets the complete path of the currently open file.
        /// </summary>
        public string CurrentXmlFile
        {
            set
            {
                if (_currentXmlFile != value)
                {
                    _currentXmlFile = value;
                    NotifyPropertyChanged(() => CurrentXmlFile);
                    NotifyPropertyChanged(() => CurrentXmlFileName);
                }
            }

            get
            {
                return _currentXmlFile;
            }
        }

        /// <summary>
        /// Gets the complete path of the currently open file.
        /// </summary>
        public string CurrentXmlFileName
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(_currentXmlFile) == false)
                        return System.IO.Path.GetFileName( _currentXmlFile );
                }
                catch
                {
                }

                return string.Empty;
            }
        }

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
                        var file = _currentXmlFile;
                        if (file == null)
                        {
                            // Gets the assembly entry location
                            var appDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                            file = appDir + @"/00_DataSamples/XmlDataSampleDemo.xml";
                        }

                        CurrentXmlFile = OpenfileWithDialog(file, _currentXmlFile);
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
                        var dlg = new SaveFileDialog();
                        dlg.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";


                        dlg.FileName = _currentXmlFile;

                        if (dlg.ShowDialog() != true)
                            return;

                        if (_xmlTree == null)
                            return;

                        if (_xmlTree.IsLoading == true)
                            return;

                        try
                        {
                            _xmlTree.Save(true, dlg.FileName);
                            _currentXmlFile = dlg.FileName;
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show(exp.Message, "An unexpected error occurred");
                        }

                    }, (p) =>
                    {
                        if (_xmlTree != null)
                            return true;

                        if (_xmlTree.IsLoading == true)
                            return false;

                        return false;
                    });
                }

                return _SaveAsXmlFileCommand;
            }
        }

        /// <summary>
        /// Gets a command that will expand all currently visible XML nodes (if any).
        /// </summary>
        public ICommand ExpandAllNodesCommand
        {
            get
            {
                if (_ExpandAllNodesCommand == null)
                {
                    _ExpandAllNodesCommand = new RelayCommand<object>((p) =>
                    {
                        if (_xmlTree == null)
                            return;

                        if (_xmlTree.IsLoading == true)
                            return;

                        var root = _xmlTree.XPathRoot;

                        foreach (var item in root)
                        {
                            item.IsExpanded = true;
                            item.ExpandAll();
                        }

                    }, (p) =>
                    {
                        if (_xmlTree != null)
                            return true;

                        if (_xmlTree.IsLoading == true)
                            return false;

                        return false;
                    });
                }

                return _ExpandAllNodesCommand;
            }
        }

        /// <summary>
        /// Gets a command that will collapse all currently visible XML nodes (if any).
        /// </summary>
        public ICommand CollapseAllNodesCommand
        {
            get
            {
                if (_CollapseAllNodesCommand == null)
                {
                    _CollapseAllNodesCommand = new RelayCommand<object>((p) =>
                    {
                        if (_xmlTree == null)
                            return;

                        if (_xmlTree.IsLoading == true)
                            return;

                        var root = _xmlTree.XPathRoot;

                        foreach (var item in root)
                        {
                            item.IsExpanded = false;
                            item.CollapseAll();
                        }

                    }, (p) =>
                    {
                        if (_xmlTree != null)
                            return true;

                        if (_xmlTree.IsLoading == true)
                            return false;

                        return false;
                    });
                }

                return _CollapseAllNodesCommand;
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
                        (p) => Closing_CanExecute());
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
            catch{ }
        }

        /// <summary>
        /// Save session data on closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
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
                ////EnableMainWindowActivated(false);

                // Persist window position, width and height from this session
                ////_SettingsManager.SessionData.MainWindowPosSz =
                ////    SettingsFactory.GetViewPosition(win.Left, win.Top, win.Width, win.Height,
                ////                                    (win.WindowState == WindowState.Maximized));
                ////
                ////_SettingsManager.SessionData.IsWorkspaceAreaOptimized = IsWorkspaceAreaOptimized;
                ////
                ////// Save/initialize program options that determine global programm behaviour
                ////SaveConfigOnAppClosed();
                ////
                ////DisposeResources();
            }
            catch
            {
            }
        }

        private void AppExit_CommandExecuted()
        {
            try
            {
                if (Closing_CanExecute() == false)
                    return;

                ////_ShutDownInProgressCancel = false;
                OnRequestClose();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Determines whether the application can currently close without problems or not.
        /// </summary>
        /// <returns>
        /// True if application is ready to close otherwise, false.
        /// </returns>
        private bool Closing_CanExecute()
        {
            if (_xmlTree == null)
                return true;

            if (_xmlTree.IsLoading == true)
                return false;

            return true;
        }

        private void OnDocumentLoaded(object sender, EventArgs e)
        {
            try
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(new EventHandler(this.OnDocumentLoaded), sender, e);
                    return;
                }

                ///var selectedItem = this.GetSelectedXPathDocumentContent();
                ///if (selectedItem == null)
                //    return;

                ///f (selectedItem.TreeView != sender)
                ///    return;

                ///this.LoadCurrentDocumentInformation();
            }
            catch // (Exception ex)
            {
                /// App.HandleException(ex);
            }
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
                _xmlTree.FileOpen(file, OnDocumentLoaded, null);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "An unexpected error occured");
            }

            return file;
        }
        #endregion methods
    }
}
