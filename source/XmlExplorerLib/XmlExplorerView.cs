namespace XmlExplorerLib
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    ///
    /// </summary>
    public class XmlExplorerView : TreeView
    {
        static XmlExplorerView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XmlExplorerView),
                               new FrameworkPropertyMetadata(typeof(XmlExplorerView)));
        }
    }
}
