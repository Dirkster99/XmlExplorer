namespace XmlExplorerLib.Themes
{
    using System.Windows;

    /// <summary>
    /// Class implements static resource keys that should be referenced to configure
    /// colors, styles and other elements that are typically changed between themes.
    /// </summary>
    public static class ResourceKeys
    {
        public static readonly ComponentResourceKey XmlDelimiterStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlDelimiterStyleKey");
        public static readonly ComponentResourceKey XmlNameStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlNameStyleKey");      
    }
}
