using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using Splat;
using WolvenManager.App.Models;
using WolvenManager.App.ViewModels;
using ProtoBuf;

namespace WolvenManager.App.Services
{
    /// <summary>
    /// A service to manage the current mod library
    /// TODO: this can get really big, and potentially hog memory
    /// TODO: think about moving this to a database or a memorymapped physical file
    /// </summary>
    public class LibraryService: ReactiveObject, ILibraryService
    {
        #region fields
        private readonly INotificationService _notificationService;
        private readonly ISettingsService _settings;

        // the actual library
        private SourceCache<ModModel, string> _mods = new(t => t.Id);
       
        #endregion

        public LibraryService()
        {
            _notificationService = Locator.Current.GetService<INotificationService>();
            _settings = Locator.Current.GetService<ISettingsService>();
            var modwatcherService = Locator.Current.GetService<IWatcherService>();

            Deserialize();

            modwatcherService.ConnectMods()
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

        /// <summary>
        /// Connection to the dynamic mod library
        /// </summary>
        /// <returns></returns>
        public IObservable<IChangeSet<ModModel, string>> Connect() => _mods.Connect();

        public void AddModToLibrary(ModModel model)
        {
            // check if already exists
            if (_mods.Keys.Contains(model.Id))
            {

            }
            else
            {
                _mods.AddOrUpdate(model);
            }
        }


        /// <summary>
        /// Serialize the library to a file
        /// </summary>
        private void Serialize()
        {
            using var file = File.Create(Constants.LibraryPath);
            Serializer.Serialize(file, _mods.Items);
        }

        /// <summary>
        /// Load the library file
        /// </summary>
        private void Deserialize()
        {
            if (!File.Exists(Constants.LibraryPath))
            {
                return;
            }

            using var file = File.OpenRead(Constants.LibraryPath);
            var moditems = Serializer.Deserialize<IEnumerable<ModModel>>(file);

            _mods = new SourceCache<ModModel, string>(_ => _.Id);
            _mods.Edit(innerCache =>
            {
                innerCache.Clear();
                innerCache.AddOrUpdate(moditems);
            });
        }


        /// <summary>
        /// Evaluates a change in the game mod directories
        /// updates the library
        /// </summary>
        /// <param name="change"></param>
        private void EvaluateChange(Change<ModItemViewModel, string> change)
        {
            switch (change.Reason)
            {
                case ChangeReason.Add:
                    var fileInfo = new FileInfo(change.Current.FullPath);

                    // check if in library
                    if (GetModForFile(fileInfo.FullName) != null)
                    {
                        // do nothing?
                        // TODO: could do something fancy and hash the file and compare to chached mod
                    }
                    else
                    {
                        // If not found, add a loose file mod
                        var mod = new ModModel();

                        var modfile = fileInfo.FullName;
                        var modName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                        if (fileInfo.Extension == ".disabled")
                        {
                            modName = Path.GetFileNameWithoutExtension(modName);
                            modfile = Path.ChangeExtension(modfile, "").TrimEnd('.');
                        }

                        mod.Files = new[] {GetRelativeGameFilePath(modfile) };
                        mod.Name = modName;
                        
                        _mods.AddOrUpdate(mod);
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

            Serialize();
        }

        /// <summary>
        /// Calculates the substring of the input string from the GamePath length.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string GetRelativeGameFilePath(string input) => input[(_settings.GamePath.Length + 1)..];

        /// <summary>
        /// Try get a mod from the library by a given file
        /// TODO: this is possibly slow
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <returns></returns>
        private ModModel GetModForFile(string path)
        {
            var results = _mods.Items.Where(_ => _.Files.Contains(path)).ToList();
            var count = results.Count;

            return count switch
            {
                0 => null,
                > 1 => throw new AmbiguousMatchException(),
                _ => results.First(),
            };
        }

    }
}


// and save the mod to Lib
//if (_settings.IsLibraryEnabled)
//{
//    // check if folder exists in library


//    // create folder with modname
//    var moddir = Directory.CreateDirectory(Path.Combine(Constants.LibraryPath, mod.Name));
//    var zipfile = Path.Combine(moddir.FullName, $"{mod.Name}.zip");
//    if (File.Exists(zipfile))
//    {
//        File.Delete(zipfile);
//    }

//    // add file to zip
//    using var archive = ZipFile.Open(zipfile, ZipArchiveMode.Create);
//    archive.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name, CompressionLevel.Optimal);

//    mod.IsInLibrary = true;
//}