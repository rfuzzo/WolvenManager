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
        //private readonly ReadOnlyObservableCollection<ModViewModel> _items;
        //public ReadOnlyObservableCollection<ModViewModel> Items => _items;
        private readonly SourceCache<ModViewModel, string> _modViewModels = new(t => t.Id);
        /// <summary>
        /// Connection to the current profile
        /// </summary>
        /// <returns></returns>
        public IObservable<IChangeSet<ModViewModel, string>> Connect() => _modViewModels.Connect();


        public ProfileService()
        {
            _notificationService = Locator.Current.GetService<INotificationService>();
            _settingsService = Locator.Current.GetService<ISettingsService>();
            _libraryService = Locator.Current.GetService<ILibraryService>();
            _pluginService = Locator.Current.GetService<IPluginService>();


            _libraryService.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _)
                .Subscribe(set =>
                {
                    foreach (var change in set)
                    {
                        EvaluateChange(change);
                    }
                });
        }

        private void EvaluateChange(Change<ModModel, string> change)
        {
            var model = change.Current;
            switch (change.Reason)
            {
                case ChangeReason.Add:
                    // check if already in profile
                    if (_modViewModels.Keys.Contains(model.Id))
                    {
                        // do nothing?
                        // TODO: could do something fancy and hash the file and compare to chached mod
                    }
                    else
                    {
                        // a mod was added to the lib but isn't here
                        var modVm = new ModViewModel(model)
                        {
                            LoadOrder = _modViewModels.Count
                        };

                        // add disabled files :(
                        foreach (var file in model.Files)
                        {
                            var gamefile = Path.Combine(_settingsService.GamePath, $"{file}.disabled");
                            if (File.Exists(gamefile))
                            {
                                modVm.DisabledFiles.Add(file);
                            }
                        }

                        // priority
                        

                        modVm.Enabled = true; //default enable new mods;
                        _modViewModels.AddOrUpdate(modVm);
                    }


                    break;
                case ChangeReason.Update:
                    break;
                case ChangeReason.Remove:
                    break;
                case ChangeReason.Refresh:
                    break;
                case ChangeReason.Moved:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task Serialize()
        {
            


        }





    }
}
