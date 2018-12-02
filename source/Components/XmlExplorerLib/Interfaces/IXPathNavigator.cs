namespace XmlExplorerLib.interfaces
{
    using System.Collections.Generic;
    using System.Xml.XPath;

    public interface IXPathNavigator
    {
        #region properties
        XPathNavigator XPathNavigator { get; }

        bool HasNamespace { get; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
        string Name { get; }
        string Value { get; }
        IEnumerable<IXPathNavigator> Children { get; }
        #endregion properties

        #region methods
        void CollapseAll();
        void ExpandAll();

        /// <summary>
        /// Gets the formated Xml that represents this Xml node and all its children.
        /// </summary>
        /// <param name="navigator"></param>
        /// <returns></returns>
        string GetXPathNavigatorFormattedXml(XPathNavigator navigator);

        /// <summary>
        /// Returns a string representing the full path of an XPathNavigator.
        /// </summary>
        /// <param name="navigator">An XPathNavigator.</param>
        /// <returns></returns>
        string GetXmlNodeFullPath(XPathNavigator navigator);
        #endregion methods
    }
}