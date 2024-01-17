using FontAwesome.Sharp;
using System.Windows;
using System.Windows.Input;
using TestCrypto.Models;
using TestCrypto.Repositories;

namespace TestCrypto.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private UserAccountModel _currentUserAccount;
        private IUserRepository userRepository;
        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;

        #region Properties
        public UserAccountModel CurrentUserAccount
        {
            get
            {
                return _currentUserAccount;
            }

            set
            {
                _currentUserAccount = value;
                OnPropertyChanged(nameof(CurrentUserAccount));
            }
        }

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
        #endregion

        public ICommand ShowHomeViewCommand { get; }
        public ICommand ShowCurrencyOverviewViewCommand { get; }
        public ICommand ShowMarketLeadersViewCommand { get; }
        public ICommand ShowTraidingVolumeViewCommand { get; }
        public ICommand ShowTransactionViewCommand { get; }
        public ICommand ShowSettingViewCommand { get; }

        public MainViewModel()
        {
            userRepository = new UserRepository();
            CurrentUserAccount = new UserAccountModel();

            ShowHomeViewCommand = new ViewModelCommand(ExecuteShowHomeViewCommand);
            ShowCurrencyOverviewViewCommand = new ViewModelCommand(ExecuteShowCurrencyOverviewViewCommand);
            ShowMarketLeadersViewCommand = new ViewModelCommand(ExecuteShowMarketLeadersViewCommand);
            ShowTraidingVolumeViewCommand = new ViewModelCommand(ExecuteShowTraidingVolumeViewCommand);
            ShowTransactionViewCommand = new ViewModelCommand(ExecuteShowTransactionViewCommand);
            ShowSettingViewCommand = new ViewModelCommand(ExecuteShowSettingViewCommand);

            ExecuteShowHomeViewCommand(null);

            LoadCurrentUserData();
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

        private void ExecuteShowHomeViewCommand(object obj)
        {
            CurrentChildView = new HomeViewModel();
            Caption = (string)Application.Current.Resources["dashboard"];
            Icon = IconChar.Home;
        }

        private void LoadCurrentUserData()
        {
            //var user = userRepository.GetByUsername(Thread.CurrentPrincipal.Identity.Name);
            //if (user != null)
            //{
            //    CurrentUserAccount.Username = user.Username;
            //    CurrentUserAccount.DisplayName = $"{user.Name} {user.LastName}";
            //    CurrentUserAccount.ProfilePicture = null;               
            //}
            //else
            //{
                CurrentUserAccount.DisplayName= (string)Application.Current.Resources["invalidUser"];
            //}
        }
    }
}
