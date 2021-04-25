using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using SharpCompress.Readers;
using Splat;
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
                await InstallMod(path);
                

                // move to file lib
                if (_settingsService.IsLibraryEnabled)
                {
                    var modname = Path.GetFileNameWithoutExtension(path.FullName);
                    var modLibDir = Path.Combine(_settingsService.DepotPath, modname);
                    if (!Directory.Exists(modLibDir))
                    {
                        Directory.CreateDirectory(modLibDir);
                    }
                    path.MoveTo(Path.Combine(modLibDir, path.Name));
                }

                // add to library class

            }
        }

        

        private static void Extract7zMod(FileInfo zipInfo)
        {
            
                


            

        }

        private static void ExtractZipMod(FileInfo zipInfo)
        {
            //foreach (var entry in entries)
            //{
            //    string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));
            //    if (!destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
            //    {
            //        continue;
            //    }

            //    if (IsPathDirectory(destinationPath))
            //    {
            //        if (Directory.Exists(destinationPath))
            //        {
            //            continue;
            //        }
            //        Directory.CreateDirectory(destinationPath);
            //    }
            //    else
            //    {
            //        if (File.Exists(destinationPath))
            //        {
            //            throw new NotImplementedException();
            //        }
            //        entry.ExtractToFile(destinationPath);
            //    }
            //}
        }


        private async Task<ModModel> InstallMod(FileInfo zipInfo)
        {
            var settingsService = Locator.Current.GetService<ISettingsService>();
            var notificationService = Locator.Current.GetService<INotificationService>();

            if (settingsService == null)
            {
                return null;
            }
            var extractPath = Path.GetFullPath(settingsService.GamePath);
            if (!PathExtensions.IsPathDirectory(extractPath))
            {
                extractPath += Path.DirectorySeparatorChar;
            }

            var entries = GetEntries(zipInfo).ToList();
            var files = entries.Where(_ => !PathExtensions.IsPathDirectory(_));
            // check if mod is malformed
            // it needs to have at least r6 or archive in it
            // TODO
            if (!(entries.Any(_ => _ == "r6/scripts") || entries.Any(_ => _ == "archive/pc/mod")))
            {
                var recovery = await InteractionHelpers.ModViewModelInteraction.Handle(files);

                if (recovery)
                {

                }


                throw new NotImplementedException();
                // open a view

                // suggest a structure

                // suggest resaving the new mod in the library
            }

            
            // do something about existing files

            
            //extract




            notificationService?.Success($"{Path.GetFileNameWithoutExtension(zipInfo.FullName)} installed.");

            return new ModModel()
            {
                Installed = true,
                Files = files
            };

        }

        private static IEnumerable<string> GetEntries(FileInfo zipInfo)
        {
            switch (zipInfo.Extension)
            {
                case ".zip":
                {
                    using Stream stream = File.OpenRead(zipInfo.FullName);
                    using var reader = ReaderFactory.Open(stream);
                    var zipEntries = new List<string>();
                    while (reader.MoveToNextEntry())
                    {
                        zipEntries.Add(reader.Entry.Key);
                    }

                    return zipEntries;
                }
                case ".7z":
                {
                    using var archive = SevenZipArchive.Open(zipInfo.FullName);
                    return archive.Entries.Select(_ => _.Key);
                }
                default:
                    throw new ArgumentOutOfRangeException();
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
