﻿namespace XmlExplorerVMLib.ViewModels.XML
{
    using XmlExplorerVMLib.Models;
    using XmlExplorerVMLib.Models.Events;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.XPath;
    using XmlExplorerLib.interfaces;

    /// <summary>
    /// Implements a treeview model that represents an Xml tree for binding to a WPF TreeView.
    /// </summary>
    internal class XPathNavigatorTreeViewModel : Base.BaseViewModel
    {
        #region fields
        private DateTime LoadingStarted;
        private bool _IsLoading = false;
        private object _Document;
        private readonly ObservableCollection<IXPathNavigator> _XPathRoot;
        #endregion fields

        #region constructors
        /// <summary>
        /// Class constructor
        /// </summary>
        public XPathNavigatorTreeViewModel()
        {
            Errors = new ObservableCollection<Error>();
            _XPathRoot = new ObservableCollection<IXPathNavigator>();
        }
        #endregion constructors

        #region properties
        public XmlNamespaceManager XmlNamespaceManager { get; private set; }

        public ObservableCollection<NamespaceDefinition> NamespaceDefinitions { get; private set; }
        public int DefaultNamespaceCount { get; private set; }

        public ObservableCollection<Error> Errors { get; private set; }

        /// <summary>
        /// Gets whether the document, if any, specifies schema information and can, therefore, be validated.
        /// </summary>
        public bool CanValidate { get; private set; }

        /// <summary>
        /// Gets the model of the Xml document tree nodes.
        /// </summary>
        public object Document
        {
            get
            {
                return _Document;
            }

            private set
            {
                if (_Document != value)
                {
                    _Document = value;
                    this.IsLoading = false;
                    this.LoadTime = DateTime.Now - this.LoadingStarted;

                    NotifyPropertyChanged(() => Document);
                }
            }
        }

        public IEnumerable<IXPathNavigator> XPathRoot
        {
            get
            {
                return _XPathRoot;
            }
        }

        public TimeSpan LoadTime { get; set; }

        public bool IsLoading
        {
            get { return _IsLoading; }
            set
            {
                if (_IsLoading != value)
                {
                    _IsLoading = value;
                    base.NotifyPropertyChanged(() => IsLoading);
                }
            }
        }

        public string FileName { get; private set; }
        #endregion properties

        #region methods
        public void FileOpen(
                  string filePathName,
                  EventHandler OnFinished,
                  EventHandler<EventArgs<Exception>> OnException)
        {
            this.FileName = filePathName;
            BeginOpen(new FileInfo(filePathName), OnFinished, OnException );
        }

        public void BeginOpen(
                  FileInfo fileInfo,
                  EventHandler OnFinished,
                  EventHandler<EventArgs<Exception>> OnException)
        {
            ClearErrorCollection();

            this.LoadingStarted = DateTime.Now;

            this.IsLoading = true;

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                    _XPathRoot.Clear();
            });

            ThreadStart start = delegate ()
            {
                XPathNavigator navigator = null;
                try
                {
                    navigator = OpenXPathNavigator(fileInfo);

                    this.LoadNamespaceDefinitions(navigator);
                    this.Validate(navigator);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);

                    this.AddError(ex.Message);

                    if (OnException != null)
                        OnException(this, new EventArgs<Exception>(ex));
                }
                finally
                {
                    Application.Current.Dispatcher.Invoke(
                        new Action<XPathNavigator>(SetDocument),
                        DispatcherPriority.Normal, navigator);

                    this.ConvertDocument(Document, _XPathRoot);

                    if (OnFinished != null)
                        OnFinished(this, EventArgs.Empty);
                }
            };
            new Thread(start).Start();
        }

        private void ConvertDocument(object document,
                                     ObservableCollection<IXPathNavigator> treeRoot)
        {
            XPathNavigator navigator;

            navigator = document as XPathNavigator;

            if (navigator == null)
                return;

            if (navigator == null)
                return;

            var root = new XPathNavigatorViewModel(navigator);

            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                foreach (var item in root.Children)
                    treeRoot.Add(item);
            });

            return;
        }

        public void SetDocument(XPathNavigator navigator)
        {
            try
            {
                this.Document = navigator;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        private static XPathNavigator OpenXPathNavigator(FileInfo fileInfo)
        {
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.ConformanceLevel = ConformanceLevel.Fragment;

            XPathDocument document = null;

            using (XmlReader reader = XmlReader.Create(fileInfo.FullName, readerSettings))
            {
                document = new XPathDocument(reader);
            }

            return document.CreateNavigator();
        }

        /// <summary>
        /// Saves a copy of the tree's XML file.
        /// </summary>
        public void Save(bool formatting, string fileName)
        {
            var fileInfo = new FileInfo(fileName);

            using (FileStream stream = new FileStream(fileInfo.FullName,
                                                      FileMode.Create, FileAccess.Write,
                                                      FileShare.None))
            {
                this.Save(stream, formatting);
            }

            FileName = fileName;
        }

        public void Save(Stream stream, bool formatting)
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.Encoding = Encoding.UTF8;
            settings.OmitXmlDeclaration = false;
            settings.Indent = formatting;

            using (XmlWriter writer = XmlTextWriter.Create(stream, settings))
            {
                XPathNavigator navigator = this.Document as XPathNavigator;
                if (navigator != null)
                {
                    navigator.WriteSubtree(writer);
                }
                else
                {
                    XPathNodeIterator iterator = this.Document as XPathNodeIterator;
                    if (iterator != null)
                    {
                        foreach (XPathNavigator node in iterator)
                        {
                            switch (node.NodeType)
                            {
                                case XPathNodeType.Attribute:
                                    writer.WriteString(node.Value);
                                    writer.WriteWhitespace(Environment.NewLine);
                                    break;

                                default:
                                    node.WriteSubtree(writer);
                                    if (node.NodeType == XPathNodeType.Text)
                                        writer.WriteWhitespace(Environment.NewLine);
                                    break;
                            }
                        }
                    }
                }

                writer.Flush();
            }
        }

        private void LoadNamespaceDefinitions(XPathNavigator navigator)
        {
            if (navigator == null)
            {
                this.XmlNamespaceManager = null;
                return;
            }

            this.XmlNamespaceManager = new XmlNamespaceManager(navigator.NameTable);

            XPathNodeIterator namespaces = navigator.Evaluate(@"//namespace::*[not(. = ../../namespace::*)]") as XPathNodeIterator;

            this.NamespaceDefinitions = new ObservableCollection<NamespaceDefinition>();

            this.DefaultNamespaceCount = 0;

            // add any namespaces within the scope of the navigator to the namespace manager
            foreach (var namespaceElement in namespaces)
            {
                XPathNavigator namespaceNavigator = namespaceElement as XPathNavigator;
                if (namespaceNavigator == null)
                    continue;

                NamespaceDefinition definition = new NamespaceDefinition()
                {
                    OldPrefix = namespaceNavigator.Name,
                    Namespace = namespaceNavigator.Value,
                };

                if (string.IsNullOrEmpty(definition.OldPrefix))
                {
                    if (DefaultNamespaceCount > 0)
                        definition.OldPrefix = "default" + (this.DefaultNamespaceCount + 1).ToString();
                    else
                        definition.OldPrefix = "default";

                    this.DefaultNamespaceCount++;
                }

                definition.NewPrefix = definition.OldPrefix;

                this.NamespaceDefinitions.Add(definition);

                this.XmlNamespaceManager.AddNamespace(definition.NewPrefix, definition.Namespace);
            }
        }
/***
        public XPathNavigatorView GetSelectedNavigatorView()
        {
            // get the selected node
            XPathNavigatorView navigator = this.SelectedItem as XPathNavigatorView;

            // if there is no selected node, default to the root node
            if (navigator == null && this.Items.Count > 0)
                return this.Items[0] as XPathNavigatorView;

            return navigator;
        }

        public void CopyOuterXml()
        {
            XPathNavigatorView navigatorView = this.GetSelectedNavigatorView();
            if (navigatorView == null)
                return;

            string text = this.GetXPathNavigatorFormattedOuterXml(navigatorView.XPathNavigator);

            Clipboard.SetText(text);
        }

        public void CopyXml()
        {
            XPathNavigatorView navigatorView = this.GetSelectedNavigatorView();
            if (navigatorView == null)
                return;

            string text = this.GetXPathNavigatorFormattedXml(navigatorView.XPathNavigator);

            Clipboard.SetText(text);
        }
***/
        private string GetXPathNavigatorFormattedXml(XPathNavigator navigator)
        {
            string outer = this.GetXPathNavigatorFormattedOuterXml(navigator);

            int index = outer.IndexOf(">") + 1;

            string xml = outer;

            if (index < xml.Length && index > 0)
                xml = xml.Remove(index);

            return xml;
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

        /// <summary>
        /// Returns a string representing the full path of an XPathNavigator.
        /// </summary>
        /// <param name="navigator">An XPathNavigator.</param>
        /// <returns></returns>
        public string GetXmlNodeFullPath(XPathNavigator navigator)
        {
            // create a StringBuilder for assembling the path
            StringBuilder sb = new StringBuilder();

            // clone the navigator (cursor), so the node doesn't lose it's place
            navigator = navigator.Clone();

            // traverse the navigator's ancestry all the way to the top
            while (navigator != null)
            {
                // skip anything but elements
                if (navigator.NodeType == XPathNodeType.Element)
                {
                    // insert the node and a seperator
                    sb.Insert(0, navigator.Name);
                    sb.Insert(0, "/");
                }
                if (!navigator.MoveToParent())
                    break;
            }

            return sb.ToString();
        }
/***
        public void ExpandAll()
        {
            XPathNavigatorView navigatorView = this.GetSelectedNavigatorView();
            if (navigatorView == null)
                return;

            navigatorView.ExpandAll();
        }

        public void CollapseAll()
        {
            XPathNavigatorView navigatorView = this.GetSelectedNavigatorView();
            if (navigatorView == null)
                return;

            navigatorView.CollapseAll();
        }

        public void SelectFirstResult(XPathNodeIterator iterator)
        {
            if (iterator == null || iterator.Count < 1)
                return;

            if (!iterator.MoveNext())
                return;

            // select the first node in the set
            XPathNavigator targetNavigator = iterator.Current;

            if (targetNavigator == null)
                return;

            this.SelectXPathNavigator(targetNavigator);
        }

        public void SelectXPathNavigator(XPathNavigator targetNavigator)
        {
            if (targetNavigator == null)
                throw new ArgumentNullException("targetNavigator");

            // we've found a node, so build an ancestor stack
            Stack<XPathNavigator> ancestors = this.GetAncestors(targetNavigator);

            IEnumerable items = this.Items;
            XPathNavigatorView targetView = null;

            while (ancestors.Count > 0)
            {
                XPathNavigator navigator = ancestors.Pop();

                foreach (object item in items)
                {
                    XPathNavigatorView view = item as XPathNavigatorView;
                    if (view == null)
                        return;

                    if (view.XPathNavigator.IsSamePosition(navigator))
                    {
                        if (ancestors.Count > 0)
                        {
                            view.IsExpanded = true;
                            items = view.Children;
                        }

                        targetView = view;
                        break;
                    }
                }
            }

            if (targetView != null)
            {
                XPathNavigatorView previousSelectedItem = this.SelectedItem as XPathNavigatorView;

                if (targetView.IsSelected && targetView != previousSelectedItem)
                    targetView.IsSelected = false;

                targetView.IsSelected = true;

                if (previousSelectedItem != null && previousSelectedItem != targetView && previousSelectedItem.IsSelected)
                {
                    previousSelectedItem.IsSelected = false;
                }
            }

            return;
        }
***/
        /// <summary>
        /// Builds and returns a Stack of XPathNavigator ancestors for a given XPathNavigator.
        /// </summary>
        /// <param name="navigator">The XPathNavigator from which to build a stack.</param>
        /// <returns></returns>
        private Stack<XPathNavigator> GetAncestors(XPathNavigator navigator)
        {
            if (navigator == null)
                throw new ArgumentNullException("navigator");

            Stack<XPathNavigator> ancestors = new Stack<XPathNavigator>();

            // navigate up the xml tree, building the stack as we go
            while (navigator != null)
            {
                // push the current ancestor onto the stack
                ancestors.Push(navigator);

                // clone the current navigator cursor, so we don't lose our place
                navigator = navigator.Clone();

                // if we've reached the top, we're done
                if (!navigator.MoveToParent())
                    break;

                // if we've reached the root, we're done
                if (navigator.NodeType == XPathNodeType.Root)
                    break;
            }

            // return the result
            return ancestors;
        }
/***
        public string CopyXPath()
        {
            XPathNavigatorView navigatorView = this.GetSelectedNavigatorView();

            string text = this.GetXmlNodeFullPath(navigatorView.XPathNavigator);

            Clipboard.SetText(text);

            return text;
        }

        public object EvaluateXPath(string xpath)
        {
            XPathNavigatorView navigatorView = this.GetSelectedNavigatorView();

            if (navigatorView == null)
                return null;

            // evaluate the expression, return the result
            return navigatorView.XPathNavigator.Evaluate(xpath, this.XmlNamespaceManager);
        }
***/
        /// <summary>
        /// Performs validation against any schemas provided in the document, if any.
        /// If validation is not possible, because no schemas are provided, an InvalidOperationException is thrown.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException" />
        public List<Error> Validate(XPathNavigator navigator)
        {
            try
            {
                this.CanValidate = false;

                ClearErrorCollection();

                if (navigator == null)
                    return null;

                string xsiPrefix = this.XmlNamespaceManager.LookupPrefix("http://www.w3.org/2001/XMLSchema-instance");

                if (string.IsNullOrEmpty(xsiPrefix))
                {
                    this.AddError("The document does not have a schema specified.", XmlSeverityType.Warning);
                    return this.Errors.ToList();
                }

                XmlSchemaSet schemas = new XmlSchemaSet();

                /*
				 * I can't believe I have to do this manually, but, here we go...
				 * 
				 * When loading a document with an XmlReader and performing validation, it will automatically
				 * detect schemas specified in the document with @xsi:schemaLocation and @xsi:noNamespaceSchemaLocation attributes.
				 * We get line number and column, but not a reference to the actual offending xml node or xpath navigator.
				 * 
				 * When validating an xpath navigator, it ignores these attributes, doesn't give us line number or column,
				 * but does give us the offending xpath navigator.
				 * 
				 * */
                foreach (var schemaAttribute in navigator.Select(
                    string.Format("//@{0}:noNamespaceSchemaLocation", xsiPrefix),
                    this.XmlNamespaceManager))
                {
                    XPathNavigator attributeNavigator = schemaAttribute as XPathNavigator;
                    if (attributeNavigator == null)
                        continue;

                    string value = attributeNavigator.Value;
                    value = this.ResolveSchemaFileName(value);

                    // add the schema
                    schemas.Add(null, value);
                }

                foreach (var schemaAttribute in navigator.Select(
                    string.Format("//@{0}:schemaLocation", xsiPrefix),
                    this.XmlNamespaceManager))
                {
                    XPathNavigator attributeNavigator = schemaAttribute as XPathNavigator;
                    if (attributeNavigator == null)
                        continue;

                    string value = attributeNavigator.Value;

                    List<KeyValuePair<string, string>> namespaceDefs = this.ParseNamespaceDefinitions(value);

                    foreach (var pair in namespaceDefs)
                        schemas.Add(pair.Key, pair.Value);
                }

                // validate the document
                navigator.CheckValidity(schemas, this.OnValidationEvent);

                this.CanValidate = true;
            }
            catch (FileNotFoundException ex)
            {
                string message = string.Format(
                    "Cannot find the schema document at '{0}'", ex.FileName);
                this.AddError(message);
            }
            catch (Exception ex)
            {
                this.AddError(ex.Message);
            }

            return this.Errors.ToList();
        }

        private List<KeyValuePair<string, string>> ParseNamespaceDefinitions(string value)
        {
            List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();

            // the value should be like: http://www.w3schools.com note.xsd
            // namespace path
            // split it on space
            string[] values = value.Split(" ".ToCharArray());

            for (int i = 0; i < values.Length; i += 2)
            {
                // first value is namespace (it's not supposed to have spaces in it)
                string targetNamespace = values[i];

                // second value should be the filename
                string schemaFileName = null;
                if (i < values.Length - 1)
                {
                    schemaFileName = values[i + 1];
                    schemaFileName = this.ResolveSchemaFileName(schemaFileName);
                }

                KeyValuePair<string, string> pair = new KeyValuePair<string, string>(targetNamespace, schemaFileName);

                pairs.Add(pair);
            }

            return pairs;
        }

        private string ResolveSchemaFileName(string schemaFileName)
        {
            if (!File.Exists(schemaFileName))
            {
                // try prepending the current file's path
                if (this.FileName != null)
                    schemaFileName = Path.Combine(Path.GetDirectoryName(FileName), schemaFileName);
            }

            return schemaFileName;
        }

        private void OnValidationEvent(object sender, ValidationEventArgs e)
        {
            Error error = new Error();
            error.Description = e.Message;
            error.Category = e.Severity;

            XmlSchemaValidationException exception = e.Exception as XmlSchemaValidationException;
            if (exception != null)
            {
                error.SourceObject = exception.SourceObject;
            }

            this.AddError(error);
        }

        private void AddError(string description)
        {
            this.AddError(description, XmlSeverityType.Error);
        }

        private void AddError(string description, XmlSeverityType category)
        {
            Error error = new Error();
            error.Description = description;
            error.Category = category;

            this.AddError(error);
        }

        private void AddError(Error error)
        {
            if (this.FileName == null)
                error.File = "Untitled";
            else
                error.File = this.FileName;

            error.DefaultOrder = this.Errors.Count;

            AddErrorIntoCollection(error);
        }

        private void ClearErrorCollection()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.Errors.Clear();
            }));
        }

        private void AddErrorIntoCollection(Error error)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.Errors.Add(error);
            }));
        }

        /***
                protected override void OnMouseUp(MouseButtonEventArgs e)
                {
                    try
                    {
                        base.OnMouseUp(e);

                        if (e.ChangedButton != MouseButton.Right)
                            return;

                        DependencyObject obj = this.InputHitTest(e.GetPosition(this)) as DependencyObject;
                        if (obj == null)
                            return;

                        // cycle up the tree until you hit a TreeViewItem   
                        while (obj != null && !(obj is TreeViewItem))
                        {
                            obj = VisualTreeHelper.GetParent(obj);
                        }

                        TreeViewItem item = obj as TreeViewItem;
                        if (item == null)
                            return;

                        XPathNavigatorView view = item.DataContext as XPathNavigatorView;
                        if (view == null)
                            return;

                        view.IsSelected = true;

                        ContextMenu contextMenu = this.Resources["treeContextMenu"] as ContextMenu;
                        if (contextMenu == null)
                            return;

                        contextMenu.PlacementTarget = item;
                        contextMenu.IsOpen = true;
                    }
                    catch (Exception ex)
                    {
                        App.HandleException(ex);
                    }
                }
        ***/
        #endregion methods
    }
}
