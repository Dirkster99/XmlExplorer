namespace XmlExplorerLib.Themes
{
    using System.Windows;

    /// <summary>
    /// Class implements static resource keys that should be referenced to configure
    /// colors, styles and other elements that are typically changed between themes.
    /// </summary>
    public static class ResourceKeys
    {
        // GlyphBrush -> GlyphBrushKey
        public static readonly ComponentResourceKey GlyphBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "GlyphBrushKey");

        #region Style Keys
        // ExpandCollapseToggleStyle -> ExpandCollapseToggleStyleKey
        public static readonly ComponentResourceKey ExpandCollapseToggleStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "ExpandCollapseToggleStyleKey");

        // TreeViewItemFocusVisual -> TreeViewItemFocusVisualStyleKey
        public static readonly ComponentResourceKey TreeViewItemFocusVisualStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "TreeViewItemFocusVisualStyleKey");

        // xmlDelimiterStyle -> XmlDelimiterStyleKey
        public static readonly ComponentResourceKey XmlDelimiterStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlDelimiterStyleKey");

        // xmlAttributeValueStyle -> XmlAttributeValueStyleKey
        public static readonly ComponentResourceKey XmlAttributeValueStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlAttributeValueStyleKey");

        // xmlAttributeNameStyle -> XmlAttributeNameStyleKey
        public static readonly ComponentResourceKey XmlAttributeNameStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlAttributeNameStyleKey");

        // xmlNameStyle -> XmlNameStyleKey
        public static readonly ComponentResourceKey XmlNameStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlNameStyleKey");

        // xmlTextStyle -> XmlTextStyleKey
        public static readonly ComponentResourceKey XmlTextStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlTextStyleKey");

        // xmlCommentStyle -> XmlCommentStyleKey
        public static readonly ComponentResourceKey XmlCommentStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlCommentStyleKey");

        // xmlProcessingInstructionStyle -> XmlProcessingInstructionStyleKey
        public static readonly ComponentResourceKey XmlProcessingInstructionStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlProcessingInstructionStyleKey");
        #endregion Style Keys
    }
}
