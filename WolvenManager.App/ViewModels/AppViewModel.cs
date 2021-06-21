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
        private readonly INotificationService _notificationService;

        public AppViewModel()
        {
            _settingsService = Locator.Current.GetService<ISettingsService>();
            _notificationService = Locator.Current.GetService<INotificationService>();

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
                Router.Navigate.Execute(new SettingsViewModel(this));
            }, CanExecuteRouting);
            RoutingCommand = ReactiveCommand.Create<Constants.RoutingIDs>(ExecuteSidebar, CanExecuteRouting);
        }


        #region properties

        private IObservable<bool> CanExecuteRouting => _settingsService.IsValid;

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


        public ReactiveCommand<Unit, Unit> RoutingSettingsCommand { get; }
        public ReactiveCommand<Constants.RoutingIDs, Unit> RoutingCommand { get; }

        private void ExecuteSidebar(Constants.RoutingIDs parameter)
        {
            switch (parameter)
            {
                case Constants.RoutingIDs.Main:
                    Router.Navigate.Execute(new ModListViewModel(this));
                    break;
                case Constants.RoutingIDs.Settings:
                    Router.Navigate.Execute(new SettingsViewModel(this));
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
                //TODO: bad?
                if (!isvalid)
                {
                    return;
                }

                // check Addons updates


            });


            // navigate to settings if game is not found
            if (string.IsNullOrEmpty(_settingsService.GamePath))
            {
                Router.Navigate.Execute(new SettingsViewModel(this));
            }
            else
            {
                Router.Navigate.Execute(new ModListViewModel(this));
            }
        }

    }
}
