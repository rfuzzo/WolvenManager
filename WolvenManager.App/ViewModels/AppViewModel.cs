using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;
using Splat;
using WolvenKit.Common.Services;
using WolvenManager.App.Services;
using WolvenManager.App.ViewModels.PageViewModels;
using ReactiveUI.Fody.Helpers;
using WolvenKit.Common.Tools;
using WolvenKit.Common.Tools.Oodle;
using WolvenManager.App.Utility;
using Microsoft.Extensions.Hosting;
using WolvenKit.Common;
using WolvenKit.Core;
using WolvenKit.Core.Services;
using WolvenManager.Installer.Commands;
using WolvenManager.Installer.Services;

namespace WolvenManager.App.ViewModels
{

    public class AppViewModel : MainViewModel, IScreen
    {
        #region fields

        private readonly ISettingsService _settingsService;
        private readonly INotificationService _notificationService;
        private readonly ILoggerService _loggerService;
        private readonly IArchiveManager _archiveService;
        private readonly IUpdateService _updateService;
        private readonly IProgressService<double> _progressService;

        
        

        
        #endregion

        #region ctor
        public AppViewModel(
            ISettingsService settingsService,
            INotificationService notificationService,
            ILoggerService loggerService,
            IArchiveManager archiveService,
            IUpdateService updateService,
            IProgressService<double> progressService
        )
        {
            _settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
            _notificationService = notificationService ?? Locator.Current.GetService<INotificationService>();
            _loggerService = loggerService ?? Locator.Current.GetService<ILoggerService>();
            _archiveService = archiveService;
            _updateService = updateService;
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
           

            _updateService.Init(new string[] {Constants.UpdateUrl}, Constants.AssemblyName, delegate(FileInfo path, bool isManaged)
            {
                if (isManaged)
                {
                    _ = Process.Start(path.FullName, "/SILENT /NOCANCEL");
                }
                else
                {
                    // move installer helper
                    var basedir = new DirectoryInfo(Path.GetDirectoryName(System.AppContext.BaseDirectory));
                    var shippedInstaller = new FileInfo(Path.Combine(basedir.FullName, "lib", Constants.UpdaterName));
                    var newPath = Path.Combine(_settingsService.GetAppData(), Constants.UpdaterName);
                    try
                    {
                        shippedInstaller.MoveTo(newPath, true);
                    }
                    catch (Exception)
                    {
                        _loggerService.Error("Could not initialize auto-installer.");
                        return;
                    }

                    // start installer helper
                    var psi = new ProcessStartInfo
                    {
                        FileName = "CMD.EXE",
                        Arguments = $"/K {newPath} install -i \"{path}\" -o \"{basedir.FullName}\" -r {Constants.AppName}"
                    };
                    var p = Process.Start(psi);
                }

                Application.Current.Shutdown();
            }, _notificationService.AskInApp);

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
                    await Task.Run( () => _archiveService.LoadFromFolder(new DirectoryInfo(_settingsService.GetArchiveDirectoryPath())));

                    // Check for updates
                    await _updateService.CheckForUpdatesAsync();

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
