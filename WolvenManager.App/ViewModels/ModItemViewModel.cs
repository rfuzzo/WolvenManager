using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;

namespace WolvenManager.App.ViewModels
{
    public class ModItemViewModel : MainViewModel
    {
        [Reactive]
        public string Path { get; set; }

        public ModItemViewModel()
        {
            
        }

    }
}
