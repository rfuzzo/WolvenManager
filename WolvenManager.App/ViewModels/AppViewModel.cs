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
using WolvenManager.App.Models;
using WolvenManager.App.Utility;
using Microsoft.Extensions.Hosting;

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

        

        #endregion

        #region ctor
        public AppViewModel(
            ISettingsService settingsService,
            INotificationService notificationService,
            ILoggerService loggerService,
            IArchiveService archiveService,
            IHostApplicationLifetime appLifetime
        )
        {
            _settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
            _notificationService = notificationService ?? Locator.Current.GetService<INotificationService>();
            _loggerService = loggerService ?? Locator.Current.GetService<ILoggerService>();
            _archiveService = archiveService;
            _appLifetime = appLifetime;

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

        #endregion

        #region properties

        [Reactive] public bool IsUpdateAvailable { get; set; }
        [Reactive] public bool IsUpdateReadyToInstall { get; set; }
        [Reactive] public double Progress { get; set; }
        [Reactive] public string Version { get; set; }

        [Reactive] public bool IsBottomContentVisible { get; set; } = true;
        public string SettingsIconName => IsUpdateAvailable ? "CogRefresh" : "Cog";

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
                    await CheckForUpdatesAsync();
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

        private async Task CheckForUpdatesAsync()
        {
            var http = new HttpClient();
            var manifestJson = await http.GetStringAsync(new Uri(Constants.RemoteManifest));

            var manifest = JsonSerializer.Deserialize<Manifest>(manifestJson);
            if (manifest == null)
            {
                return;
            }

            var latestVersion = new Version(manifest.Version);
            var myVersion = CommonFunctions.GetAssemblyVersion(Constants.AssemblyName);

            if (latestVersion > myVersion)
            {
                IsUpdateAvailable = true;

                // check if portable
                // TODO

                {
                    // check if update already downloaded before
                    var physicalPath = new FileInfo(Path.Combine(_settingsService.GetTempDir(), manifest.Installer.Key));
                    if (physicalPath.Exists)
                    {
                        using (var mySha256 = SHA256.Create())
                        {
                            var hash = CommonFunctions.HashFile(physicalPath, mySha256);
                            if (manifest.Installer.Value.Equals(hash))
                            {
                                HandleUpdateFromFile(physicalPath);
                            }
                            else
                            {
                                // hash was not matching, redownload from remote
                                try
                                {
                                    physicalPath.Delete();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                    throw;
                                }
                                finally
                                {
                                    await DownloadUpdateAsync(manifest, manifest.Installer.Key);
                                }
                            }
                        }
                    }
                    else
                    {
                        await DownloadUpdateAsync(manifest, manifest.Installer.Key);
                    }
                }
            }
        }

        private async Task DownloadUpdateAsync(Manifest manifest, string key)
        {
            var latestVersion = new Version(manifest.Version);
            var myVersion = CommonFunctions.GetAssemblyVersion(Constants.AssemblyName);
            if (MessageBox.Show(
                $"You've got version {myVersion} of {Constants.ProductName}. Would you like to update to the latest version {latestVersion}?",
                $"Update {Constants.ProductName}?", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }



            using (var wc = new WebClient())
            {
                var dlObservable = Observable.FromEventPattern<DownloadProgressChangedEventHandler, DownloadProgressChangedEventArgs>(
                    handler => wc.DownloadProgressChanged += handler,
                    handler => wc.DownloadProgressChanged -= handler);
                var dlCompleteObservable = Observable.FromEventPattern<AsyncCompletedEventHandler, AsyncCompletedEventArgs>(
                    handler => wc.DownloadFileCompleted += handler,
                    handler => wc.DownloadFileCompleted -= handler);

                _ = dlObservable
                    .Select(_ => (double)_.EventArgs.ProgressPercentage)
                    .Subscribe(d =>
                    {
                        Progress = d;
                    });

                _ = dlCompleteObservable
                    .Select(_ => _.EventArgs)
                    .Subscribe(c =>
                    {
                        OnDownloadCompletedCallback(c, manifest, key);
                    });

                var uri = new Uri(Constants.LatestRelease + key);
                var physicalPath = Path.Combine(_settingsService.GetTempDir(), key);
                wc.DownloadFileAsync(uri, physicalPath);
            }

            await Task.CompletedTask;
        }

        private void OnDownloadCompletedCallback(AsyncCompletedEventArgs e, Manifest manifest, string key)
        {
            if (e.Cancelled)
            {
                _loggerService.Info("File download cancelled.");
            }

            if (e.Error != null)
            {
                _loggerService.Error(e.Error.ToString());
            }

            // check downloaded file
            var physicalPath = new FileInfo(Path.Combine(_settingsService.GetTempDir(), manifest.Installer.Key));
            if (physicalPath.Exists)
            {
                using (var mySha256 = SHA256.Create())
                {
                    var hash = CommonFunctions.HashFile(physicalPath, mySha256);
                    if (manifest.Installer.Value.Equals(hash))
                    {
                        HandleUpdateFromFile(physicalPath);
                    }
                    else
                    {
                        _loggerService.Error("Downloaded file does not match expected file.");
                    }
                }
            }
            else
            {
                _loggerService.Error("File download failed.");
            }
        }

        private void HandleUpdateFromFile(FileInfo path)
        {
            IsUpdateAvailable = false;
            IsUpdateReadyToInstall = true;

            // handle update
            // TODO:portable

            // installer

            // ask user to restart
            if (MessageBox.Show(
                $"Update available. Restart?",
                $"Update {Constants.ProductName}?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var proc = Process.Start(path.FullName, "/SILENT /NOCANCEL");

                //System.Windows.Forms.Application.Restart();
                System.Windows.Application.Current.Shutdown();
            }

            // run installer

        }

        #endregion
    }
}
