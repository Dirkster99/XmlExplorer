﻿namespace Edi.SettingsView.Config.ViewModels
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using Edi.Core.ViewModels.Base;
	using ICSharpCode.AvalonEdit;
	using ICSharpCode.AvalonEdit.Edi.BlockSurround;
	using Edi.Settings.ProgramSettings;
	using UnitComboLib.Models.Unit.Screen;
	using UnitComboLib.ViewModels;
    using UnitComboLib.Models.Unit;
    using Edi.Settings.Interfaces;
    using Edi.Settings;

    public class ConfigViewModel : DialogViewModelBase
	{
		#region fields
		private bool _WordWrapText;
		private bool _ReloadOpenFilesOnAppStart;
		private bool _RunSingleInstance;

		private ILang _LanguageSelected;

		private bool _TextToHTML_ShowLineNumbers = true;
		private bool _TextToHTML_TextToHTML_AlternateLineBackground = true;

		private bool _HighlightOnFileNew = true;
		private string _FileNewDefaultFileName = Edi.Util.Local.Strings.STR_FILE_DEFAULTNAME;
		private string _FileNewDefaultFileExtension = ".txt";
		#endregion fields

		#region constructor
		/// <summary>
		/// Class constructor
		/// </summary>
		public ConfigViewModel()
			: base()
		{
			// Setup default values here - real values are loaded in a specific method of this class (!)
			_WordWrapText = false;
			_ReloadOpenFilesOnAppStart = false;
			_RunSingleInstance = true;

			this.WordWrapText = false;

			// Get default list of units from settings manager
			var unitList = new ObservableCollection<UnitComboLib.Models.ListItem>(SettingsFactory.GenerateScreenUnitList());
			this.SizeUnitLabel =
                UnitComboLib.UnitViewModeService.CreateInstance(
                    unitList,
                    new ScreenConverter(),
                    (int)ZoomUnit.Percentage, 100);

			this.EditorTextOptions = new TextEditorOptions();

			// Initialize localization settings
			Languages = new List<ILang>(SettingsFactory.GetSupportedLanguages());

			// Set default language to make sure app neutral is selected and available for sure
			// (this is a fallback if all else fails)
			try
			{
				this.LanguageSelected = this.Languages.FirstOrDefault(lang => lang.BCP47 == SettingsFactory.DefaultLocal);
			}
			catch
			{
			}
		}
		#endregion constructor

		#region properties
		/// <summary>
		/// Get/set whether WordWarp should be applied in editor (by default) or not.
		/// </summary>
		public bool WordWrapText
		{
			get
			{
				return _WordWrapText;
			}

			set
			{
				if (_WordWrapText != value)
				{
					_WordWrapText = value;
					this.RaisePropertyChanged(() => this.WordWrapText);
				}
			}
		}

		/// <summary>
		/// Expose AvalonEdit Text Editing options for editing in program settings view.
		/// </summary>
		public TextEditorOptions EditorTextOptions { get; set; }

		#region Application Behaviour
		/// <summary>
		/// Get/set whether application re-loads files open in last sesssion or not
		/// </summary>
		public bool ReloadOpenFilesOnAppStart
		{
			get
			{
				return _ReloadOpenFilesOnAppStart;
			}

			set
			{
				if (_ReloadOpenFilesOnAppStart != value)
				{
					_ReloadOpenFilesOnAppStart = value;
					this.RaisePropertyChanged(() => this.ReloadOpenFilesOnAppStart);
				}
			}
		}

		/// <summary>
		/// Get/set whether application can be started more than once.
		/// </summary>
		public bool RunSingleInstance
		{
			get
			{
				return _RunSingleInstance;
			}

			set
			{
				if (_RunSingleInstance != value)
				{
					_RunSingleInstance = value;
					this.RaisePropertyChanged(() => this.RunSingleInstance);
				}
			}
		}
		#endregion Application Behaviour

		#region ScaleView
		/// <summary>
		/// Scale view of text in percentage of font size
		/// </summary>
		public IUnitViewModel SizeUnitLabel { get; set; }

		/// <summary>
		/// Get/set unit of document zoom unit.
		/// </summary>
		public ZoomUnit DocumentZoomUnit
		{
			get
			{
				if (this.SizeUnitLabel.SelectedItem.Key == Itemkey.ScreenFontPoints)
					return ZoomUnit.Points;

				return ZoomUnit.Percentage;
			}

			set
			{
				if (ConvertZoomUnit(value) != this.SizeUnitLabel.SelectedItem.Key)
				{
					if (value == ZoomUnit.Points)
						this.SizeUnitLabel.SetSelectedItemCommand.Execute(Itemkey.ScreenFontPoints);
					else
						this.SizeUnitLabel.SetSelectedItemCommand.Execute(Itemkey.ScreenPercent);

					this.RaisePropertyChanged(() => this.DocumentZoomUnit);
				}
			}
		}

		/// <summary>
		/// Get the title string of the view - to be displayed in the associated view
		/// (e.g. as dialog title)
		/// </summary>
		public string WindowTitle
		{
			get
			{
				return Edi.Util.Local.Strings.STR_ProgramSettings_Caption;
			}
		}
		#endregion ScaleView

		#region Language Localization Support
		/// <summary>
		/// Get list of GUI languages supported in this application.
		/// </summary>
		public List<ILang> Languages { get; private set; }

		/// <summary>
		/// Get/set language of message box buttons for display in localized form.
		/// </summary>
		public ILang LanguageSelected
		{
			get
			{
				return _LanguageSelected;
			}

			set
			{
				if (_LanguageSelected != value)
				{
					_LanguageSelected = value;
					this.RaisePropertyChanged(() => this.LanguageSelected);
				}
			}
		}
		#endregion Language Localization Support

		#region Text To HTML Export
		/// <summary>
		/// Get/set whether Text to HTML should contain line numbers or not.
		/// </summary>
		public bool TextToHTML_ShowLineNumbers
		{
			get
			{
				return _TextToHTML_ShowLineNumbers;
			}

			set
			{
				if (_TextToHTML_ShowLineNumbers != value)
				{
					_TextToHTML_ShowLineNumbers = value;
					this.RaisePropertyChanged(() => this.TextToHTML_ShowLineNumbers);
				}
			}
		}

		/// <summary>
		/// Get/set whether Text to HTML should contain an alternating background.
		/// </summary>
		public bool TextToHTML_AlternateLineBackground
		{
			get
			{
				return _TextToHTML_TextToHTML_AlternateLineBackground;
			}

			set
			{
				if (_TextToHTML_TextToHTML_AlternateLineBackground != value)
				{
					_TextToHTML_TextToHTML_AlternateLineBackground = value;
					this.RaisePropertyChanged(() => this.TextToHTML_AlternateLineBackground);
				}
			}
		}
		#endregion Text To HTML Export

		#region NewFileDefaults
		/// <summary>
		/// Determine whether a file created with File>New should be highlighted or not.
		/// </summary>
		public bool HighlightOnFileNew
		{
			get
			{
				return _HighlightOnFileNew;
			}

			set
			{
				if (_HighlightOnFileNew != value)
				{
					_HighlightOnFileNew = value;
					this.RaisePropertyChanged(() => this.HighlightOnFileNew);
				}
			}
		}

		/// <summary>
		/// Get/set default name of file that is created via File>New.
		/// </summary>
		public string FileNewDefaultFileName
		{
			get
			{
				return _FileNewDefaultFileName;
			}

			set
			{
				if (_FileNewDefaultFileName != value)
				{
					_FileNewDefaultFileName = value;
					this.RaisePropertyChanged(() => this.FileNewDefaultFileName);
				}
			}
		}

		/// <summary>
		/// Get/set default string of file extension (including '.' character)
		/// that is created via File>New.
		/// </summary>
		public string FileNewDefaultFileExtension
		{
			get
			{
				return _FileNewDefaultFileExtension;
			}

			set
			{
				if (_FileNewDefaultFileExtension != value)
				{
					_FileNewDefaultFileExtension = value;
					this.RaisePropertyChanged(() => this.FileNewDefaultFileExtension);
				}
			}
		}
		#endregion NewFileDefaults
		#endregion properties

		#region methods
		/// <summary>
		/// Get default block definitions to add / remove surrounding selection blocks in text.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<BlockDefinition> GetDefaultBlockDefinitions()
		{
			List<BlockDefinition> ret = new List<BlockDefinition>();

			ret.Add(new BlockDefinition("/**", "**/", BlockDefinition.BlockAt.StartAndEnd,
																	string.Empty,   // Default BlockSurrond
																	System.Windows.Input.Key.D1,
																	System.Windows.Input.ModifierKeys.Control
																));

			ret.Add(new BlockDefinition("////", string.Empty, BlockDefinition.BlockAt.Start,
																	string.Empty,   // Default BlockSurrond
																	System.Windows.Input.Key.D2,
																	System.Windows.Input.ModifierKeys.Control
																));

			ret.Add(new BlockDefinition(string.Empty, "<<<<", BlockDefinition.BlockAt.End,
																	string.Empty,   // Default BlockSurrond
																	System.Windows.Input.Key.D3,
																	System.Windows.Input.ModifierKeys.Control
																));

			ret.Add(new BlockDefinition("/**", "**/", BlockDefinition.BlockAt.StartAndEnd,
																	"*.txt",
																	System.Windows.Input.Key.D1,
																	System.Windows.Input.ModifierKeys.Control
																));

			ret.Add(new BlockDefinition("/**", "**/", BlockDefinition.BlockAt.StartAndEnd,
																	"*.cs",
																	System.Windows.Input.Key.D1,
																	System.Windows.Input.ModifierKeys.Control
																));

			ret.Add(new BlockDefinition("<!--", "-->", BlockDefinition.BlockAt.StartAndEnd,
																	"*.xml;*.html;*.htm",
																	System.Windows.Input.Key.D1,
																	System.Windows.Input.ModifierKeys.Control
																));

			return ret;
		}

		/// <summary>
		/// Reset the view model to those options that are going to be presented for editing.
		/// </summary>
		/// <param name="settingData"></param>
		public void LoadOptionsFromModel(IOptions settingData)
		{
			this.ReloadOpenFilesOnAppStart = settingData.ReloadOpenFilesOnAppStart;
			this.RunSingleInstance = settingData.RunSingleInstance;

			this.WordWrapText = settingData.WordWarpText;

			this.EditorTextOptions = new TextEditorOptions(settingData.EditorTextOptions);
			this.SizeUnitLabel = UnitComboLib.UnitViewModeService.CreateInstance(
                new ObservableCollection<UnitComboLib.Models.ListItem>(
                    SettingsFactory.GenerateScreenUnitList()),
						new ScreenConverter(),
					(int)settingData.DocumentZoomUnit, settingData.DocumentZoomView);

			try
			{
				this.LanguageSelected = this.Languages.FirstOrDefault(lang => lang.BCP47 == settingData.LanguageSelected);
			}
			catch
			{
			}

			this.TextToHTML_ShowLineNumbers = settingData.TextToHTML_ShowLineNumbers;
			this.TextToHTML_AlternateLineBackground = settingData.TextToHTML_AlternateLineBackground;

			this.HighlightOnFileNew = settingData.HighlightOnFileNew;
			this.FileNewDefaultFileName = settingData.FileNewDefaultFileName;
			this.FileNewDefaultFileExtension = settingData.FileNewDefaultFileExtension;
		}

		/// <summary>
		/// Save changed settings back to model for further
		/// application and persistence in file system.
		/// </summary>
		/// <param name="settingData"></param>
		public void SaveOptionsToModel(IOptions settingData)
		{
			settingData.ReloadOpenFilesOnAppStart = this.ReloadOpenFilesOnAppStart;
			settingData.RunSingleInstance = this.RunSingleInstance;

			settingData.WordWarpText = this.WordWrapText;

			settingData.EditorTextOptions = new TextEditorOptions(this.EditorTextOptions);
			if (this.SizeUnitLabel.SelectedItem.Key == UnitComboLib.Models.Unit.Itemkey.ScreenFontPoints)
				settingData.DocumentZoomUnit = ZoomUnit.Points;
			else
				settingData.DocumentZoomUnit = ZoomUnit.Percentage;

			settingData.DocumentZoomView = (int)this.SizeUnitLabel.Value;

			settingData.LanguageSelected = this.LanguageSelected.BCP47;

			settingData.TextToHTML_ShowLineNumbers = this.TextToHTML_ShowLineNumbers;
			settingData.TextToHTML_AlternateLineBackground = this.TextToHTML_AlternateLineBackground;

			settingData.HighlightOnFileNew = this.HighlightOnFileNew;
			settingData.FileNewDefaultFileName = this.FileNewDefaultFileName;
			settingData.FileNewDefaultFileExtension = this.FileNewDefaultFileExtension;

			settingData.IsDirty = true;
		}

		/// <summary>
		/// Convert between local zoom unit enumeration and remote zoom unit enumeration.
		/// </summary>
		/// <param name="unit"></param>
		/// <returns></returns>
		private UnitComboLib.Models.Unit.Itemkey ConvertZoomUnit(ZoomUnit unit)
		{
			switch (unit)
			{
				case ZoomUnit.Percentage:
					return UnitComboLib.Models.Unit.Itemkey.ScreenPercent;

				case ZoomUnit.Points:
					return UnitComboLib.Models.Unit.Itemkey.ScreenFontPoints;

				default:
					throw new System.NotImplementedException(unit.ToString());
			}
		}
		#endregion methods
	}
}
