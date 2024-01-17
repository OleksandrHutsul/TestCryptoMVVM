using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Input;
using TestCrypto.APIs;

namespace TestCrypto.ViewModels
{
    internal class TransactionViewModel: ViewModelBase
    {
        private string _errorMessage;
        private string _dataOfInputComboBox;
        private string _dataOfOutputComboBox;
        private string _dataOfInputTextBox;
        private string _dataOfOutputTextBox;
        private string _dataOfPriceTextBox;
        private string _dataOfResultTextBox;
        private bool _isInputTextBoxEnabled;
        private bool _isInputComboBoxEnabled;

        private ObservableCollection<CurrencyDataApi> _currencyDataCollection;

        #region Properties
        public ObservableCollection<CurrencyDataApi> CurrencyDataCollection
        {
            get { return _currencyDataCollection; }
            set
            {
                _currencyDataCollection = value;
                OnPropertyChanged(nameof(CurrencyDataCollection));
            }
        }

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

        public string DataOfInputComboBox
        {
            get
            {
                return _dataOfInputComboBox;
            }

            set
            {
                _dataOfInputComboBox = value;
                OnPropertyChanged(nameof(DataOfInputComboBox));
            }
        }

        public string DataOfOutputComboBox
        {
            get
            {
                return _dataOfOutputComboBox;
            }

            set
            {
                _dataOfOutputComboBox = value;
                OnPropertyChanged(nameof(DataOfOutputComboBox));
            }
        }

        public string DataOfInputTextBox
        {
            get
            {
                return _dataOfInputTextBox;
            }

            set
            {
                _dataOfInputTextBox = value;
                OnPropertyChanged(nameof(DataOfInputTextBox));
            }
        }

        public string DataOfOutputTextBox
        {
            get
            {
                return _dataOfOutputTextBox;
            }

            set
            {
                _dataOfOutputTextBox = value;
                OnPropertyChanged(nameof(DataOfOutputTextBox));
            }
        }

        public string DataOfPriceTextBox
        {
            get
            {
                return _dataOfPriceTextBox;
            }

            set
            {
                _dataOfPriceTextBox = value;
                OnPropertyChanged(nameof(DataOfPriceTextBox));
            }
        }

        public string DataOfResultTextBox
        {
            get
            {
                return _dataOfResultTextBox;
            }

            set
            {
                _dataOfResultTextBox = value;
                OnPropertyChanged(nameof(DataOfResultTextBox));
            }
        }

        public bool IsInputTextBoxEnabled
        {
            get { return _isInputTextBoxEnabled; }
            set
            {
                _isInputTextBoxEnabled = value;
                OnPropertyChanged(nameof(IsInputTextBoxEnabled));
            }
        }

        public bool IsInputComboBoxEnabled
        {
            get { return _isInputComboBoxEnabled; }
            set
            {
                _isInputComboBoxEnabled = value;
                OnPropertyChanged(nameof(IsInputComboBoxEnabled));
            }
        }
        #endregion

        public ICommand ShowConvertCurrencyCommand { get; }
        public ICommand ShowActiveControlCommand { get; }

        public TransactionViewModel()
        {
            ShowConvertCurrencyCommand = new ViewModelCommand(ExecuteShowConvertCurrencyCommand);
            ShowActiveControlCommand = new ViewModelCommand(ExecuteShowActiveControlCommand);

            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await FillCurrencyDataCollection();
        }

        private void ExecuteShowActiveControlCommand(object parameter)
        {
            string controlType = parameter as string;

            if (controlType == "TextBox")
            {
                IsInputComboBoxEnabled = false;
                IsInputTextBoxEnabled = true;
            }
            else if (controlType == "ComboBox")
            {
                IsInputComboBoxEnabled = true;
                IsInputTextBoxEnabled = false;
            }
        }

        private void ExecuteShowConvertCurrencyCommand(object obj)
        {
            decimal price = ParseDecimal(DataOfPriceTextBox);
            if (IsInputComboBoxEnabled)
            {
                if (string.IsNullOrEmpty(DataOfInputComboBox) || string.IsNullOrEmpty(DataOfOutputComboBox))
                {
                    ErrorMessage = (string)Application.Current.Resources["emptySearch"];
                    return;
                }
                // Отримання введених даних валют та ціни з полів користувача
                string inputCurrency = DataOfInputComboBox;
                string outputCurrency = DataOfOutputComboBox;

                // Отримання курсу для вибраних валют
                var inputCurrencyData = CurrencyDataCollection.FirstOrDefault(c => c.Symbol == inputCurrency);
                var outputCurrencyData = CurrencyDataCollection.FirstOrDefault(c => c.Symbol == outputCurrency);

                if (inputCurrencyData != null && outputCurrencyData != null)
                {
                    // Виклик методу для обчислення
                    decimal result = AmountConvertCurrency(price, inputCurrencyData.RateUsd, outputCurrencyData.RateUsd);

                    // Встановлення результату
                    DataOfResultTextBox = result.ToString("N2");
                    ErrorMessage = "";
                }
                else
                {
                    // Обробка випадку, коли вибрані валюти не знайдені в даних API
                    ErrorMessage = (string)Application.Current.Resources["currencySelection"];
                    DataOfResultTextBox = "";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(DataOfInputTextBox) || string.IsNullOrEmpty(DataOfOutputTextBox))
                {
                    ErrorMessage = (string)Application.Current.Resources["emptySearch"];
                    return;
                }

                string inputCurrency = DataOfInputTextBox.ToUpper();
                string outputCurrency = DataOfOutputTextBox.ToUpper();

                bool inputCurrencyExists = CurrencyDataCollection.Any(c => c.Symbol == inputCurrency);
                bool outputCurrencyExists = CurrencyDataCollection.Any(c => c.Symbol == outputCurrency);

                if (inputCurrencyExists && outputCurrencyExists)
                {
                    var inputCurrencyData = CurrencyDataCollection.FirstOrDefault(c => c.Symbol == inputCurrency);
                    var outputCurrencyData = CurrencyDataCollection.FirstOrDefault(c => c.Symbol == outputCurrency);
                    // Виклик методу для обчислення
                    decimal result = AmountConvertCurrency(price, inputCurrencyData.RateUsd, outputCurrencyData.RateUsd);

                    // Встановлення результату
                    DataOfResultTextBox = result.ToString("N2");
                    ErrorMessage = "";
                }
                else
                {
                    // Обробка випадку, коли вибрані валюти не знайдені в даних API
                    ErrorMessage = (string)Application.Current.Resources["currencySelection"];
                    DataOfResultTextBox = "";
                }
            }
            
        }

        private decimal ParseDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            value = value.Replace(',', '.');

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                return result;

            if (decimal.TryParse(value, out result))
                return result;

            value = value.Replace('.', ',');

            if (decimal.TryParse(value, out result))
                return result;

            return 0;
        }

        private async Task<List<CurrencyDataApi>> GetDataCurrencyFromApiAsync()
        {
            string apiUrl = "https://api.coincap.io/v2/rates";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(apiUrl);

                    response.EnsureSuccessStatusCode();

                    var apiResponse = await response.Content.ReadFromJsonAsync<CurrencyDataApiResponse>();

                    return apiResponse.Data.Select(currency => new CurrencyDataApi
                    {
                        Symbol = currency.Symbol,
                        RateUsd = ParseDecimal(currency.RateUsd)
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching data from API: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<CurrencyDataApi>();
            }
        }

        private decimal AmountConvertCurrency(decimal amount, decimal inputRate, decimal outputRate)
        {
            return amount * (inputRate / outputRate);
        }

        private async Task FillCurrencyDataCollection()
        {
            try
            {
                // Отримання даних з API
                List<CurrencyDataApi> currenciesFromApi = await GetDataCurrencyFromApiAsync();

                // Заповнення колекції
                CurrencyDataCollection = new ObservableCollection<CurrencyDataApi>(currenciesFromApi);
            }
            catch (Exception ex)
            {
                ErrorMessage = (string)Application.Current.Resources["fillingCurrency"];
            }
        }
    }
}
