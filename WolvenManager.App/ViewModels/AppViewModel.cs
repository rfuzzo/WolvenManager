using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using ReactiveUI;
using Splat;
using WolvenKit.Common.Services;
using WolvenManager.App.Services;
using WolvenManager.App.ViewModels.PageViewModels;
using DynamicData;
using ReactiveUI.Fody.Helpers;

namespace WolvenManager.App.ViewModels
{

    public class AppViewModel : MainViewModel, IScreen
    {
        private readonly ISettingsService _settingsService;
        private readonly INotificationService _notificationService;
        private readonly ILoggerService _loggerService;

        

        private string Version { get; set; }


        public AppViewModel(
            ISettingsService settingsService,
            INotificationService notificationService,
            ILoggerService loggerService
            )
        {
            _settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
            _notificationService = notificationService ?? Locator.Current.GetService<INotificationService>();
            _loggerService = loggerService ?? Locator.Current.GetService<ILoggerService>();

            // routing
            Router = new RoutingState();

            // versioning
            var assembly = Assembly.GetExecutingAssembly();
            var productName = ((AssemblyProductAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute), false))?.Product;
            Version = assembly.GetName().Version?.ToString();
            Title = $"{productName}-{Version}";


            OnStartup();

            // commands
            RoutingSettingsCommand = ReactiveCommand.Create(delegate()
            {
                Router.Navigate.Execute(Locator.Current.GetService<SettingsViewModel>());
            }, CanExecuteRouting);
            RoutingCommand = ReactiveCommand.Create<Constants.RoutingIDs>(ExecuteSidebar, CanExecuteRouting);

            ToggleBottomBarCommand = ReactiveCommand.Create(() =>
            {
                IsBottomBarVisible = !IsBottomBarVisible;
            });






            //filter, sort and populate reactive list,
            _loggerService.Connect() //connect to the cache
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _logEntries)
                .Subscribe();
        }

        #region properties

        [Reactive]
        public bool IsBottomBarVisible { get; set; }

        private string _title;
        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        private readonly ReadOnlyObservableCollection<LogEntry> _logEntries;
        public ReadOnlyObservableCollection<LogEntry> LogEntries => _logEntries;

        public RoutingState Router { get; }

        #endregion



        #region commands


        public ReactiveCommand<Unit, Unit> ToggleBottomBarCommand { get; }


        private IObservable<bool> CanExecuteRouting => _settingsService.IsValid;


        public ReactiveCommand<Unit, Unit> RoutingSettingsCommand { get; }
        public ReactiveCommand<Constants.RoutingIDs, Unit> RoutingCommand { get; }

        private void ExecuteSidebar(Constants.RoutingIDs parameter)
        {
            switch (parameter)
            {
                case Constants.RoutingIDs.Main:
                    Router.Navigate.Execute(Locator.Current.GetService<ModListViewModel>());
                    break;
                case Constants.RoutingIDs.Modkit:
                    Router.Navigate.Execute(Locator.Current.GetService<ModkitViewModel>());
                    break;
                case Constants.RoutingIDs.Settings:
                    Router.Navigate.Execute(Locator.Current.GetService<SettingsViewModel>());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parameter), parameter, null);
            }


            
        }

        #endregion

        public void OnStartup()
        {
            // Once 
            _settingsService.IsValid.Subscribe(isvalid =>
            {
                if (!isvalid)
                {
                    return;
                }
            });

            // navigate to settings if game is not found
            if (string.IsNullOrEmpty(_settingsService.GamePath))
            {
                Router.Navigate.Execute(Locator.Current.GetService<SettingsViewModel>());
            }
            else
            {
                Router.Navigate.Execute(Locator.Current.GetService<ModkitViewModel>());
            }
        }

    }
}
