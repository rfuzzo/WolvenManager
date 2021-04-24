using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;
using WolvenManager.App.Models;

namespace WolvenManager.App.ViewModels
{
    public class ModViewModel : MainViewModel
    {
        public ModViewModel(ModModel model)
        {
            Model = model;
        }

        [Reactive]
        [JsonIgnore]
        public ModModel Model { get; set; }

        
        public string Id => Model.Id;
        // readony?
        public string Name => Model.Name;

        public bool IsInLibrary => Model.IsInLibrary;

        public IEnumerable<string> Files => Model.Files;

        public IEnumerable<string> DisabledFiles => Files.Where(_ => Path.GetExtension(_) == ".disabled");

        [Reactive]
        public bool IsEnabled { get; set; }



    }
}
