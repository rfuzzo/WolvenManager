using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using SharpCompress.Readers;
using Splat;
using WolvenManager.App.Arguments;
using WolvenManager.App.Attributes;
using WolvenManager.App.Models;
using WolvenManager.App.Services;
using WolvenManager.App.Utility;

namespace WolvenManager.App.ViewModels.PageViewModels
{
    [RoutingUrl(Constants.RoutingIDs.Main)]
    public class ModListViewModel : PageViewModel
    {
        public ModListViewModel(IScreen screen = null) : base(typeof(ModListViewModel), screen)
        {
            var currentProfile = Locator.Current.GetService<IProfileService>();
            currentProfile.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out BindingData)
                .Subscribe();

            InstallModCommand = ReactiveCommand.CreateFromTask(InstallMod);
            ToggleSidebarCommand = ReactiveCommand.Create(() =>
            {
                IsSideBarVisible = !IsSideBarVisible;
            });
        }

        private async Task InstallMod()
        {
            var browseFiles = _interactionService.BrowseFiles("zip files (*.zip;*.7z)|*.zip;*.7z");

            foreach (var path in browseFiles.Select(_ => new FileInfo(_)))
            {
                // extract to mod folder
                var mod = await ModInstallHelper.InstallMod(path);
                if (mod == null)
                {
                    return;
                }

                // add to library class
                // and check if it exists
                _libraryService.AddModToLibrary(mod);


                // move zip to file lib
                if (_settingsService.IsLibraryEnabled)
                {
                    try
                    {
                        var modname = Path.GetFileNameWithoutExtension(path.FullName);
                        var modLibDir = Path.Combine(_settingsService.DepotPath, modname);
                        if (!Directory.Exists(modLibDir))
                        {
                            Directory.CreateDirectory(modLibDir);
                        }
                        path.MoveTo(Path.Combine(modLibDir, path.Name));
                        mod.IsInPhysicalLibrary = true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
            }
        }

        


        #region properties
        [Reactive]
        public bool IsSideBarVisible { get; set; }

        public readonly ReadOnlyObservableCollection<ModViewModel> BindingData;

        public ReactiveCommand<Unit, Unit> InstallModCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleSidebarCommand { get; }


        #endregion

    }
}
