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
                var mod = await InstallMod(path);
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

        private static void ExtractZipMod(string zipPath, string extractPath, ZipModifyArgs e)
        {
            using Stream stream = File.OpenRead(zipPath);
            using var reader = ReaderFactory.Open(stream);
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    if (e != null && e.Output.Any(_ => _.Name == reader.Entry.Key))
                    {
                        var newEntry = e.Output.First(_ => _.Name == reader.Entry.Key);
                        string destinationPath = Path.GetFullPath(Path.Combine(extractPath, newEntry.Name));
                        if (!destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                        {
                            continue;
                        }
                        reader.WriteEntryToDirectory(extractPath, new ExtractionOptions()
                        {
                            ExtractFullPath = false,
                            Overwrite = true
                        });
                    }
                    else
                    {
                        reader.WriteEntryToDirectory(extractPath, new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });
                    }
                }
            }
        }


        private static async Task<ModModel> InstallMod(FileInfo zipInfo)
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

            var entries = TryGetEntries(zipInfo).ToList();
            var files = entries
                .Where(_ => !_.IsDirectory)
                .ToList();
            ZipModifyArgs recovery = null;
            // check if mod is malformed
            if ((files.Any(_ => Path.GetExtension(_.Name) == ".reds") && entries.All(_ => _.Name != "r6/scripts")) || 
                (files.Any(_ => Path.GetExtension(_.Name) == ".archive") && entries.All(_ => _.Name != "archive/pc/mod")))
            {
                // open mod fixer view
                recovery = await InteractionHelpers.ModViewModelInteraction.Handle(files);

                if (recovery.Output == null)
                {
                    // canceled
                    return null;
                }
            }

            // do something about existing files


            //extract
            switch (zipInfo.Extension)
            {
                case ".zip":
                    ExtractZipMod(zipInfo.FullName, extractPath, recovery);
                    break;
                case ".7z":
                    throw new NotImplementedException();
                    break;
                default:
                    break;
            }


            notificationService?.Success($"{Path.GetFileNameWithoutExtension(zipInfo.FullName)} installed.");
            
            var modname = Path.GetFileNameWithoutExtension(zipInfo.FullName);
            // sanitize modname
            modname = modname.Split('-').FirstOrDefault();
            //TODO: some user interaction

            return new ModModel()
            {
                Installed = true,
                Files = files.Select(_ => _.Name),
                Name = modname
            };

        }

        private static IEnumerable<ModFileModel> TryGetEntries(FileInfo zipInfo)
        {
            try
            {
                switch (zipInfo.Extension)
                {
                    case ".zip":
                    {
                        using Stream stream = File.OpenRead(zipInfo.FullName);
                        using var reader = ReaderFactory.Open(stream);
                        var zipEntries = new List<ModFileModel>();
                        while (reader.MoveToNextEntry())
                        {
                            zipEntries.Add(new ModFileModel(reader.Entry.Key, reader.Entry.IsDirectory));
                        }

                        //r6/scripts/
                        //r6/scripts/holserByTap.reds
                        return zipEntries;
                    }
                    case ".7z":
                    {
                        using var archive = SevenZipArchive.Open(zipInfo.FullName);
                        var entries = archive.Entries
                            .Select(_ => _.IsDirectory
                                ? new ModFileModel($"{_.Key}/", _.IsDirectory)
                                : new ModFileModel(_.Key, _.IsDirectory))
                            .ToList();
                        return entries;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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
