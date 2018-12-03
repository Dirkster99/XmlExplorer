namespace XmlExplorerDemo.Interfaces
{
    using System.Collections.Generic;
    using XmlExplorerDemo.ViewModels.Themes;

    internal interface IThemesManagerViewModel
    {
        ThemeDefinitionViewModel DefaultTheme { get; }
        bool IsEnabled { get; }
        List<ThemeDefinitionViewModel> ListOfThemes { get; }
        ThemeDefinitionViewModel SelectedTheme { get; }

        /// <summary>
        /// Applies a new theme based on the supplied theme name.
        /// </summary>
        /// <param name="themeName"></param>
        bool ApplyTheme(string themeName);
    }
}