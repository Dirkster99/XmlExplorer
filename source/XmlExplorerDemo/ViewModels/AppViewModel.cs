namespace XmlExplorerDemo.ViewModels
{
    using XmlExplorerDemo.ViewModels.Base;
    using XmlExplorerDemo.ViewModels.XML;
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Win32;
    using System.IO;
    using System.Xml.XPath;

    internal class AppViewModel : Base.BaseViewModel
    {
        #region fields
        readonly XPathNavigatorTreeViewModel _xmlTree = null;
        private string _currentXmlFile = null;

        private ICommand _LoadXMLFileCommand;
        private ICommand _ExpandAllNodesCommand;
        private ICommand _SaveAsXmlFileCommand;
        private ICommand _CollapseAllNodesCommand;
        #endregion fields

        #region ctors
        /// <summary>
        /// Class constructor
        /// </summary>
        public AppViewModel()
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
                            return;

                        file = dlg.FileName;

                        try
                        {
                            _xmlTree.FileOpen(file, OnDocumentLoaded, null);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show(exp.Message, "An unexpected error occured");
                        }

                        _currentXmlFile = file;
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

                    },(p) =>
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
        #endregion properties

        #region methods
        private void OnDocumentLoaded(object sender, EventArgs e)
        {
            try
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(new EventHandler(this.OnDocumentLoaded), sender, e);
                    return;
                }

///                var selectedItem = this.GetSelectedXPathDocumentContent();
///                if (selectedItem == null)
//                    return;

///               if (selectedItem.TreeView != sender)
///                    return;

///                this.LoadCurrentDocumentInformation();
            }
            catch // (Exception ex)
            {
///                App.HandleException(ex);
            }
        }
        #endregion methods
    }
}

