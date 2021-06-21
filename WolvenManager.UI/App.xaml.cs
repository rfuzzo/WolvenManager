using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;
using Splat;
using WolvenKit.Common.Services;
using WolvenManager.App.Services;
using WolvenManager.App.ViewModels;
using WolvenManager.App.ViewModels.Dialogs;
using WolvenManager.App.ViewModels.PageViewModels;
using WolvenManager.UI.Implementations;
using WolvenManager.UI.Services;
using WolvenManager.UI.Views;
using INotificationService = WolvenManager.App.Services.INotificationService;

namespace WolvenManager.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // register synfusion
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDM4Njc4QDMxMzkyZTMxMmUzMG5SV05xYWpUK3lBc3RKZE0vNnJsK09qYmx6YWppRmlFeEZlcjcwSnF2L0E9");

            // register Services

            Locator.CurrentMutable.RegisterConstant(SettingsService.Load(), typeof(ISettingsService));
            Locator.CurrentMutable.RegisterConstant(new NotificationService(), typeof(INotificationService));
            Locator.CurrentMutable.RegisterConstant(new InteractionService(), typeof(IInteractionService));

            Locator.CurrentMutable.RegisterConstant(new LoggerService(), typeof(ILoggerService));
            Locator.CurrentMutable.RegisterConstant(new HashService(), typeof(IHashService));

            // register VieModels
            Locator.CurrentMutable.Register(() => new MainWindow(), typeof(IViewFor<AppViewModel>));
            Locator.CurrentMutable.Register(() => new ModListView(), typeof(IViewFor<ModListViewModel>));
            Locator.CurrentMutable.Register(() => new SettingsView(), typeof(IViewFor<SettingsViewModel>));

            //Locator.CurrentMutable.Register(() => new ModFilesValidationView(), typeof(IViewFor<ModFilesValidationViewModel>));


            Locator.CurrentMutable.RegisterConstant(
                new BooleanToVisibilityTypeConverter(),
                typeof(IBindingTypeConverter)
            );



        }
    }
}
