using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using WolvenKit.Common.Services;
using WolvenKit.RED4.Archive;
using WolvenKit.RED4.CR2W.Archive;
using WolvenManager.App.Services;

namespace WolvenManager.App.ViewModels
{
    public class ArchiveViewModel : ReactiveObject
    {
        private readonly Archive _archive;

        public ArchiveViewModel(Archive archive)
        {
            _archive = archive;


            foreach (var (_, value) in _archive.Files)
            {
                SubFiles.Add(new ArchiveFileViewModel(value));
            }
        }

        public ObservableCollection<ArchiveFileViewModel> SubFiles { get; } = new();

        public int LoadOrder { get; set; }

        public string Name => _archive.Name;

    }
}
