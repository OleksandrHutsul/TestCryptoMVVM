using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace TestCrypto.ViewModels
{
    internal class SettingViewModel : ViewModelBase
    {
        private string _errorMessage;
        private string _languageCollection;
        private string _themeCollection;

        private ObservableCollection<string> _dataOfThemeComboBox;
        private ObservableCollection<string> _dataOfLanguageComboBox;

        #region Properties
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }

            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public string LanguageCollection
        {
            get
            {
                return _languageCollection;
            }

            set
            {
                _languageCollection = value;
                OnPropertyChanged(nameof(LanguageCollection));
            }
        }

        public string ThemeCollection
        {
            get
            {
                return _themeCollection;
            }

            set
            {
                _themeCollection = value;
                OnPropertyChanged(nameof(ThemeCollection));
            }
        }

        public ObservableCollection<string> DataOfThemeComboBox
        {
            get
            {
                return _dataOfThemeComboBox;
            }
            set
            {
                _dataOfThemeComboBox = value;
                OnPropertyChanged(nameof(DataOfThemeComboBox));
            }
        }

        public ObservableCollection<string> DataOfLanguageComboBox
        {
            get
            {
                return _dataOfLanguageComboBox;
            }
            set
            {
                _dataOfLanguageComboBox = value;
                OnPropertyChanged(nameof(DataOfLanguageComboBox));
            }
        }

        #endregion

        public ICommand ShowSaveSettingCommand { get; }

        public SettingViewModel()
        {
            DataOfThemeComboBox = new ObservableCollection<string> { "Light", "Dark" };

            DataOfLanguageComboBox = new ObservableCollection<string> { "Українська", "Français", "English" };

            ShowSaveSettingCommand = new ViewModelCommand(ExecuteShowSaveSettingCommand);
        }

        private void ExecuteShowSaveSettingCommand(object obj)
        {
            if (string.IsNullOrEmpty(LanguageCollection) && string.IsNullOrEmpty(ThemeCollection))
            {
                ErrorMessage = (string)Application.Current.Resources["selectOptions"];
            }
            else
            {
                ErrorMessage = "";
                if (!string.IsNullOrEmpty(LanguageCollection))
                {
                    SetLanguage();
                }
                if (!string.IsNullOrEmpty(ThemeCollection))
                {
                    SetTheme();
                }
            }
        }

        private void SetTheme()
        {
            string style = null;

            switch (ThemeCollection)
            {
                case "Dark":
                    style = "Dark";
                    break;
                case "Light":
                    style = "Light";
                    break;
                default:
                    style = "Dark";
                    break;
            }

            if (style != null)
            {
                var uri = new Uri("Styles/" + style + ".xaml", UriKind.Relative);
                ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;

                Application.Current.MainWindow.Resources.MergedDictionaries
                    .Where(d => d.Source != null && d.Source.OriginalString.StartsWith("Styles/"))
                    .ToList()
                    .ForEach(d => Application.Current.MainWindow.Resources.MergedDictionaries.Remove(d));

                Application.Current.MainWindow.Resources.MergedDictionaries.Add(resourceDict);
            }
        }

        private void SetLanguage()
        {
            string language = null;

            switch (LanguageCollection)
            {
                case "English":
                    language = "en-US";
                    break;
                case "Українська":
                    language = "uk-UA";
                    break;
                case "Français":
                    language = "fr-FR";
                    break;
                default: 
                    language = "en-US"; 
                    break;
            }

            if (language != null)
            {
                ResourceDictionary languageDictionary = new ResourceDictionary
                {
                    Source = new Uri($"/Resource/Languages/language.{language}.xaml", UriKind.Relative)
                };

                Application.Current.Resources.MergedDictionaries
                    .Where(d => d.Source != null && d.Source.OriginalString.StartsWith("/Resource/Languages/"))
                    .ToList()
                    .ForEach(d => Application.Current.Resources.MergedDictionaries.Remove(d));

                Application.Current.Resources.MergedDictionaries.Add(languageDictionary);
            }
        }
    }
}
