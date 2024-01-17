using System.Windows.Input;
using System.Globalization;
using System.Net.Http;
using System.Windows;
using System.Net.Http.Json;
using System.Collections.ObjectModel;
using TestCrypto.APIs;
using System.Windows.Documents;
using System.Diagnostics;
using System.Security.Policy;
using System;

namespace TestCrypto.ViewModels
{
    internal class CurrencyOverviewViewModel: ViewModelBase
    {
        private string _errorMessage;
        private string _dataOfSearch;
        private ObservableCollection<CoinDataApi> _coinDataCollection;

        public ObservableCollection<CoinDataApi> CoinDataCollection
        {
            get { return _coinDataCollection; }
            set
            {
                _coinDataCollection = value;
                OnPropertyChanged(nameof(CoinDataCollection));
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

        public string DataOfSearch
        {
            get
            {
                return _dataOfSearch;
            }

            set
            {
                _dataOfSearch = value;
                OnPropertyChanged(nameof(DataOfSearch));
            }
        }

        public ICommand ShowDataCurrencyCommand { get; }
        public ICommand OpenLinkCommand { get; }

        public CurrencyOverviewViewModel()
        {
            ShowDataCurrencyCommand = new ViewModelCommand(ExecuteShowDataCurrencyCommand);
            OpenLinkCommand = new ViewModelCommand(ExecuteOpenLinkCommand);
        }

        private void ExecuteOpenLinkCommand(object obj)
        {
            if (Uri.IsWellFormedUriString((string)obj, UriKind.Absolute))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = (string)obj,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            else
            {
                ErrorMessage = (string)Application.Current.Resources["errorDisplay"];
            }
        }

        private async void ExecuteShowDataCurrencyCommand(object obj)
        {
            if (string.IsNullOrEmpty(DataOfSearch))
            {
                ErrorMessage = (string)Application.Current.Resources["emptySearch"];
                return;
            }

            ErrorMessage = "";
            List<CoinDataApi> allCoinData = await GetCoinDataFromApiAsync();

            if (allCoinData.Count() > 0)
            {
                CoinDataCollection = new ObservableCollection<CoinDataApi>(FilterCoinData(allCoinData));
            }
            else
            {
                ErrorMessage = (string)Application.Current.Resources["errorDisplay"];
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

        private async Task<List<CoinDataApi>> GetCoinDataFromApiAsync()
        {
            string apiUrl = "https://api.coincap.io/v2/assets";

            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(apiUrl);

                    response.EnsureSuccessStatusCode();

                    var apiResponse = await response.Content.ReadFromJsonAsync<CoinDataApiResponse>();

                    return apiResponse.Data.Select(coin => new CoinDataApi
                    {
                        Rank = int.Parse(coin.Rank),
                        Symbol = coin.Symbol,
                        Name = coin.Name,
                        Supply = ParseDecimal(coin.Supply),
                        PriceUsd = ParseDecimal(coin.PriceUsd),
                        ChangePercent24Hr = ParseDecimal(coin.ChangePercent24Hr),
                        Explorer = coin.Explorer
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching data from API: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<CoinDataApi>();
            }
        }

        private List<CoinDataApi> FilterCoinData(List<CoinDataApi> allCoinData)
        {
            string searchText = DataOfSearch.ToLower();

            return allCoinData.Where(coin =>
                coin.Name.ToLower().Contains(searchText) ||
                coin.Symbol.ToLower().Contains(searchText) ||
                coin.Rank.ToString().Contains(searchText)
            ).ToList();
        }
    }
}