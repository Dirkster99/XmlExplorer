namespace XmlExplorerLib.Themes
{
    using System.Windows;

    /// <summary>
    /// Class implements static resource keys that should be referenced to configure
    /// colors, styles and other elements that are typically changed between themes.
    /// </summary>
    public static class ResourceKeys
    {
        #region Accent Keys
        /// <summary>
        /// Accent Color Key - This Color key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ControlAccentColorKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentColorKey");

        /// <summary>
        /// Accent Brush Key - This Brush key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ControlAccentBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentBrushKey");
        #endregion Accent Keys      

        #region SelectedItems Brushkeys
        /// <summary>
        /// Selected Background Brush Key
        /// This Brush key is used to accent the background color of selected elements in the UI.
        /// </summary>
        public static readonly ComponentResourceKey ItemSelectedBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ItemSelectedBackgroundBrushKey");

        /// <summary>
        /// Selected Foreground Brush Key
        /// This Brush key is used to define foreground color of selected elements in the UI.
        /// </summary>
        public static readonly ComponentResourceKey ItemSelectedForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ItemSelectedForegroundBrushKey");

        /// <summary>
        /// Gets the Background Brush key to color the background of the selected item
        /// when it is not focused.
        /// </summary>
        public static readonly ComponentResourceKey ItemSelectedNotFocusedForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ItemSelectedNotFocusedForegroundBrushKey");

        /// <summary>
        /// Gets the Background Brush key to color the background of the selected item
        /// when it is not focused.
        /// </summary>
        public static readonly ComponentResourceKey ItemSelectedNotFocusedBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ItemSelectedNotFocusedBackgroundBrushKey");
        #endregion SelectedItems Brushkeys

        /// <summary>
        /// Gets a the applicable foreground Brush key that should be used for coloring text.
        /// </summary>
        public static readonly ComponentResourceKey ControlTextBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlTextBrushKey");

        // Foreground color of disabled Xml items
        public static readonly ComponentResourceKey XmlItemDisabledForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlItemDisabledForegroundBrushKey");

        public static readonly ComponentResourceKey XmlDelimiterForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlDelimiterForegroundBrushKey");

        public static readonly ComponentResourceKey XmlAttributeValueForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlAttributeValueForegroundBrushKey");
        public static readonly ComponentResourceKey XmlAttributeNameForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlAttributeNameForegroundBrushKey");
        public static readonly ComponentResourceKey XmlNameForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlNameForegroundBrushKey");
        public static readonly ComponentResourceKey XmlTextForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlTextForegroundBrushKey");
        public static readonly ComponentResourceKey XmlCommentForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlCommentForegroundBrushKey");
        public static readonly ComponentResourceKey XmlProcessingInstructionForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "XmlProcessingInstructionForegroundBrushKey");

        // GlyphBrush -> GlyphBrushKey
        public static readonly ComponentResourceKey GlyphBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "GlyphBrushKey");
        public static readonly ComponentResourceKey GlyphDisabledBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "GlyphDisabledBrushKey");

        public static readonly ComponentResourceKey TreeViewItemFocusVisualForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "TreeViewItemFocusVisualForegroundBrushKey");
        
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
