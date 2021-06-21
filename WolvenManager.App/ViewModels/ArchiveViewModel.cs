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
using WolvenKit.RED4.CR2W.Archive;
using WolvenManager.App.Services;

namespace WolvenManager.App.ViewModels
{
    public class ArchiveViewModel : ReactiveObject
    {
        private readonly IHashService _hashService;


        public ArchiveViewModel(string path)
        {
            _hashService = Locator.Current.GetService<IHashService>();

            Archive = Red4ParserServiceExtensions.ReadArchive(path, _hashService);

            foreach (var (_, value) in Archive.Files)
            {
                SubFiles.Add(new ArchiveFileViewModel(value));
            }

        }

        public Archive Archive { get; }

        public ObservableCollection<ArchiveFileViewModel> SubFiles { get; } = new();



    }
}
