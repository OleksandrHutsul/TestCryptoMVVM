using OxyPlot;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Input;
using TestCrypto.APIs;

namespace TestCrypto.ViewModels
{
    internal class MarketLeadersViewModel: ViewModelBase
    {
        private string _errorMessage;
        private string _dataOfInputTextBox;
        private string _isEnableTextBlock;
        private ObservableCollection<CurrencyDataApi> _currencyDataCollection;
        private PlotModel _plotModel;

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

        public string IsEnableTextBlock
        {
            get
            {
                return _isEnableTextBlock;
            }

            set
            {
                _isEnableTextBlock = value;
                OnPropertyChanged(nameof(IsEnableTextBlock));
            }
        }

        public PlotModel PlotModel
        {
            get 
            { 
                return _plotModel; 
            }

            set
            {
                _plotModel = value;
                OnPropertyChanged(nameof(PlotModel));
            }
        }
        #endregion

        public ICommand ShowChartLeadersCommand { get; }

        public MarketLeadersViewModel()
        {
            ShowChartLeadersCommand = new ViewModelCommand(ExecuteShowChartLeadersCommand);
        }

        private void ExecuteShowChartLeadersCommand(object obj)
        {
            LoadAndPlotData();
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

        private async void LoadAndPlotData()
        {
            try
            {
                List<CoinDataApi> coinDataList = await GetCoinDataFromApiAsync();

                int numberOfCoinsToShow;

                if (int.TryParse(DataOfInputTextBox, out numberOfCoinsToShow) && numberOfCoinsToShow >= 1 && numberOfCoinsToShow <= 100)
                {
                    coinDataList = coinDataList.Take(numberOfCoinsToShow).ToList();

                    var plotModel = new OxyPlot.PlotModel();
                    var barSeries = new OxyPlot.Series.BarSeries();

                    for (int i = 0; i < coinDataList.Count; i++)
                    {
                        barSeries.Items.Add(new OxyPlot.Series.BarItem { Value = coinDataList[i].Rank, CategoryIndex = i });
                    }

                    plotModel.Series.Add(barSeries);

                    var coinNameAxis = new OxyPlot.Axes.CategoryAxis
                    {
                        Position = OxyPlot.Axes.AxisPosition.Left,
                        Title = "Coin Name",
                        Key = "CoinNames",
                        ItemsSource = coinDataList.Select(coin => coin.Name),
                        FontSize = 12,
                    };

                    var rankAxis = new OxyPlot.Axes.LinearAxis
                    {
                        Position = OxyPlot.Axes.AxisPosition.Bottom,
                        Title = "Rank",
                        Key = "RankAxis",
                        FontSize = 12,
                    };

                    plotModel.Axes.Add(coinNameAxis);
                    plotModel.Axes.Add(rankAxis);

                    coinNameAxis.TextColor = OxyColor.FromRgb(206, 206, 206);
                    barSeries.FillColor = OxyColor.FromRgb(206, 206, 206);
                    coinNameAxis.AxislineColor = OxyColor.FromRgb(206, 206, 206);
                    coinNameAxis.TicklineColor = OxyColor.FromRgb(206, 206, 206);
                    coinNameAxis.TitleColor = OxyColor.FromRgb(206, 206, 206);
                    rankAxis.TitleColor = OxyColor.FromRgb(206, 206, 206);
                    rankAxis.TextColor = OxyColor.FromRgb(206, 206, 206);
                    rankAxis.AxislineColor = OxyColor.FromRgb(206, 206, 206);
                    rankAxis.TicklineColor = OxyColor.FromRgb(206, 206, 206);
                    plotModel.PlotAreaBorderColor = OxyColor.FromRgb(206, 206, 206);

                    IsEnableTextBlock = "Hidden";
                    PlotModel = plotModel;
                    ErrorMessage = "";
                }
                else
                {
                    ErrorMessage = (string)Application.Current.Resources["validNumber1and100"];
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = (string)Application.Current.Resources["errorLoadingPlot"];
            }
        }
    }
}
