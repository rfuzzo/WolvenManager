using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Cryptography;
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
using WolvenManager.App.Utility;
using Microsoft.Extensions.Hosting;
using WolvenManager.Installer;

namespace WolvenManager.App.ViewModels
{

    public class AppViewModel : MainViewModel, IScreen
    {
        #region fields

        private readonly ISettingsService _settingsService;
        private readonly INotificationService _notificationService;
        private readonly ILoggerService _loggerService;
        private readonly IArchiveService _archiveService;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IUpdateService _updateService;

        
        


        #endregion

        #region ctor
        public AppViewModel(
            ISettingsService settingsService,
            INotificationService notificationService,
            ILoggerService loggerService,
            IArchiveService archiveService,
            IHostApplicationLifetime appLifetime,
            IUpdateService updateService
        )
        {
            _settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
            _notificationService = notificationService ?? Locator.Current.GetService<INotificationService>();
            _loggerService = loggerService ?? Locator.Current.GetService<ILoggerService>();
            _archiveService = archiveService;
            _appLifetime = appLifetime;
            _updateService = updateService;


            // routing
            Router = new RoutingState();

            // versioning
            
            var assembly = CommonFunctions.GetAssembly(Constants.AssemblyName);
            var productName = assembly.GetName();
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

            CheckForUpdatesCommand = ReactiveCommand.CreateFromTask(_updateService.CheckForUpdatesAsync);

            
        }

        #endregion

        #region properties

        
        [Reactive] public double Progress { get; set; }
        [Reactive] public string Version { get; set; }

        [Reactive] public bool IsBottomContentVisible { get; set; } = true;
        public string SettingsIconName => _updateService.IsUpdateAvailable ? "CogRefresh" : "Cog";

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

        public ReactiveCommand<Unit, Unit> RoutingSettingsCommand { get; }

        public ReactiveCommand<Unit, Unit> CheckForUpdatesCommand { get; }
        public ReactiveCommand<Unit, Unit> RoutingModsCommand { get; }
        private IObservable<bool> CanExecuteRoutingToMods => _archiveService.IsLoaded.ObserveOn(RxApp.MainThreadScheduler);

        public ReactiveCommand<Unit, Unit> RoutingSearchCommand { get; }
        public ReactiveCommand<Constants.RoutingIDs, Unit> RoutingCommand { get; }
        private IObservable<bool> CanExecuteRouting => _settingsService.IsValid;

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

        #region methods

        private async void OnStartup()
        {
            _updateService.Init(Constants.UpdateUrl, Constants.AssemblyName, delegate(FileInfo path)
            {
                var proc = Process.Start(path.FullName, "/SILENT /NOCANCEL");

                Application.Current.Shutdown();
            });

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

                    // Check for updates
                    await _updateService.CheckForUpdatesAsync();
                }
            });

            // navigate to settings if game is not found
            var foundGamePath = _settingsService.GetGameRootPath();
            if (!string.IsNullOrEmpty(foundGamePath) && Directory.Exists(foundGamePath) && CommonHelpers.IsMainFolder(foundGamePath))
            {
                await Router.Navigate.Execute(Locator.Current.GetService<ModkitViewModel>());
            }
            else
            {
                await Router.Navigate.Execute(Locator.Current.GetService<SettingsViewModel>());
            }
        }

        

        #endregion
    }
}
