namespace XmlExplorerLib.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///
    /// </summary>
    public class XPathTreeView : TreeView
    {
        static XPathTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XPathTreeView),
                               new FrameworkPropertyMetadata(typeof(XPathTreeView)));
        }

        /// <summary>
        /// Creates the element that is used to display a <see cref="XPathTreeViewItem">.
        /// </summary>
        /// <returns>A new <see cref="BreadcrumbTreeItem"> object instance.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new XPathTreeViewItem() { };
        }
    }
}
