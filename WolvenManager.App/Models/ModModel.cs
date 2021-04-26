using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProtoBuf;
using ReactiveUI.Fody.Helpers;
using WolvenManager.App.Utility;
using WolvenManager.App.ViewModels;

namespace WolvenManager.App.Models
{
    [ProtoContract]
    public class ModModel : MainViewModel
    {
        public ModModel()
        {
            
        }


        //[ProtoMember(1)]
        public string Id => $"{Name}_{ContentHash}";

        /// <summary>
        /// Calculates a hash over the mod files
        /// TODO: how fast is this
        /// </summary>
        private int ContentHash => HashHelpers.GetHashCodeOfList(Files);

        [Reactive]
        [ProtoMember(1)]
        public string Name { get; set; }

        [Reactive]
        [ProtoMember(2)]
        public IEnumerable<string> Files { get; set; }


        [Reactive]
        [ProtoMember(3)]
        public bool IsInPhysicalLibrary { get; set; }

        [Reactive]
        [ProtoMember(4)]
        public bool Installed { get; set; }

        [ProtoMember(5)]
        public byte[] ZipHash { get; set; }

    }
}
