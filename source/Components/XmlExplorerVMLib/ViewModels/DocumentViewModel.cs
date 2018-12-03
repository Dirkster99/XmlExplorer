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
    using XmlExplorerLib.interfaces;
    using System.Xml.XPath;
    using System.Xml;
    using System.Text;
    using System.Linq;

    /// <summary>
    /// Implements a viewmodel that keeps and manages all core states relevant to
    /// the appliaction. This viewmodel is typically bound to the MainWindow.
    /// </summary>
    internal class DocumentViewModel : Base.BaseViewModel, IDocumentViewModel
    {
        #region fields
        readonly XPathNavigatorTreeViewModel _xmlTree = null;
        private string _currentXmlFile = null;

        private ICommand _ExpandAllNodesCommand;
        private ICommand _CollapseAllNodesCommand;

        private XPathNavigatorViewModel _SelectedObject;
        private ICommand _SelectedItemChangedCommand;
        private ICommand _ExpandAllItemsHereCommand;
        private ICommand _CollapseAllNodesHereCommand;
        private ICommand _CopyXPathCommand;
        private ICommand _CopyXMlTagCommand;
        private ICommand _CopyXMlCommand;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public DocumentViewModel()
        {
            _xmlTree = new XPathNavigatorTreeViewModel();
        }
        #endregion ctors

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
        /// Gets a command to process a change in the selected item in the Xml tree.
        /// The new selected item is expected as a parameter of this command.
        /// </summary>
        public ICommand SelectedItemChangedCommand
        {
            get
            {
                if (_SelectedItemChangedCommand == null)
                {
                    _SelectedItemChangedCommand = new RelayCommand<object>((p) =>
                    {
                        var param = p as XPathNavigatorViewModel;

                        if (param != null)
                            SelectedObject = param;
                    });
                }

                return _SelectedItemChangedCommand;
            }
        }

        /// <summary>
        /// Gets the currently selected item (or null) in this viewmodel.
        /// </summary>
        public XPathNavigatorViewModel SelectedObject
        {
            set
            {
                if (_SelectedObject != value)
                {
                    _SelectedObject = value;
                    NotifyPropertyChanged(() => _SelectedObject);
                }
            }

            get
            {
                return _SelectedObject;
            }
        }

        /// <summary>
        /// Gets the complete path of the currently open file.
        /// </summary>
        public string CurrentXmlFile
        {
            get
            {
                return _currentXmlFile;
            }

            protected set
            {
                if (_currentXmlFile != value)
                {
                    _currentXmlFile = value;
                    NotifyPropertyChanged(() => CurrentXmlFile);
                    NotifyPropertyChanged(() => CurrentXmlFileName);
                }
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
        /// Gets a command that writes the XMl TAG text of a given
        /// <see cref="IXPathNavigator"/> node and its child into the Winodws Clipboard.
        /// 
        /// The <see cref="IXPathNavigator"/> node should be supplied as parameter,
        /// into the Window clipboard.
        /// </summary>
        public ICommand CopyXMlCommand
        {
            get
            {
                if (_CopyXMlCommand == null)
                {
                    _CopyXMlCommand = new RelayCommand<object>((p) =>
                    {
                        var param = p as IXPathNavigator;

                        if (param == null)
                            return;

                        string text = this.GetXPathNavigatorFormattedOuterXml(param.XPathNavigator);
                        Clipboard.SetText(text);

                    }, (p) =>
                    {
                        if (_xmlTree == null)
                            return false;

                        if (_xmlTree.IsLoading == true)
                            return false;

                        var param = p as IXPathNavigator;

                        if (param != null)
                            return true;

                        return false;
                    });
                }

                return _CopyXMlCommand;
            }
        }

        /// <summary>
        /// Gets a command that writes the XMl Tag text of a given
        /// <see cref="IXPathNavigator"/> node, which should be supplied as parameter,
        /// into the Window clipboard.
        /// </summary>
        public ICommand CopyXMlTagCommand
        {
            get
            {
                if (_CopyXMlTagCommand == null)
                {
                    _CopyXMlTagCommand = new RelayCommand<object>((p) =>
                    {
                        var param = p as IXPathNavigator;

                        if (param == null)
                            return;

                        string xmlText = param.GetXPathNavigatorFormattedXml(param.XPathNavigator);
                        Clipboard.SetText(xmlText);

                    }, (p) =>
                    {
                        if (_xmlTree == null)
                            return false;

                        if (_xmlTree.IsLoading == true)
                            return false;

                        var param = p as IXPathNavigator;

                        if (param != null)
                            return true;

                        return false;
                    });
                }

                return _CopyXMlTagCommand;
            }
        }

        /// <summary>
        /// Gets a command that writes the XPath of a given
        /// <see cref="IXPathNavigator"/> node, which should be supplied as parameter,
        /// into the Window clipboard.
        /// </summary>
        public ICommand CopyXPathCommand
        {
            get
            {
                if (_CopyXPathCommand == null)
                {
                    _CopyXPathCommand = new RelayCommand<object>((p) =>
                    {
                        var param = p as IXPathNavigator;

                        if (param == null)
                            return;

                        string xpath = param.GetXmlNodeFullPath(param.XPathNavigator);
                        Clipboard.SetText(xpath);

                    }, (p) =>
                    {
                        if (_xmlTree == null)
                            return false;

                        if (_xmlTree.IsLoading == true)
                            return false;

                        var param = p as IXPathNavigator;

                        if (param != null)
                            return true;

                        return false;
                    });
                }

                return _CopyXPathCommand;
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
                        if (_xmlTree == null)
                            return false;

                        if (_xmlTree.IsLoading == true)
                            return false;

                        if (_xmlTree.XPathRoot.Count() == 0)
                            return false;

                        return true;
                    });
                }

                return _ExpandAllNodesCommand;
            }
        }

        /// <summary>
        /// Gets a command that expand all Xml Nodes underneath a given
        /// <see cref="IXPathNavigator"/> node which should be supplied as parameter.
        /// </summary>
        public ICommand ExpandAllItemsHereCommand
        {
            get
            {
                if (_ExpandAllItemsHereCommand == null)
                {
                    _ExpandAllItemsHereCommand = new RelayCommand<object>((p) =>
                    {
                        var param = p as IXPathNavigator;

                        if (param == null)
                            return;

                        param.ExpandAll();
                    }, (p) =>
                    {
                        if (_xmlTree == null)
                            return false;

                        if (_xmlTree.IsLoading == true)
                            return false;

                        var param = p as IXPathNavigator;

                        if (param != null)
                            return true;

                        return false;
                    });
                }

                return _ExpandAllItemsHereCommand;
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
                        if (_xmlTree == null)
                            return false;

                        if (_xmlTree.IsLoading == true)
                            return false;

                        if (_xmlTree.XPathRoot.Count() == 0)
                            return false;

                        return true;
                    });
                }

                return _CollapseAllNodesCommand;
            }
        }

        /// <summary>
        /// Gets a command that collapses all Xml Nodes underneath a given
        /// <see cref="IXPathNavigator"/> node which should be supplied as parameter.
        /// </summary>
        public ICommand CollapseAllNodesHereCommand
        {
            get
            {
                if (_CollapseAllNodesHereCommand == null)
                {
                    _CollapseAllNodesHereCommand = new RelayCommand<object>((p) =>
                    {
                        var param = p as IXPathNavigator;

                        if (param == null)
                            return;

                        param.CollapseAll();

                    }, (p) =>
                    {
                        if (_xmlTree == null)
                            return false;

                        if (_xmlTree.IsLoading == true)
                            return false;

                        var param = p as IXPathNavigator;

                        if (param != null)
                            return true;

                        return false;
                    });
                }

                return _CollapseAllNodesHereCommand;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Method opens an XML file and attempts to load the XML
        /// into the internal viewmodel representation.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool FileOpenXml(string file)
        {
            try
            {
                _xmlTree.FileOpen(file, OnDocumentLoaded, null);
                CurrentXmlFile = file;
                return true;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "An unexpected error occured");
            }

            return false;
        }

        /// <summary>
        /// Determines whether a Save Xml command can currently performed or not.
        /// </summary>
        /// <returns></returns>
        public bool SaveXml_CanExecut()
        {
            if (_xmlTree == null)
                return false;

            if (_xmlTree.IsLoading == true)
                return false;

            if (_xmlTree.XPathRoot.Count() == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Method saves the current Xml content into an XML formated text file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool SaveXml(string file)
        {
            try
            {
                _xmlTree.Save(true, file);
                CurrentXmlFile = file;
                return true;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "An unexpected error occurred");
            }

            return false;
        }

        /// <summary>
        /// Determines whether the application can currently close without problems or not.
        /// </summary>
        /// <returns>
        /// True if application is ready to close otherwise, false.
        /// </returns>
        public bool Closing_CanExecute()
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

        private string GetXPathNavigatorFormattedOuterXml(XPathNavigator navigator)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings();

                settings.Encoding = Encoding.ASCII;
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;
                settings.ConformanceLevel = ConformanceLevel.Fragment;

                using (XmlWriter writer = XmlTextWriter.Create(stream, settings))
                {
                    navigator.WriteSubtree(writer);

                    writer.Flush();

                    return Encoding.ASCII.GetString(stream.ToArray());
                }
            }
        }
        #endregion methods
    }
}
