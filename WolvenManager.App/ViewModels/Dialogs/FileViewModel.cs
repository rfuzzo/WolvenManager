using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace WolvenManager.App.ViewModels.Dialogs
{
    public enum EFileValidState
    {
        Unknown,
        Invalid,
        Valid
    }

    public class FileViewModel : ReactiveObject
    {
        private FileViewModel _parent;
        public ObservableCollection<FileViewModel> Children { get; set; }

        public FileViewModel Parent
        {
            get => _parent;
            set
            {
                this.RaiseAndSetIfChanged(ref _parent, value);
                this.RaisePropertyChanged(nameof(IsValid));
                this.RaisePropertyChanged(nameof(ValidIconPath));
            }
        }

        [Reactive]
        public string FullName { get; private set; }

        [Reactive]
        public bool IsDirectory { get; private set; }

        public string Name => Path.GetFileName(FullName);

        public string ComputedFullName => Parent == null ? Name : Path
            .Combine(Parent.ComputedFullName, Name)
            .Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);


        public EFileValidState IsValid =>
            IsDirectory
                ? EFileValidState.Valid
                : Path.GetExtension(ComputedFullName) == ".reds"
                    ? ComputedFullName.Contains("r6/scripts/") ? EFileValidState.Valid : EFileValidState.Invalid
                    : Path.GetExtension(ComputedFullName) == ".archive"
                        ? ComputedFullName.Contains("archive/pc/mod/") ? EFileValidState.Valid :
                        EFileValidState.Invalid
                        : EFileValidState.Unknown;

        public string ValidIconPath =>
            IsDirectory switch
            {
                true => "None",
                _ => IsValid switch
                {
                    EFileValidState.Unknown => "Question",
                    EFileValidState.Invalid => "Close",
                    EFileValidState.Valid => "Check",
                    _ => throw new ArgumentOutOfRangeException()
                }
            };


        //public SolidColorBrush ValidIconBrush =>
        //    IsDirectory switch
        //    {
        //        true => new SolidColorBrush(Colors.Transparent),
        //        _ => IsValid switch
        //        {
        //            EFileValidState.Unknown => new SolidColorBrush(Colors.CornflowerBlue),
        //            EFileValidState.Invalid => new SolidColorBrush(Colors.Red),
        //            EFileValidState.Valid => new SolidColorBrush(Colors.LawnGreen),
        //            _ => throw new ArgumentOutOfRangeException()
        //        }
        //    };

        public string IconPath =>
            IsDirectory
                ? Children.Any() ? "FolderOpened" : "Folder"
                : "File";



        public FileViewModel(string path, bool isDirectory)
        {
            Children = new ObservableCollection<FileViewModel>();
            IsDirectory = isDirectory;
            FullName = path;


        }
    }
}
