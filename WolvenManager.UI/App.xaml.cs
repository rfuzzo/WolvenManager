using System;
using ReactiveUI;
using Splat;
using WolvenKit.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Microsoft.Extensions.Logging;
using CP77.CR2W;
using CP77Tools.Tasks;
using WolvenKit.CLI;
using WolvenKit.Common.Model.Arguments;
using WolvenKit.Modkit.RED4;
using WolvenKit.Modkit.RED4.RigFile;
using WolvenKit.RED4.CR2W;
using WolvenManager.App.Services;
using WolvenManager.App.ViewModels;
using WolvenManager.App.ViewModels.PageViewModels;
using WolvenManager.UI.Implementations;
using WolvenManager.UI.Services;
using WolvenManager.UI.Views;
using System.Windows;

namespace WolvenManager.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App //: Application
    {
        public App()
        {
            // register synfusion
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(
                "NDM4Njc4QDMxMzkyZTMxMmUzMG5SV05xYWpUK3lBc3RKZE0vNnJsK09qYmx6YWppRmlFeEZlcjcwSnF2L0E9");

            Init();

        }

        public IServiceProvider Container { get; private set; }
        private IHost _host;

        void Init()
        {
            _host = Host
                .CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.UseMicrosoftDependencyResolver();
                    var resolver = Locator.CurrentMutable;
                    resolver.InitializeSplat();
                    resolver.InitializeReactiveUI();
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddSplat();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // local services
                    services.AddSingleton(typeof(ISettingsService), SettingsService.Load());
                    services.AddSingleton<INotificationService, NotificationService>();
                    services.AddSingleton<IInteractionService, InteractionService>();


                    // register your personal services here
                    services.AddSingleton<ILoggerService, ReactiveLoggerService>();
                    services.AddSingleton<IProgress<double>, PercentProgressService>();

                    services.AddSingleton<IHashService, HashService>();


                    services.AddSingleton<Red4ParserService>();
                    services.AddSingleton<TargetTools>();      //Cp77FileService
                    services.AddSingleton<RIG>();              //Cp77FileService
                    services.AddSingleton<MeshTools>();        //RIG, Cp77FileService

                    services.AddSingleton<ModTools>();         //Cp77FileService, ILoggerService, IProgress, IHashService, Mesh, Target

                    services.AddSingleton<IConsoleFunctions, ConsoleFunctions>();


                    // register viewModels
                    services.AddSingleton<AppViewModel>();
                    services.AddSingleton<ModListViewModel>();
                    services.AddSingleton<SettingsViewModel>();
                    services.AddSingleton<ModkitViewModel>();

                    // this passes IScreen resolution through to the previous viewmodel registration.
                    // this is to prevent multiple instances by mistake.

                    services.AddSingleton<IScreen, AppViewModel>(x => x.GetRequiredService<AppViewModel>());

                    // register views
                    services.AddSingleton<IViewFor<ModkitViewModel>, ModkitView>();
                    services.AddSingleton<IViewFor<AppViewModel>, MainWindow>();
                    services.AddSingleton<IViewFor<ModListViewModel>, ModListView>();
                    services.AddSingleton<IViewFor<SettingsViewModel>, SettingsView>();

                    ////Locator.CurrentMutable.Register(() => new ModFilesValidationView(), typeof(IViewFor<ModFilesValidationViewModel>));


                    //Locator.CurrentMutable.RegisterConstant(
                    //    new BooleanToVisibilityTypeConverter(),
                    //    typeof(IBindingTypeConverter)
                    //);
                })
                .UseEnvironment(Environments.Development)
                .Build();

            // Since MS DI container is a different type,
            // we need to re-register the built container with Splat again
            Container = _host.Services;
            Container.UseMicrosoftDependencyResolver();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();

            var mainWindow = _host.Services.GetService<IViewFor<AppViewModel>>();
            if (mainWindow is MainWindow window)
            {
                window.Show();
            }
            
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
        }
    }
}
