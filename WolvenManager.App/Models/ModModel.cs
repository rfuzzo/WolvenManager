﻿using System.Collections.Generic;
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
        public string Id => $"{Name}_{Hash}";

        /// <summary>
        /// Calculates a hash over the mod files
        /// TODO: how fast is this
        /// </summary>
        private int Hash => HashHelpers.GetHashCodeOfList(Files);

        [Reactive]
        [ProtoMember(1)]
        public string Name { get; set; }

        [Reactive]
        [ProtoMember(2)]
        public IEnumerable<string> Files { get; set; }


        [Reactive]
        [ProtoMember(3)]
        public bool IsInLibrary { get; set; }
    }
}
