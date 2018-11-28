namespace XmlExplorerLib
{
    using System.Windows;
    using System.Windows.Controls;

    public class XPathTreeViewItem : TreeViewItem
    {
        #region Constructor
        /// <summary>
        /// Static constructor
        /// </summary>
        static XPathTreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XPathTreeViewItem),
                    new FrameworkPropertyMetadata(typeof(XPathTreeViewItem)));
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public XPathTreeViewItem()
        {
        }
        #endregion

        #region methods
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new XPathTreeViewItem();
        }
        #endregion methods
    }
}
