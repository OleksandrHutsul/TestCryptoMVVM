using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Input;
using TestCrypto.APIs;

namespace TestCrypto.ViewModels
{
    internal class TradingVolumeViewModel: ViewModelBase
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

        public ICommand ShowCandlesChartCommand { get; }

        public TradingVolumeViewModel()
        {
            ShowCandlesChartCommand = new ViewModelCommand(ExecuteShowCandlesChartCommand);
        }

        private void ExecuteShowCandlesChartCommand(object obj)
        {
            if (int.TryParse(DataOfInputTextBox, out int numberOfData) && numberOfData >= 1 && numberOfData <= 33)
            {
                LoadAndPlotData(numberOfData);
            }
            else
            {
                ErrorMessage = (string)Application.Current.Resources["validNumber1and33"];
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

        private async Task<List<CandlesDataJsonString>> ReadCandlesDataFromJsonFileAsync(string filePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string jsonContent = await reader.ReadToEndAsync();
                    CandlesDataJsonResponse apiResponse = JsonConvert.DeserializeObject<CandlesDataJsonResponse>(jsonContent);

                    if (apiResponse != null && apiResponse.Data != null)
                    {
                        return apiResponse.Data;
                    }
                    else
                    {
                        MessageBox.Show("Invalid JSON format or missing data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return new List<CandlesDataJsonString>();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading JSON file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return new List<CandlesDataJsonString>();
            }
        }

        private async void LoadAndPlotData(int numberOfData)
        {
            try
            {
                string jsonFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JsonForCandlesChart");
                string jsonFilePath = Path.Combine(jsonFolderPath, "candles.json");

                List<CandlesDataJsonString> candlesDataList = await ReadCandlesDataFromJsonFileAsync(jsonFilePath);

                var plotModel = new PlotModel();
                var candlestickSeries = new CandleStickSeries
                {
                    IncreasingColor = OxyColors.Green,
                    DecreasingColor = OxyColors.Red,
                };

                for (int i = 0; i < Math.Min(numberOfData, candlesDataList.Count); i++)
                {
                    var open = Convert.ToDouble(candlesDataList[i].Open, CultureInfo.InvariantCulture);
                    var high = Convert.ToDouble(candlesDataList[i].High, CultureInfo.InvariantCulture);
                    var low = Convert.ToDouble(candlesDataList[i].Low, CultureInfo.InvariantCulture);
                    var close = Convert.ToDouble(candlesDataList[i].Close, CultureInfo.InvariantCulture);

                    candlestickSeries.Items.Add(new HighLowItem(i, high, low, open, close));
                }

                plotModel.Series.Add(candlestickSeries);

                var categoryAxis = new CategoryAxis
                {
                    Position = AxisPosition.Bottom,
                    Title = "Candles",
                    Key = "CandlesData",
                    ItemsSource = Enumerable.Range(1, candlesDataList.Count).Select(i => i.ToString()),
                    FontSize = 12,
                };

                var yAxis = new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Price",
                    Key = "YAxis",
                    FontSize = 12,
                };

                categoryAxis.TextColor = OxyColor.FromRgb(206, 206, 206);
                categoryAxis.AxislineColor = OxyColor.FromRgb(206, 206, 206);
                categoryAxis.TicklineColor = OxyColor.FromRgb(206, 206, 206);
                categoryAxis.TitleColor = OxyColor.FromRgb(206, 206, 206);
                yAxis.TitleColor = OxyColor.FromRgb(206, 206, 206);
                yAxis.TextColor = OxyColor.FromRgb(206, 206, 206);
                yAxis.AxislineColor = OxyColor.FromRgb(206, 206, 206);
                yAxis.TicklineColor = OxyColor.FromRgb(206, 206, 206);
                plotModel.PlotAreaBorderColor = OxyColor.FromRgb(206, 206, 206);

                plotModel.Axes.Add(categoryAxis);
                plotModel.Axes.Add(yAxis);

                IsEnableTextBlock = "Hidden";
                PlotModel = plotModel;
                ErrorMessage = "";
            }
            catch (Exception ex)
            {
                ErrorMessage = (string)Application.Current.Resources["errorLoadingPlot"];
            }
        }
    }
}
