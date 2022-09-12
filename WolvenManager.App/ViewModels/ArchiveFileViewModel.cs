using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using WolvenKit.Common;
using WolvenKit.Core.Interfaces;

namespace WolvenManager.App.ViewModels
{
    public class ArchiveFileViewModel : ReactiveObject
    {
        public ArchiveFileViewModel(IGameFile model)
        {
            Model = model;
        }

        public ArchiveViewModel Parent { get; set; }

        public IGameFile Model { get; }

        public string Name => Path.GetFileName(Model.Name);

        public ulong Id => Model.Key;


    }
}
