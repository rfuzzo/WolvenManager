using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;

namespace WolvenManager.App.ViewModels
{
    public class ModViewModel : MainViewModel
    {
        [Reactive]
        public string Name { get; set; }

        [Reactive]
        public IEnumerable<string> Files { get; set; }

        [Reactive]
        public bool IsLooseFile { get; set; }


        public ModViewModel()
        {
            
        }

    }
}
