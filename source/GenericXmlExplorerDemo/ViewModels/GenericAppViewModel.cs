namespace GenericXmlExplorerDemo.ViewModels
{
    using GenericXmlExplorerDemo.Interfaces;
    using GenericXmlExplorerDemo.ViewModels.Base;
    using Microsoft.Win32;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using XmlExplorerVMLib.Interfaces;

    internal class GenericAppViewModel : Base.BaseViewModel, IGenericAppViewModel
    {
        #region fields
        private readonly XmlExplorerVMLib.Interfaces.IDocumentViewModel _demo;
        private ICommand _LoadXMLFileCommand;
        private ICommand _SaveAsXmlFileCommand;
        private ICommand _ApplicationExitCommand;
        private bool? _DialogCloseResult;
        private bool _ShutDownInProgress = false;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public GenericAppViewModel(IDocumentViewModel demoAooViewModel)
            : this()
        {
            _demo = demoAooViewModel;
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        protected GenericAppViewModel()
        {

        }
        #endregion ctors

        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event EventHandler RequestClose;

        #region properties
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

                        OpenfileWithDialog(file, Demo.CurrentXmlFile);
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


        private void SaveWithDialog_Executed()
        {
            if (Demo.SaveXml_CanExecut() == false)
                return;

            var dlg = new SaveFileDialog();
            dlg.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";

            dlg.FileName = Demo.CurrentXmlFile;

            if (dlg.ShowDialog() != true)
                return;

            Demo.SaveXml(dlg.FileName);
        }
        #endregion methods
    }
}
