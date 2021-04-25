using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WolvenManager.App.Utility;

namespace WolvenManager.App.ViewModels.Dialogs
{
    public class ModFilesValidationViewModel : MainViewModel
    {
        private readonly Dictionary<string, FileSystemInfoViewModel> _fileDictionary;

        private readonly ReadOnlyObservableCollection<FileSystemInfoViewModel> _derived;
        public ReadOnlyObservableCollection<FileSystemInfoViewModel> BoundCollection => _derived;


        public ModFilesValidationViewModel(IEnumerable<string> input)
        {
            _fileDictionary = new Dictionary<string, FileSystemInfoViewModel>();
            foreach (var path in input)
            {
                AddFile(path);
            }

            var topentries = _fileDictionary.Values.Where(_ => _.Parent == null);
            var x = new ObservableCollectionExtended<FileSystemInfoViewModel>();
            x.ToObservableChangeSet().Bind(out _derived).Subscribe();
            x.AddRange(topentries);

            OkCommand = ReactiveCommand.Create(ExecuteOK/*, CanExecuteOK*/);
            CancelCommand = ReactiveCommand.Create(() =>
            {
                DialogResult = false;
            });

        }


        private void AddFile(string path)
        {
            // first check if an entry with that name is already in the collection
            if (_fileDictionary.ContainsKey(path))
            {
                return;
            }

            var queue = new Queue<FileSystemInfoViewModel>();

            // create substrings
            var splits = new List<string>();
            var word = "";
            foreach (var c in path)
            {
                if (c == Path.DirectorySeparatorChar || c == Path.AltDirectorySeparatorChar)
                {
                    splits.Add(word);
                }
                word += c;
            }
            splits.Add(word);

            // we can cheat here and always assume the last item is a file
            // while the rest is a directory
            // and we know it's not in the collection already
            if (splits.Count <= 1)
            {
                // just one file
                var vm = new FileSystemInfoViewModel(splits.First(), false);
                _fileDictionary.Add(vm.Name, vm);
            }
            else
            {
                var parentName = "";
                for (int i = 0; i < splits.Count; i++)
                {
                    var f = splits[i];
                    var vm = new FileSystemInfoViewModel(f, true);
                    if (i == splits.Count - 1)
                    {
                        // last item is a file
                        vm = new FileSystemInfoViewModel(f, false);
                    }

                    if (!string.IsNullOrEmpty(parentName)) // has a parent - is already added
                    {
                        vm.Parent = _fileDictionary[parentName];
                        if (_fileDictionary[parentName].Children.All(_ => _.Name != vm.Name))
                        {
                            _fileDictionary[parentName].Children.Add(vm);
                        }
                    }
                    else // has no parent
                    {
                        
                    }

                    if (!_fileDictionary.ContainsKey(f))
                    {
                        _fileDictionary.Add(f, vm);
                    }
                    else
                    {

                    }

                    parentName = f;
                }
            }
        }

        private void ExecuteOK()
        {
            DialogResult = true;
        }

        public bool? DialogResult { get; set; }

        

        public ReactiveCommand<Unit, Unit> OkCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    }


    public class FileSystemInfoViewModel : ReactiveObject
    {
        public ObservableCollection<FileSystemInfoViewModel> Children { get; private set; }

        public FileSystemInfoViewModel Parent { get; set; }

        public string Name { get; }

        public string FullName => Parent == null 
            ? Name 
            : Path.Combine(Parent.FullName, Name);


        private readonly bool _isDirectory;

        public string IconPath => _isDirectory 
            ? Children.Any() ? "FolderOpened" : "Folder" 
            : "File";


        public FileSystemInfoViewModel(string path, bool isDirectory)
        {
            Children = new ObservableCollection<FileSystemInfoViewModel>();
            _isDirectory = isDirectory;
            Name = path;
        }
    }


}
