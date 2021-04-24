using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

        // bound mod change set
        //private readonly ReadOnlyObservableCollection<ModItemViewModel> _items;

        // the actual library
        private SourceCache<ModModel, string> _mods = new(t => t.Id);


        private static string LibraryPath
        {
            get
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;
                var filename = Path.GetFileNameWithoutExtension(path);
                var dir = Path.GetDirectoryName(path);
                return Path.Combine(dir ?? "", filename + "lib.bin");
            }
        }
        #endregion

        public LibraryService()
        {
            _notificationService = Locator.Current.GetService<INotificationService>();
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


        /// <summary>
        /// Serialize the library to a file
        /// </summary>
        private void Serialize()
        {
            using var file = File.Create(LibraryPath);
            Serializer.Serialize(file, _mods.Items);
        }

        /// <summary>
        /// Load the library file
        /// </summary>
        private void Deserialize()
        {
            if (File.Exists(LibraryPath))
            {
                using var file = File.OpenRead(LibraryPath);
                var moditems = Serializer.Deserialize<IEnumerable<ModModel>>(file);

                _mods = new SourceCache<ModModel, string>(_ => _.Id);
                _mods.Edit(innerCache =>
                {
                    innerCache.Clear();
                    innerCache.AddOrUpdate(moditems);
                });
            }
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

                    // check if in Modlist
                    if (GetModForFile(fileInfo.FullName) != null)
                    {
                        // do nothing?
                        // could something fancy and hash the file and compare to chached mod 
                        // meh
                    }
                    else
                    {
                        // If not found, add a loose file mod
                        var mod = new ModModel()
                        {
                            Name = fileInfo.Name,
                            Files = new[] { fileInfo.FullName }
                        };

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
