using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using WolvenManager.App.ViewModels;

namespace WolvenManager.App.Models
{
    public class ModFileModel : MainViewModel
    {
        public ModFileModel(string name, bool isDirectory)
        {
            Name = name;
            IsDirectory = isDirectory;
        }

        public string Name { get; set; }
        public bool IsDirectory { get; set; }
    }
}
