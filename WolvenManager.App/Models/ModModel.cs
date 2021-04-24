using System.Collections.Generic;
using ProtoBuf;
using ReactiveUI.Fody.Helpers;
using WolvenManager.App.ViewModels;

namespace WolvenManager.App.Models
{
    [ProtoContract]
    public class ModModel : MainViewModel
    {
        //[ProtoMember(1)]
        public string Id
        {
            get
            {
                return $"{Name}";
            }
        }

        [Reactive]
        [ProtoMember(2)]
        public string Name { get; set; }

        [Reactive]
        [ProtoMember(3)]
        public string LibraryLocation { get; set; }

        [Reactive]
        [ProtoMember(4)]
        public IEnumerable<string> Files { get; set; }

    }
}
