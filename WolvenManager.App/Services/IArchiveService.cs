using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using WolvenKit.Common;
using WolvenKit.RED4.CR2W.Archive;

namespace WolvenManager.App.Services
{
    public interface IArchiveService
    {
        public IObservable<IChangeSet<FileEntry, ulong>> Connect();
        public IObservable<IChangeSet<GameFileTreeNode, string>> ConnectHierarchy();
        public IObservable<IChangeSet<Archive, string>> ConnectModArchives();

        void Load();
        public IObservable<bool> IsLoaded { get; }
    }
}
