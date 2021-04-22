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
using WolvenManager.App.ViewModels;

namespace WolvenManager.App.Services
{
    /// <summary>
    /// Serializable 
    /// </summary>
    public class ProfileService : ReactiveObject, IProfileService, ISerializable
    {
        private readonly ReadOnlyObservableCollection<ModItemViewModel> _items;

        private readonly SourceCache<ModViewModel, string> _mods = new(t => t.Name);
        public IObservable<IChangeSet<ModViewModel, string>> Connect() => _mods.Connect();


        public ProfileService()
        {
            var watcherService = Locator.Current.GetService<IWatcherService>();
            watcherService.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _items)
                .Subscribe(OnNext);

        }

        private void OnNext(IChangeSet<ModItemViewModel> _)
        {
            foreach (var change in _)
            {
                EvaluateChange(change);
            }
        }

        private ModViewModel GetModForFile(string path)
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

        private void EvaluateChange(Change<ModItemViewModel> change)
        {
            switch (change.Reason)
            {
                case ListChangeReason.Add:
                    break;
                case ListChangeReason.AddRange:
                    // new files are added to the watched directory
                    foreach (var modItemViewModel in change.Range)
                    {
                        var fileInfo = new FileInfo(modItemViewModel.Path);

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
                            var mod = new ModViewModel()
                            {
                                Name = fileInfo.Name,
                                IsLooseFile = true,
                                Files = new[] { fileInfo.FullName }
                            };

                            _mods.AddOrUpdate(mod);

                        }
                    }


                    break;
                case ListChangeReason.Replace:
                    break;
                case ListChangeReason.Remove:
                    break;
                case ListChangeReason.RemoveRange:
                    break;
                case ListChangeReason.Refresh:
                    break;
                case ListChangeReason.Moved:
                    break;
                case ListChangeReason.Clear:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
