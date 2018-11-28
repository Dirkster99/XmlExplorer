namespace XmlExplorerDemo.Converters
{
    using XmlExplorerDemo.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Windows.Data;
    using System.Xml.XPath;

    public class XPathNavigatorItemsConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<XPathNavigatorViewModel> childNavigatorViews;

            XPathNodeIterator iterator = value as XPathNodeIterator;
            if (iterator != null)
            {
                childNavigatorViews = new List<XPathNavigatorViewModel>();

                foreach (XPathNavigator childNavigator in iterator)
                {
                    childNavigatorViews.Add(new XPathNavigatorViewModel(childNavigator));
                }

                return childNavigatorViews;
            }

            XPathNavigator navigator;

            XPathNavigatorViewModel view = value as XPathNavigatorViewModel;

            if (view != null)
                navigator = view.XPathNavigator;
            else
                navigator = value as XPathNavigator;

            if (navigator == null)
                return null;

            childNavigatorViews = new List<XPathNavigatorViewModel>();

            foreach (XPathNavigator childNavigator in navigator.SelectChildren(XPathNodeType.All))
            {
                childNavigatorViews.Add(new XPathNavigatorViewModel(childNavigator));
            }

            return childNavigatorViews;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
