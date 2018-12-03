namespace XmlExplorerVMLib.ViewModels
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.XPath;
    using XmlExplorerLib.interfaces;

    internal class XPathNavigatorViewModel : Base.BaseViewModel, IXPathNavigator
    {
        #region fields
        private IEnumerable<IXPathNavigator> _children;
        private bool _isSelected;
        private bool _isExpanded;
        #endregion fields

        #region ctors
        /// <summary>
        /// Parameterized class constructor
        /// </summary>
        /// <param name="navigator"></param>
        public XPathNavigatorViewModel(XPathNavigator navigator)
            : this()
        {
            this.XPathNavigator = navigator;
        }

        /// <summary>
        /// Hidden class constructor
        /// </summary>
        /// <param name="navigator"></param>
        protected XPathNavigatorViewModel()
        {
        }
        #endregion ctors

        #region properties
        public XPathNavigator XPathNavigator { get; protected set; }

        public bool IsSelected
        {
            get { return _isSelected; }

            set
            {
                if (!_isSelected.Equals(value))
                {
                    _isSelected = value;
                    base.OnPropertyChanged("IsSelected");
                }
            }
        }

        public bool IsExpanded
        {
            get { return _isExpanded; }

            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    NotifyPropertyChanged(() => IsExpanded);
                }
            }
        }

        public string Name
        {
            get
            {
                return this.XPathNavigator.Name;
            }
        }

        public string Value
        {
            get
            {
                return this.XPathNavigator.Value;
            }
        }

        public IEnumerable<IXPathNavigator> Children
        {
            get
            {
                if (_children == null)
                {
                    List<IXPathNavigator> childNavigatorViews = new List<IXPathNavigator>();

                    foreach (XPathNavigator childNavigator in this.XPathNavigator.SelectChildren(XPathNodeType.All))
                    {
                        childNavigatorViews.Add(new XPathNavigatorViewModel(childNavigator));
                    }

                    _children = childNavigatorViews.ToArray();
                }

                return _children;
            }
        }

        public bool HasNamespace
        {
            get
            {
                if (!string.IsNullOrEmpty(this.XPathNavigator.Prefix))
                    return true;

                if (!string.IsNullOrEmpty(this.XPathNavigator.NamespaceURI))
                    return true;

                return false;
            }
        }

        public string XmlTag
        {
            get
            {
                var ret = GetXPathNavigatorFormattedXml(this.XPathNavigator);
                return ret;
            }
        }
        #endregion properties

        #region methods
        public void CollapseAll()
        {
            this.IsExpanded = false;

            foreach (var child in this.Children)
                child.CollapseAll();
        }

        public void ExpandAll()
        {
            this.IsExpanded = true;

            foreach (var child in this.Children)
                child.ExpandAll();
        }

        /// <summary>
        /// Gets the formated Xml that represents this Xml node and all its children.
        /// </summary>
        /// <param name="navigator"></param>
        /// <returns></returns>
        public string GetXPathNavigatorFormattedXml(XPathNavigator navigator)
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
                settings.CloseOutput = true;
                settings.NewLineOnAttributes = true;

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
        #endregion methods
    }
}
