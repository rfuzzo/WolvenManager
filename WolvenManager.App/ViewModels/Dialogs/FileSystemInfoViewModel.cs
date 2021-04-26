using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace WolvenManager.App.ViewModels.Dialogs
{
    public class FileSystemInfoViewModel : ReactiveObject
    {
        public ObservableCollection<FileSystemInfoViewModel> Children { get; }

        public FileSystemInfoViewModel Parent { get; set; }

        public string Name { get; }

        public string FullName => Parent == null
            ? Name
            : Path.Combine(Parent.FullName, Name);

        public bool IsValid => Name is "r6" or 
            "archive" or 
            "archive/pc" or 
            "archive/pc/mod" || 
                               Name.Contains("r6/scripts") || 
                               Name.Contains("archive/pc/mod");

        private readonly bool _isDirectory;

        public string IconPath => _isDirectory
            ? Children.Any() ? "FolderOpened" : "Folder"
            : "File";

        public string ValidIconPath =>
            _isDirectory switch
            {
                true => "",
                _ => IsValid ? "Check" : "Close"
            };

        public FileSystemInfoViewModel(string path, bool isDirectory)
        {
            Children = new ObservableCollection<FileSystemInfoViewModel>();
            _isDirectory = isDirectory;
            Name = path;
        }
    }
}
