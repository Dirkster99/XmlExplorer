namespace XmlExplorerLib.Templates
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Xml.XPath;
    using XmlExplorerLib.interfaces;

    public class XPathNodeTypeDataTemplateSelector : DataTemplateSelector
    {
        public override System.Windows.DataTemplate SelectTemplate(object item,
                                                                   DependencyObject container)
        {
            IXPathNavigator view = item as IXPathNavigator;

            if (view != null)
            {
                XPathNavigator navigator = view.XPathNavigator;
                var presenter = container as FrameworkElement;
                if (navigator != null && presenter != null)
                {
                    switch (navigator.NodeType)
                    {
                        case XPathNodeType.Root:
                            return presenter.FindResource("elementXPathNavigatorTemplate") as DataTemplate; //xmlDeclarationXmlNodeTemplate

                        case XPathNodeType.ProcessingInstruction:
                            return presenter.FindResource("processingInstructionXPathNavigatorTemplate") as DataTemplate;

                        case XPathNodeType.Comment:
                            return presenter.FindResource("commentXPathNavigatorTemplate") as DataTemplate;

                        case XPathNodeType.Element:
                            return presenter.FindResource("elementXPathNavigatorTemplate") as DataTemplate;

                        case XPathNodeType.Text:
                            return presenter.FindResource("textXPathNavigatorTemplate") as DataTemplate;

                        case XPathNodeType.Attribute:
                            return presenter.FindResource("attributeXPathNavigatorTemplate") as DataTemplate;
                    }
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
