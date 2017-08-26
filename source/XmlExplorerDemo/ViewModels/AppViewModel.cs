namespace XmlExplorerDemo.ViewModels
{
    using XmlExplorerDemo.ViewModels.Base;
    using XmlExplorerDemo.ViewModels.XML;
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;

    internal class AppViewModel : Base.BaseViewModel
    {
        readonly XPathNavigatorTreeView _xmlTree = null;
        private ICommand _LoadXMLFileCommand;

        public AppViewModel()
        {
            _xmlTree = new XPathNavigatorTreeView();
        }

        public object XmlTree
        {
            get
            {
                return _xmlTree;
            }
        }

        public ICommand LoadXMLFileCommand
        {
            get
            {
                if (_LoadXMLFileCommand == null)
                {
                    _LoadXMLFileCommand = new RelayCommand<object>((p) =>
                    {
                        // Gets the assembly entry location
                        var appDir = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                        var file = appDir + @"/00_DataSamples/XmlDataSampleDemo.xml";

                        _xmlTree.FileOpen(file, OnDocumentLoaded, null);
                    });
                }

                return _LoadXMLFileCommand;
            }
        }

        void OnDocumentLoaded(object sender, EventArgs e)
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
    }
}

