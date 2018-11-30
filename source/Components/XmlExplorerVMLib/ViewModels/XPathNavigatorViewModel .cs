namespace XmlExplorerVMLib.ViewModels
{
    using System.Collections.Generic;
    using System.Xml.XPath;
    using XmlExplorerLib.interfaces;

    internal class XPathNavigatorViewModel : Base.BaseViewModel, IXPathNavigator
    {
        private IEnumerable<IXPathNavigator> _children;

        public XPathNavigatorViewModel(XPathNavigator navigator)
        {
            this.XPathNavigator = navigator;
        }

        private bool _isSelected;

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

        private bool _isExpanded;
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

        public XPathNavigator XPathNavigator { get; set; }

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

        public void ExpandAll()
        {
            this.IsExpanded = true;

            foreach (var child in this.Children)
                child.ExpandAll();
        }

        public void CollapseAll()
        {
            this.IsExpanded = false;

            foreach (var child in this.Children)
                child.CollapseAll();
        }
    }
}
