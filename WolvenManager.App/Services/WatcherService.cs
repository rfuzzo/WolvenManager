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
        #region fields

        private readonly SourceCache<ModItemViewModel, string> _mods;
        public IObservable<IChangeSet<ModItemViewModel, string>> ConnectMods() => _mods.Connect();

        private readonly ISettingsService _settings;


        private readonly FileSystemWatcher _scriptsWatcher;
        private readonly FileSystemWatcher _modsWatcher;

        #endregion






        public WatcherService()
        {
            _settings = Locator.Current.GetService<ISettingsService>();

            _mods = new SourceCache<ModItemViewModel, string>(_ => _.FullPath);

            // scripts 
            _scriptsWatcher = new FileSystemWatcher(_settings.ScriptsDir, "*")
            {
                IncludeSubdirectories = false
            };
            _scriptsWatcher.Created += OnChanged;
            _scriptsWatcher.Changed += OnChanged;
            _scriptsWatcher.Deleted += OnChanged;
            _scriptsWatcher.Renamed += OnRenamed;
            _scriptsWatcher.EnableRaisingEvents = true;


            // mods 
            _modsWatcher = new FileSystemWatcher(_settings.ModsDir, "*")
            {
                IncludeSubdirectories = false
            };
            _modsWatcher.Created += OnChanged;
            _modsWatcher.Changed += OnChanged;
            _modsWatcher.Deleted += OnChanged;
            _modsWatcher.Renamed += OnRenamed;
            _modsWatcher.EnableRaisingEvents = true;

        }

        public bool IsSuspended { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public async void RefreshAsync()
        {
            await Task.Run(DetectScripts);
            await Task.Run(DetectMods);
        }

        /// <summary>
        /// get all script mod loose files 
        /// </summary>
        /// <returns></returns>
        private void DetectScripts()
        {
            var scriptsMods = Directory
                    .GetFiles(_settings.ScriptsDir, "*", SearchOption.AllDirectories)
                    .Select(_ => new FileInfo(_))
                    .Where(_ => _.Extension == ".reds" ||
                                (_.Extension == ".disabled" && new FileInfo(_.FullName[..^9]).Extension == ".reds"));

            _mods.Edit(innerList =>
            {
                innerList.AddOrUpdate(scriptsMods.Select(_ => new ModItemViewModel()
                {
                    FullPath = _.FullName,
                }));
            });

        }

        /// <summary>
        /// get all mods loose files
        /// </summary>
        /// <returns></returns>
        private void DetectMods()
        {
            var scriptsMods = Directory
                .GetFiles(_settings.ModsDir, "*", SearchOption.AllDirectories)
                .Select(_ => new FileInfo(_))
                .Where(_ => _.Extension == ".archive" ||
                            (_.Extension == ".disabled" && new FileInfo(_.FullName[..^9]).Extension == ".archive"));

            _mods.Edit(innerList =>
            {
                innerList.AddOrUpdate(scriptsMods.Select(_ => new ModItemViewModel()
                {
                    FullPath = _.FullName,
                }));
            });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (IsSuspended)
            {
                //Log.Debug("Watching is suspended, ignoring file system watcher change");
                return;
            }


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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            
        }



    }
}
