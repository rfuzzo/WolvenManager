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
    public class PackageResolverViewModel : MainViewModel, IDialogViewModel
    {
        public ObservableCollection<FileViewModel> BoundCollection { get; set; }

        [Reactive]
        public bool? DialogResult { get; set; }

        public PackageResolverViewModel(IEnumerable<ModFileModel> input)
        {
            BoundCollection = new ObservableCollection<FileViewModel>();

            var dict = Init(input);
            var topentries = dict.Values.Where(_ => _.Parent == null);
            BoundCollection.AddRange(topentries);

            OkCommand = ReactiveCommand.Create(ExecuteOk, CanExecuteOk);
            CancelCommand = ReactiveCommand.Create(() =>
            {
                DialogResult = false;
            });
        }

        /// <summary>
        /// Initializes the zip file collection
        /// </summary>
        /// <param name="input"></param>
        private static Dictionary<string, FileViewModel> Init(IEnumerable<ModFileModel> input)
        {
            var fileDictionary = new Dictionary<string, FileViewModel>();
            var modFileModels = input.ToList();
            if (modFileModels.Any(_ => Path.GetExtension(_.Name) == ".reds"))
            {
                AddFile(fileDictionary, new ModFileModel("r6/scripts", true), true);
            }
            if (modFileModels.Any(_ => Path.GetExtension(_.Name) == ".archive"))
            {
                AddFile(fileDictionary, new ModFileModel("archive/pc/mod", true), true);
            }
            foreach (var path in modFileModels)
            {
                path.Name = path.Name.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                // auto-correct file
                if (Path.GetExtension(path.Name) == ".reds" && !path.Name.Contains("r6/scripts/"))
                {
                    var filename = Path.GetFileName(path.Name);
                    if (filename != null)
                    {
                        path.Name = Path.Combine("r6", "scripts", filename)
                            .Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    }
                }
                if (Path.GetExtension(path.Name) == ".archive" && !path.Name.Contains("archive/pc/mod/"))
                {
                    var filename = Path.GetFileName(path.Name);
                    if (filename != null)
                    {
                        path.Name = Path.Combine("archive", "pc", "mod", filename)
                            .Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    }
                }

                AddFile(fileDictionary, path);
            }

            return fileDictionary;
        }

        private IObservable<bool> CanExecuteOk =>
            this.WhenAnyValue(
                x => x.BoundCollection,
                (item) =>
                    item.All(_ => _.IsValid is EFileValidState.Valid or EFileValidState.Unknown)
            );


        private static void AddFile(IDictionary<string, FileViewModel> fileDictionary, ModFileModel path, bool overrideFolder = false)
        {
            // first check if an entry with that name is already in the collection
            if (fileDictionary.ContainsKey(path.Name))
            {
                return;
            }
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
                var vm = new FileViewModel(splits.First(), overrideFolder);
                fileDictionary.Add(vm.FullName, vm);
            }
            else
            {
                var parentName = "";
                for (int i = 0; i < splits.Count; i++)
                {
                    var f = splits[i];
                    var vm = new FileViewModel(f, true);
                    if (i == splits.Count - 1)
                    {
                        // last item is a file
                        vm = new FileViewModel(f, overrideFolder);
                    }

                    if (!string.IsNullOrEmpty(parentName)) // has a parent - is already added
                    {
                        vm.Parent = fileDictionary[parentName];
                        if (fileDictionary[parentName].Children.All(_ => _.FullName != vm.FullName))
                        {
                            fileDictionary[parentName].Children.Add(vm);
                        }
                    }

                    if (!fileDictionary.ContainsKey(f))
                    {
                        fileDictionary.Add(f, vm);
                    }

                    parentName = f;
                }
            }
        }

        private void ExecuteOk() => DialogResult = true;


        #region commands

        public ReactiveCommand<Unit, Unit> OkCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        #endregion

        public IEnumerable<FileViewModel> GetOutput()
        {
            var leaves = new List<FileViewModel>();
            foreach (var fileViewModel in BoundCollection)
            {
                leaves.AddRange(FindAll(fileViewModel));
            }
            var fileLeaves = leaves.Where(_ => !_.IsDirectory);

            return fileLeaves;//.Select(_ => new ModFileModel(_.ComputedFullName, false));
        }

        private static IEnumerable<FileViewModel> FindAll(FileViewModel root)
        {
            if (root.Children.Count == 0)
            {
                yield return root;
            }

            foreach (var child in root.Children)
            {
                var found = FindAll(child);

                foreach (var item in found)
                {
                    yield return item;
                }
            }
        }

    }
}
