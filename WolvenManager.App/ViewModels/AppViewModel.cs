using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;
using Splat;
using WolvenKit.Common.Services;
using WolvenManager.App.Services;
using WolvenManager.App.ViewModels.PageViewModels;
using DynamicData;
using ReactiveUI.Fody.Helpers;
using WolvenKit.Common.Tools;
using WolvenKit.Common.Tools.Oodle;
using WolvenManager.App.Models;
using WolvenManager.App.Utility;

namespace WolvenManager.App.ViewModels
{

    public class AppViewModel : MainViewModel, IScreen
    {
        private readonly ISettingsService _settingsService;
        private readonly INotificationService _notificationService;
        private readonly ILoggerService _loggerService;
        private readonly IArchiveService _archiveService;

        private string Version { get; set; }


        public AppViewModel(
            ISettingsService settingsService,
            INotificationService notificationService,
            ILoggerService loggerService,
            IArchiveService archiveService
            )
        {
            _settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
            _notificationService = notificationService ?? Locator.Current.GetService<INotificationService>();
            _loggerService = loggerService ?? Locator.Current.GetService<ILoggerService>();
            _archiveService = archiveService;

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
            RoutingModsCommand = ReactiveCommand.Create(delegate ()
            {
                Router.Navigate.Execute(Locator.Current.GetService<ModListViewModel>());
            }, CanExecuteRoutingToMods);
             RoutingSearchCommand = ReactiveCommand.Create(delegate ()
            {
                Router.Navigate.Execute(Locator.Current.GetService<SearchViewModel>());
            }, CanExecuteRoutingToMods);
            RoutingCommand = ReactiveCommand.Create<Constants.RoutingIDs>(ExecuteSidebar,
                CanExecuteRouting);


            ToggleBottomBarCommand = ReactiveCommand.Create(() =>
            {
                IsBottomContentVisible = !IsBottomContentVisible;
            });

            CheckForUpdatesCommand = ReactiveCommand.CreateFromTask(CheckForUpdatesAsync);


        }

        private static async Task CheckForUpdatesAsync()
        {
            var http = new HttpClient();
            var manifestJson = await http.GetStringAsync(new Uri("https://github.com/rfuzzo/WolvenManager/releases/latest/download/manifest.json"));

            var manifest = JsonSerializer.Deserialize<Manifest>(manifestJson);
            if (manifest == null)
            {
                return;
            }

            var latestVersion = new Version(manifest.Version);
            var myVersion = CommonFunctions.GetAssemblyVersion(Constants.AssemblyName);

            if (latestVersion > myVersion)
            {
                if (MessageBox.Show(
                    $"You've got version {myVersion} of WolvenManager. Would you like to update to the latest version {latestVersion}?",
                    "Update SecretStartup?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    
                }
            }
        }


        #region properties

        [Reactive] public bool IsBottomContentVisible { get; set; } = true;

        private string _title;
        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public RoutingState Router { get; }

        #endregion

        #region commands


        public ReactiveCommand<Unit, Unit> ToggleBottomBarCommand { get; }


        private IObservable<bool> CanExecuteRouting => _settingsService.IsValid;
        private IObservable<bool> CanExecuteRoutingToMods => _archiveService.IsLoaded.ObserveOn(RxApp.MainThreadScheduler);


        public ReactiveCommand<Unit, Unit> RoutingSettingsCommand { get; }
        public ReactiveCommand<Unit, Unit> CheckForUpdatesCommand { get; }
        public ReactiveCommand<Unit, Unit> RoutingModsCommand { get; }
        public ReactiveCommand<Unit, Unit> RoutingSearchCommand { get; }
        public ReactiveCommand<Constants.RoutingIDs, Unit> RoutingCommand { get; }

        private void ExecuteSidebar(Constants.RoutingIDs parameter)
        {
            switch (parameter)
            {
                //case Constants.RoutingIDs.Mods:
                //    Router.Navigate.Execute(Locator.Current.GetService<ModListViewModel>());
                //    break;
                case Constants.RoutingIDs.Modkit:
                    Router.Navigate.Execute(Locator.Current.GetService<ModkitViewModel>());
                    break;
                case Constants.RoutingIDs.Settings:
                    Router.Navigate.Execute(Locator.Current.GetService<SettingsViewModel>());
                    break;
                case Constants.RoutingIDs.Mod:
                    break;
                //case Constants.RoutingIDs.Search:
                //    Router.Navigate.Execute(Locator.Current.GetService<SearchViewModel>());
                //    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parameter), parameter, null);
            }


            
        }

        #endregion

        private void OnStartup()
        {
            // Once 
            _settingsService.IsValid.Subscribe(async isvalid =>
            {
                if (!isvalid)
                {
                    return;
                }
                else
                {
                    // load oodle
                    var oodlePath = _settingsService.GetOodlePath();
                    OodleLoadLib.Load(oodlePath);

                    // load managers
                    await Task.Run( () => _archiveService.Load());
                }
            });

            // navigate to settings if game is not found
            var foundGamePath = _settingsService.GetGameRootPath();
            if (!string.IsNullOrEmpty(foundGamePath) && Directory.Exists(foundGamePath) && CommonHelpers.IsMainFolder(foundGamePath))
            {
                Router.Navigate.Execute(Locator.Current.GetService<ModkitViewModel>());
            }
            else
            {
                Router.Navigate.Execute(Locator.Current.GetService<SettingsViewModel>());
            }
        }

    }
}
