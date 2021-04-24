using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Splat;
using WolvenManager.App.Services;
using WolvenManager.App.ViewModels.PageViewModels;

namespace WolvenManager.App.ViewModels
{

    public class AppViewModel : MainViewModel, IScreen
    {
        private readonly ISettingsService _settingsService;
        private readonly IPluginService _pluginService;

        public AppViewModel()
        {
            _settingsService = Locator.Current.GetService<ISettingsService>();
            _pluginService = Locator.Current.GetService<IPluginService>();

            // routing
            Router = new RoutingState();

            // versioning
            var assembly = Assembly.GetExecutingAssembly();
            var productName = ((AssemblyProductAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute), false))?.Product;
            Version = assembly.GetName().Version?.ToString();
            Title = $"{productName}-{Version}";


            OnStartup();

            // commands
            SidebarCommand = ReactiveCommand.Create<Constants.RoutingIDs>(ExecuteSidebar, CanExecuteRouting);
        }

        #region properties

        private IObservable<bool> CanExecuteRouting => _settingsService.IsValid();

        private string title;
        public string Title
        {
            get => title;
            set => this.RaiseAndSetIfChanged(ref title, value);
        }

        private string Version { get; set; }


        public RoutingState Router { get; }

        #endregion



        #region commands
        


        public ReactiveCommand<Constants.RoutingIDs, Unit> SidebarCommand { get; }

        private void ExecuteSidebar(Constants.RoutingIDs parameter)
        {
            switch (parameter)
            {
                case Constants.RoutingIDs.Main:
                    Router.Navigate.Execute(new ModListViewModel());
                    break;
                case Constants.RoutingIDs.Library:
                    break;
                case Constants.RoutingIDs.Extensions:
                    break;
                case Constants.RoutingIDs.Profiles:
                    break;
                case Constants.RoutingIDs.Settings:
                    Router.Navigate.Execute(new SettingsViewModel());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parameter), parameter, null);
            }


            
        }

        #endregion

        public void OnStartup()
        {
            Task.Run(() => _pluginService.Init());

            // Once 
            _settingsService.IsValid().Subscribe(isvalid =>
            {
                //TODO: bad?
                if (!isvalid)
                {
                    return;
                }

                Locator.CurrentMutable.RegisterConstant(new WatcherService(), typeof(IWatcherService));
                // requires IWatcherService
                Locator.CurrentMutable.RegisterConstant(new LibraryService(), typeof(ILibraryService));
                // requires ILibraryService
                Locator.CurrentMutable.RegisterConstant(new ProfileService(), typeof(IProfileService));


                var watcher = Locator.Current.GetService<IWatcherService>();
                // check all loose files
                watcher.RefreshAsync();

                // check Addons updates


            });


            // navigate to settings if game is not found
            if (string.IsNullOrEmpty(_settingsService.GamePath))
            {
                Router.Navigate.Execute(new SettingsViewModel());
            }
            else
            {
                Router.Navigate.Execute(new ModListViewModel());
            }
        }

    }
}
