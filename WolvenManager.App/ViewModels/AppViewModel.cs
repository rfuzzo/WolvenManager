using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Hosting;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using WolvenKit.Common;
using WolvenKit.Common.Services;
using WolvenKit.Core;
using WolvenKit.Core.Compression;
using WolvenKit.Core.Services;
using WolvenManager.App.Services;
using WolvenManager.App.Utility;
using WolvenManager.App.ViewModels.PageViewModels;

namespace WolvenManager.App.ViewModels
{

    public class AppViewModel : MainViewModel, IScreen
    {
        private readonly ISettingsService _settingsService;
        private readonly INotificationService _notificationService;
        private readonly ILoggerService _loggerService;
        private readonly IArchiveManager _archiveService;
        private readonly IProgressService<double> _progressService;

        #region ctor
        public AppViewModel(
            ISettingsService settingsService,
            INotificationService notificationService,
            ILoggerService loggerService,
            IArchiveManager archiveService,
            IProgressService<double> progressService
        )
        {
            _settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
            _notificationService = notificationService ?? Locator.Current.GetService<INotificationService>();
            _loggerService = loggerService ?? Locator.Current.GetService<ILoggerService>();
            _archiveService = archiveService;
            _progressService = progressService;

            // routing
            Router = new RoutingState();

            // versioning

            var assembly = CommonFunctions.GetAssembly(Constants.AssemblyName);
            var productName = assembly.GetName();
            Version = assembly.GetName().Version?.ToString();
            Title = $"{productName}-{Version}";

            OnStartup();

            // commands
            RoutingSettingsCommand = ReactiveCommand.Create(delegate ()
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

            ToggleBottomBarCommand = ReactiveCommand.Create(delegate ()
            {
                IsBottomContentVisible = !IsBottomContentVisible;
            });

            _ = Observable.FromEventPattern<EventHandler<double>, double>(
                handler => _progressService.ProgressChanged += handler,
                handler => _progressService.ProgressChanged -= handler)
                .Select(_ => _.EventArgs * 100)
                .ToProperty(this, x => x.Progress, out _progress);
        }


        #endregion

        #region properties

        private readonly ObservableAsPropertyHelper<double> _progress;
        public double Progress => _progress.Value;



        [Reactive] public string Version { get; set; }

        [Reactive] public bool IsBottomContentVisible { get; set; } = true;

        public string SettingsIconName => "Cog";

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

        public ReactiveCommand<Unit, Unit> RoutingModsCommand { get; }

        private IObservable<bool> CanExecuteRoutingToMods => _archiveService.WhenAny(x => x.IsManagerLoaded, b => b.Value == true);

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
                    if (!Oodle.Load())
                    {
                        throw new FileNotFoundException($"oo2ext_7_win64.dll not found.");
                    }

                    // load managers
                    await Observable.Start(() => _archiveService.LoadFromFolder(new DirectoryInfo(_settingsService.GetArchiveDirectoryPath())), RxApp.MainThreadScheduler);

                    // Cleanup temp folders
                    var installerPath = Path.Combine(Path.GetTempPath(), $"{Constants.ProductName}-installer-{Version}.exe");
                    TryFileDelete(installerPath);
                    var portablePath = Path.Combine(Path.GetTempPath(), $"{Constants.ProductName}-{Version}.zip");
                    TryFileDelete(installerPath);
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

        private static void TryFileDelete(string installername)
        {
            if (!File.Exists(installername))
            {
                return;
            }

            try
            {
                File.Delete(installername);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
    }
}
