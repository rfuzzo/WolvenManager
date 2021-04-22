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
using WolvenManager.UI.Services;
using WolvenManager.UI.Views;

namespace WolvenManager.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());

            Locator.CurrentMutable.Register(() => new SettingsView(), typeof(IViewFor<SettingsViewModel>));
            Locator.CurrentMutable.Register(() => new ModListView(), typeof(IViewFor<ModListViewModel>));


            Locator.CurrentMutable.RegisterConstant(new AppSettingsService(), typeof(IAppSettingsService));
            //Locator.CurrentMutable.RegisterConstant(new NotificationService(), typeof(INotificationService));
            
            

        }
    }
}
