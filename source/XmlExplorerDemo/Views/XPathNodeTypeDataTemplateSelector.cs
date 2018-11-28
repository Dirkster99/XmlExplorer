namespace XmlExplorerDemo.Views
{
    using XmlExplorerDemo.ViewModels;
    using System.Windows;
    using System.Windows.Controls;
    using System.Xml.XPath;

    public class XPathNodeTypeDataTemplateSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item,
                                                                   DependencyObject container)
        {
            XPathNavigatorView view = item as XPathNavigatorView;
            FrameworkElement presenter = container as FrameworkElement;

            if (view != null && container != null)
            {
                XPathNavigator navigator = view.XPathNavigator;

                if (navigator != null)
                {
                    switch (navigator.NodeType)
                    {
                        case XPathNodeType.Root:
                            return (DataTemplate)presenter.FindResource("elementXPathNavigatorTemplate") as DataTemplate; //xmlDeclarationXmlNodeTemplate

                        case XPathNodeType.ProcessingInstruction:
                            return (DataTemplate)presenter.FindResource("processingInstructionXPathNavigatorTemplate") as DataTemplate;

                        case XPathNodeType.Comment:
                            return (DataTemplate)presenter.FindResource("commentXPathNavigatorTemplate") as DataTemplate;

                        case XPathNodeType.Element:
                            return (DataTemplate)presenter.FindResource("elementXPathNavigatorTemplate") as DataTemplate;

                        case XPathNodeType.Text:
                            return (DataTemplate)presenter.FindResource("textXPathNavigatorTemplate") as DataTemplate;

                        case XPathNodeType.Attribute:
                            return (DataTemplate)presenter.FindResource("attributeXPathNavigatorTemplate") as DataTemplate;
                    }
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
