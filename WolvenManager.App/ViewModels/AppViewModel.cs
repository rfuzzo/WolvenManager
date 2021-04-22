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

namespace WolvenManager.App.ViewModels
{

    public class AppViewModel : MainViewModel, IScreen
    {
        private readonly IAppSettingsService settings;

        public AppViewModel()
        {

            settings = Locator.Current.GetService<IAppSettingsService>();
            settings.Load();

            


            // routing
            Router = new RoutingState();

            // versioning
            var assembly = Assembly.GetExecutingAssembly();
            var productName = ((AssemblyProductAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute), false))?.Product;
            Version = assembly.GetName().Version?.ToString();
            Title = $"{productName}-{Version}";



            // commands
            SidebarCommand = ReactiveCommand.Create<Constants.Constants.RoutingIDs>(ExecuteSidebarCommand, CanExecuteRouting);


            settings.IsValid().Subscribe(_ =>
            {
                Locator.CurrentMutable.RegisterConstant( new WatcherService(), typeof(IWatcherService));
                Locator.CurrentMutable.RegisterConstant(new ProfileService(), typeof(IProfileService));
            });


            // navigate to settings if game is not found
            if (string.IsNullOrEmpty(settings.GamePath))
            {
                Router.Navigate.Execute(new SettingsViewModel());
            }
            else
            {
                Router.Navigate.Execute(new ModListViewModel());
                
            }
        }

        #region properties

        private IObservable<bool> CanExecuteRouting => settings.IsValid();

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
        


        public ReactiveCommand<Constants.Constants.RoutingIDs, Unit> SidebarCommand { get; }

        private void ExecuteSidebarCommand(Constants.Constants.RoutingIDs parameter)
        {
            switch (parameter)
            {
                case Constants.Constants.RoutingIDs.Main:
                    Router.Navigate.Execute(new ModListViewModel());
                    break;
                case Constants.Constants.RoutingIDs.Library:
                    break;
                case Constants.Constants.RoutingIDs.Extensions:
                    break;
                case Constants.Constants.RoutingIDs.Profiles:
                    break;
                case Constants.Constants.RoutingIDs.Settings:
                    Router.Navigate.Execute(new SettingsViewModel());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(parameter), parameter, null);
            }


            
        }

        #endregion



    }
}
