using FontAwesome.Sharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace TestCrypto.ViewModels
{
    internal class HomeViewModel: ViewModelBase
    {
        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;

        public ViewModelBase CurrentChildView
        {
            get
            {
                return _currentChildView;
            }
            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
            }
        }

        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }

        public IconChar Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        public ICommand ShowCurrencyOverviewViewCommand { get; }
        public ICommand ShowMarketLeadersViewCommand { get; }
        public ICommand ShowTraidingVolumeViewCommand { get; }
        public ICommand ShowTransactionViewCommand { get; }
        public ICommand ShowSettingViewCommand { get; }

        public HomeViewModel()
        {
            //ShowCurrencyOverviewViewCommand = new ViewModelCommand(ExecuteShowCurrencyOverviewViewCommand);
            //ShowMarketLeadersViewCommand = new ViewModelCommand(ExecuteShowMarketLeadersViewCommand);
            //ShowTraidingVolumeViewCommand = new ViewModelCommand(ExecuteShowTraidingVolumeViewCommand);
            //ShowTransactionViewCommand = new ViewModelCommand(ExecuteShowTransactionViewCommand);
            //ShowSettingViewCommand = new ViewModelCommand(ExecuteShowSettingViewCommand);
        }

        private void ExecuteShowSettingViewCommand(object obj)
        {
            CurrentChildView = new SettingViewModel();
            Caption = (string)Application.Current.Resources["setting"];
            Icon = IconChar.Tools;
        }

        private void ExecuteShowTransactionViewCommand(object obj)
        {
            CurrentChildView = new TransactionViewModel();
            Caption = (string)Application.Current.Resources["transaction"];
            Icon = IconChar.Wallet;
        }

        private void ExecuteShowTraidingVolumeViewCommand(object obj)
        {
            CurrentChildView = new TradingVolumeViewModel();
            Caption = (string)Application.Current.Resources["traidingVolume"];
            Icon = IconChar.ChartColumn;
        }

        private void ExecuteShowMarketLeadersViewCommand(object obj)
        {
            CurrentChildView = new MarketLeadersViewModel();
            Caption = (string)Application.Current.Resources["marketLeaders"];
            Icon = IconChar.ChartBar;
        }

        private void ExecuteShowCurrencyOverviewViewCommand(object obj)
        {
            CurrentChildView = new CurrencyOverviewViewModel();
            Caption = (string)Application.Current.Resources["currencyOverview"];
            Icon = IconChar.MagnifyingGlassDollar;
        }
    }
}
