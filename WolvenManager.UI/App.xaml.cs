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
using WolvenManager.App.Services;
using WolvenManager.App.ViewModels;
using WolvenManager.App.ViewModels.Dialogs;
using WolvenManager.App.ViewModels.PageViewModels;
using WolvenManager.UI.Implementations;
using WolvenManager.UI.Services;
using WolvenManager.UI.Views;
using WolvenManager.UI.Views.Dialogs;
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
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("@31392e312e30Sg4FiMStw/tz47UG2h7GTXSEOnz622pb1XyrSzFw5d0=");

            // register Services

            Locator.CurrentMutable.RegisterConstant(SettingsService.Load(), typeof(ISettingsService));
            Locator.CurrentMutable.RegisterConstant(new NotificationService(), typeof(INotificationService));
            Locator.CurrentMutable.RegisterConstant(new InteractionService(), typeof(IInteractionService));

            Locator.CurrentMutable.RegisterConstant(new PluginService(), typeof(IPluginService));

            // register VieModels
            Locator.CurrentMutable.Register(() => new MainWindow(), typeof(IViewFor<AppViewModel>));
            Locator.CurrentMutable.Register(() => new SettingsView(), typeof(IViewFor<SettingsViewModel>));
            Locator.CurrentMutable.Register(() => new ModListView(), typeof(IViewFor<ModListViewModel>));
            
            //Locator.CurrentMutable.Register(() => new ModFilesValidationView(), typeof(IViewFor<ModFilesValidationViewModel>));


            Locator.CurrentMutable.RegisterConstant(
                new BooleanToVisibilityTypeConverter(),
                typeof(IBindingTypeConverter)
            );



        }
    }
}
