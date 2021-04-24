using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using WolvenManager.App.Models;
using WolvenManager.App.ViewModels;

namespace WolvenManager.App.Services
{
    /// <summary>
    ///  
    /// </summary>
    public class ProfileService : ReactiveObject, IProfileService
    {
        private readonly INotificationService _notificationService;
        private readonly ISettingsService _settingsService;
        private readonly ILibraryService _libraryService;
        private readonly IPluginService _pluginService;

        // bound library
        private readonly ReadOnlyObservableCollection<ModViewModel> _items;
        public ReadOnlyObservableCollection<ModViewModel> Items => _items;


        public ProfileService()
        {
            _notificationService = Locator.Current.GetService<INotificationService>();
            _settingsService = Locator.Current.GetService<ISettingsService>();
            _libraryService = Locator.Current.GetService<ILibraryService>();
            _pluginService = Locator.Current.GetService<IPluginService>();


            _libraryService.Connect()
                .Transform(_ => new ModViewModel(_))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _items)
                .Subscribe(set =>
                {
                    // observe changes in the library
                });



        }

        


        public async Task Save()
        {
            


        }





    }
}
