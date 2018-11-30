namespace XmlExplorerLib.interfaces
{
    using System.Collections.Generic;
    using System.Xml.XPath;

    public interface IXPathNavigator
    {
        #region properties
        bool HasNamespace { get; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
        string Name { get; }
        string Value { get; }
        IEnumerable<IXPathNavigator> Children { get; }
        XPathNavigator XPathNavigator { get; set; }
        #endregion properties

        #region methods
        void CollapseAll();
        void ExpandAll();
        #endregion methods
    }
}