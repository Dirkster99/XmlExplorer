namespace XmlExplorerDemo.ViewModels.Themes
{
    using MLib.Interfaces;
    using MLib.Themes;
    using Settings.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using XmlExplorerDemo.Interfaces;
    using XmlExplorerDemo.Models;

    /// <summary>
    /// ViewModel class that manages theme properties for binding and display in WPF UI.
    /// </summary>
    internal class ThemesManagerViewModel : XmlExplorerDemo.ViewModels.Base.BaseViewModel,
                                            IThemesManagerViewModel
    {
        #region private fields
        protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private ThemeDefinitionViewModel _SelectedTheme = null;
        private bool _IsEnabled = true;

        private readonly ThemeDefinitionViewModel _DefaultTheme = null;
        private readonly Dictionary<string, ThemeDefinitionViewModel> _ListOfThemes = null;
        private readonly ISettingsManager _SettingsManager = null;
        private readonly IAppearanceManager _AppearanceManager = null;
        #endregion private fields

        #region constructors
        /// <summary>
        /// Standard Constructor
        /// </summary>
        public ThemesManagerViewModel(ISettingsManager settings,
                                      IAppearanceManager appearanceManager)
            : this()
        {
            _SettingsManager = settings;
            _AppearanceManager = appearanceManager;

            CreateDefaultsSettings(settings, appearanceManager);

            foreach (var item in _SettingsManager.Themes.GetThemeInfos())
            {
                var list = new List<string>();
                foreach (var subitem in item.ThemeSources)
                    list.Add(subitem.ToString());

                _ListOfThemes.Add(item.DisplayName, new ThemeDefinitionViewModel(new ThemeDefinition(item.DisplayName, list)));
            }


            // Lets make sure there is a default
            string defaultThemeDisplayName = _AppearanceManager.GetDefaultTheme().DisplayName;
            _ListOfThemes.TryGetValue(defaultThemeDisplayName, out _DefaultTheme);

            // and something sensible is selected
            _SelectedTheme = _DefaultTheme;
            _SelectedTheme.IsSelected = true;
        }

        /// <summary>
        /// Hidden standard constructor
        /// </summary>
        protected ThemesManagerViewModel()
        {
            _ListOfThemes = new Dictionary<string, ThemeDefinitionViewModel>();
        }
        #endregion constructors

        #region properties
        /// <summary>
        /// Returns a default theme that should be applied when nothing else is available.
        /// </summary>
        public ThemeDefinitionViewModel DefaultTheme
        {
            get
            {
                return _DefaultTheme;
            }
        }

        /// <summary>
        /// Returns a list of theme definitons.
        /// </summary>
        public List<ThemeDefinitionViewModel> ListOfThemes
        {
            get
            {
                return _ListOfThemes.Select(it => it.Value).ToList();
            }
        }

        /// <summary>
        /// Gets the currently selected theme (or desfault on applaiction start-up)
        /// </summary>
        public ThemeDefinitionViewModel SelectedTheme
        {
            get
            {
                return _SelectedTheme;
            }

            private set
            {
                if (_SelectedTheme != value)
                {
                    if (_SelectedTheme != null)
                        _SelectedTheme.IsSelected = false;

                    _SelectedTheme = value;

                    if (_SelectedTheme != null)
                        _SelectedTheme.IsSelected = true;

                    this.NotifyPropertyChanged(() => this.SelectedTheme);
                }
            }
        }

        /// <summary>
        /// Gets whether a different theme can be selected right now or not.
        /// This property should be bound to the UI that selects a different
        /// theme to avoid the case in which a user could select a theme and
        /// select a different theme while the first theme change request is
        /// still processed.
        /// </summary>
        public bool IsEnabled
        {
            get { return _IsEnabled; }

            private set
            {
                if (_IsEnabled != value)
                {
                    _IsEnabled = value;
                    NotifyPropertyChanged(() => IsEnabled);
                }
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Applies a new theme based on the supplied theme name.
        /// </summary>
        /// <param name="themeName"></param>
        public bool ApplyTheme(string themeName)
        {
            if (themeName != null)
            {
                IsEnabled = false;
                try
                {
                    Color AccentColor = ThemesManagerViewModel.GetCurrentAccentColor(_SettingsManager);
                    _AppearanceManager.SetTheme(_SettingsManager.Themes, themeName, AccentColor);

                    ThemeDefinitionViewModel o;
                    _ListOfThemes.TryGetValue(themeName, out o);
                    SelectedTheme = o;
                    _SettingsManager.Options.SetOptionValue("Appearance", "ThemeDisplayName", themeName);

                    return true;
                }
                catch (Exception exp)
                {
                    logger.Error(exp);
                }
                finally
                {
                    IsEnabled = true;
                }
            }

            return false;
        }

        /// <summary>
        /// Applies a new theme based on the given parameter.
        /// </summary>
        /// <param name="thisTheme"></param>
        public bool ApplyTheme(ThemeDefinitionViewModel thisTheme)
        {
            return ApplyTheme(thisTheme.Model.Name);
        }

        public static Color GetCurrentAccentColor(ISettingsManager settings)
        {
            Color AccentColor = default(Color);

            if (settings.Options.GetOptionValue<bool>("Appearance", "ApplyWindowsDefaultAccent"))
            {
                try
                {
                    AccentColor = SystemParameters.WindowGlassColor;
                }
                catch
                {
                }

                // This may be black on Windows 7 and the experience is black & white then :-(
                if (AccentColor == default(Color) || AccentColor == Colors.Black || AccentColor.A == 0)
                    AccentColor = Color.FromRgb(0x1b, 0xa1, 0xe2);
            }
            else
                AccentColor = settings.Options.GetOptionValue<Color>("Appearance", "AccentColor");

            return AccentColor;
        }

        private void CreateDefaultsSettings(ISettingsManager settings
                                          , IAppearanceManager appearance)
        {
            try
            {
                // Add default themings for Dark and Light
                appearance.SetDefaultThemes(settings.Themes);

                // Add additional Dark resources to those theme resources added above
                appearance.AddThemeResources("Dark", new List<Uri>
                {
                  // Todo: Add Additional references to theming resources here
                  new Uri("/MWindowLib;component/Themes/DarkTheme.xaml", UriKind.RelativeOrAbsolute)
                 ,new Uri("/XmlExplorerDemo;component/BindToMLib/MWindowLib_DarkLightBrushs.xaml", UriKind.RelativeOrAbsolute)
                 ,new Uri("/XmlExplorerLib;component/Themes/DarkBrushs.xaml", UriKind.RelativeOrAbsolute)
                 ,new Uri("/XmlExplorerDemo;component/BindToMLib/XmlExplorerLib_DarkLightBrushs.xaml", UriKind.RelativeOrAbsolute)

                }, settings.Themes);
            }
            catch
            {
            }

            try
            {
                // Add additional Light resources to those theme resources added above
                appearance.AddThemeResources("Light", new List<Uri>
                {
                  // Todo: Add Additional references to theming resources here
                  new Uri("/MWindowLib;component/Themes/LightTheme.xaml", UriKind.RelativeOrAbsolute)
                 ,new Uri("/XmlExplorerDemo;component/BindToMLib/MWindowLib_DarkLightBrushs.xaml", UriKind.RelativeOrAbsolute)
                 ,new Uri("/XmlExplorerLib;component/Themes/LightBrushs.xaml", UriKind.RelativeOrAbsolute)
                 ,new Uri("/XmlExplorerDemo;component/BindToMLib/XmlExplorerLib_DarkLightBrushs.xaml", UriKind.RelativeOrAbsolute)

                }, settings.Themes);
            }
            catch
            {
            }

            try
            {
                // Create a general settings model to make sure the app is at least governed by defaults
                // if there are no customized settings on first ever start-up of application
                var options = settings.Options;

                SettingDefaults.CreateGeneralSettings(options);
                SettingDefaults.CreateAppearanceSettings(options, settings);

                settings.Options.SetUndirty();
            }
            catch
            {
            }
        }
        #endregion methods
    }
}
