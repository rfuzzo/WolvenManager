using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using ProtoBuf;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WolvenKit.Common;
using WolvenKit.Common.Services;
using WolvenKit.RED4.CR2W.Archive;

namespace WolvenManager.App.Services
{
    public class ArchiveService : ReactiveObject, IArchiveService
    {
        private readonly IHashService _hashService;
        private readonly ISettingsService _settingsService;
        private readonly INotificationService _notificationService;

        
        



        public ArchiveService(
            IHashService hashService,
            INotificationService notificationService,
            ISettingsService settingsService
            )
        {
            _hashService = hashService;
            _settingsService = settingsService;
            _notificationService = notificationService;

            _fileCache = new SourceCache<FileEntry, ulong>(t => t.Key);
            _rootCache = new SourceCache<GameFileTreeNode, string>(t => t.FullPath);
        }

        public ArchiveManager ArchiveManager { get; private set; }
        [Reactive] public bool IsLoaded { get; set; }

        private readonly SourceCache<FileEntry, ulong> _fileCache;
        public IObservable<IChangeSet<FileEntry, ulong>> Connect() => _fileCache.Connect();

        private readonly SourceCache<GameFileTreeNode, string> _rootCache;
        public IObservable<IChangeSet<GameFileTreeNode, string>> ConnectHierarchy() => _rootCache.Connect();


        public void Load()
        {
            var archiveDir = Path.Combine(_settingsService.GamePath, "archive", "pc", "content");
            var chachePath = Path.Combine(_settingsService.AppData, "archives.bin");

            try
            {

                if (File.Exists(chachePath))
                {
                    using var file = File.OpenRead(chachePath);
                    ArchiveManager = Serializer.Deserialize<ArchiveManager>(file);

                    if (ArchiveManager.Archives.Count < 1)
                    {
                        throw new SerializationException();
                    }
                }
                else
                {
                    ArchiveManager = new ArchiveManager(_hashService);
                    ArchiveManager.LoadFromFolder(Path.GetDirectoryName(archiveDir));

                    using var file = File.Create(chachePath);
                    Serializer.Serialize(file, ArchiveManager);
                }
            }
            catch (Exception e)
            {
                ArchiveManager = new ArchiveManager(_hashService);
               
                ArchiveManager.LoadFromFolder(Path.GetDirectoryName(archiveDir));

                using var file = File.Create(chachePath);
                Serializer.Serialize(file, ArchiveManager);
            }

            // connect
            IsLoaded = true;
            _fileCache.Edit(innerCache =>
            {
                innerCache.Clear();
                var vals = ArchiveManager.Items.Values.SelectMany( _ => _);
                innerCache.AddOrUpdate(vals.Cast<FileEntry>());
            });

            _rootCache.Edit(innerCache =>
            {
                innerCache.Clear();
                innerCache.AddOrUpdate(ArchiveManager.RootNode);
            });

            _notificationService.Success("Finished Loading archives");

        }
    }
}
