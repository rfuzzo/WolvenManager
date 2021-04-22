using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using Splat;
using WolvenManager.App.ViewModels;

namespace WolvenManager.App.Services
{
    /// <summary>
    /// This service watches certain locations in the game files and notifies changes
    /// </summary>
    public class WatcherService : IWatcherService
    {
        private readonly SourceList<ModItemViewModel> _mods = new();
        public IObservable<IChangeSet<ModItemViewModel>> Connect() => _mods.Connect();

        private readonly IAppSettingsService _settings;


        private readonly FileSystemWatcher _fswScripts;
        private FileSystemWatcher _fileSystemWatcher2;
        private FileSystemWatcher _fileSystemWatcher3;

              
        

  
        public WatcherService()
        {
            _settings = Locator.Current.GetService<IAppSettingsService>();

            // initial set
            _mods = DetectMods();


            _fswScripts = new FileSystemWatcher(_settings.ScriptsDir, "*")
            {
                
                IncludeSubdirectories = false
            };

            _fswScripts.Created += FswScriptsOnChanged;
            _fswScripts.Changed += FswScriptsOnChanged;
            _fswScripts.Deleted += FswScriptsOnChanged;
            _fswScripts.Renamed += FswScriptsOnRenamed;

            _fswScripts.EnableRaisingEvents = true;



        }

        private SourceList<ModItemViewModel> DetectMods()
        {
            var scriptsMods = Directory
                    .GetFiles(_settings.ScriptsDir, "*")
                    .Select(_ => new FileInfo(_))
                    .Where(_ => _.Extension == ".reds" ||
                                (_.Extension == ".disabled" && new FileInfo(_.FullName[..^9]).Extension == ".reds"));

            var sourceList = new SourceList<ModItemViewModel>();
            sourceList.Edit(innerList =>
            {
                innerList.Clear();
                innerList.AddRange(scriptsMods.Select(_ => new ModItemViewModel()
                {
                    Path = _.FullName,
                }));
            });

            return sourceList;
        }

        private void FswScriptsOnChanged(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    break;
                case WatcherChangeTypes.Deleted:
                    

                    break;
                case WatcherChangeTypes.Changed:
                    break;
                case WatcherChangeTypes.Renamed:
                    break;
                case WatcherChangeTypes.All:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();



            }
        }


        private void FswScriptsOnRenamed(object sender, RenamedEventArgs e)
        {
            
        }



    }
}
