using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WolvenManager.App.Arguments;
using WolvenManager.App.Models;
using WolvenManager.App.Utility;

namespace WolvenManager.App.ViewModels.Dialogs
{
    public class ModFilesValidationViewModel : MainViewModel, IDialogViewModel
    {
        private readonly Dictionary<string, FileSystemInfoViewModel> _fileDictionary;

        private readonly ReadOnlyObservableCollection<FileSystemInfoViewModel> _derived;
        public ReadOnlyObservableCollection<FileSystemInfoViewModel> BoundCollection => _derived;

        [Reactive]
        public bool? DialogResult { get; set; }

        public ModFilesValidationViewModel(IEnumerable<ModFileModel> input)
        {
            _fileDictionary = new Dictionary<string, FileSystemInfoViewModel>();
            
            AddFile(new ModFileModel("r6/scripts", true), true);
            AddFile(new ModFileModel("archive/pc/mod", true), true);
            foreach (var path in input)
            {
                AddFile(path);
            }

            var x = new ObservableCollectionExtended<FileSystemInfoViewModel>();
            x.ToObservableChangeSet().Bind(out _derived).Subscribe();
            var topentries = _fileDictionary.Values.Where(_ => _.Parent == null);
            x.AddRange(topentries);

            OkCommand = ReactiveCommand.Create(ExecuteOK, CanExecuteOk);
            CancelCommand = ReactiveCommand.Create(() =>
            {
                DialogResult = false;
            });
        }

        private IObservable<bool> CanExecuteOk
        {
            get
            {
                return this.WhenAnyValue(
                    x => x.BoundCollection,
                    (item) =>
                        item.All(_ => _.IsValid)
                );

            }
        }


        private void AddFile(ModFileModel path, bool overrideFolder = false)
        {
            // first check if an entry with that name is already in the collection
            if (_fileDictionary.ContainsKey(path.Name))
            {
                return;
            }

            var queue = new Queue<FileSystemInfoViewModel>();

            // create substrings
            var splits = new List<string>();
            var word = "";
            foreach (var c in path.Name)
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
                var vm = new FileSystemInfoViewModel(splits.First(), overrideFolder);
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
                        vm = new FileSystemInfoViewModel(f, overrideFolder);
                    }

                    if (!string.IsNullOrEmpty(parentName)) // has a parent - is already added
                    {
                        vm.Parent = _fileDictionary[parentName];
                        if (_fileDictionary[parentName].Children.All(_ => _.Name != vm.Name))
                        {
                            _fileDictionary[parentName].Children.Add(vm);
                        }
                    }

                    if (!_fileDictionary.ContainsKey(f))
                    {
                        _fileDictionary.Add(f, vm);
                    }

                    parentName = f;
                }
            }
        }

        private void ExecuteOK()
        {
            DialogResult = true;
        }



        public ReactiveCommand<Unit, Unit> OkCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public IEnumerable<ModFileModel> GetOutput()
        {



            return null;
        }
    }
}
