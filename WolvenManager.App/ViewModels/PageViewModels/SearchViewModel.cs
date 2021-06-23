using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WolvenManager.App.Attributes;
using CP77Tools.Tasks;
using DotNetHelper.FastMember.Extension.Helpers;
using DynamicData;
using DynamicData.Binding;
using Splat;
using Syncfusion.Windows.PropertyGrid;
using WolvenKit.Common;
using WolvenKit.Common.Services;
using WolvenKit.RED4.CR2W.Archive;
using WolvenManager.App.Services;
using WolvenManager.Models;

namespace WolvenManager.App.ViewModels.PageViewModels
{
    [RoutingUrl(Constants.RoutingIDs.Search)]
    public class SearchViewModel : PageViewModel
    {
        private readonly IConsoleFunctions _consoleFunctions;
        private readonly ILoggerService _loggerService;
        private readonly IArchiveService _archiveService;

        private readonly ReadOnlyObservableCollection<FileEntryViewModel> _bindingData;
        public ReadOnlyObservableCollection<FileEntryViewModel> BindingData => _bindingData;

        private readonly ReadOnlyObservableCollection<GameFileTreeNode> _bindingHData;
        public ReadOnlyObservableCollection<GameFileTreeNode> BindingHData => _bindingHData;

        [Reactive] public GameFileTreeNode SelectedItem { get; set; }
        [Reactive] public IEnumerable<FileEntryViewModel> SelectedFiles { get; set; }

        public SearchViewModel(
            IConsoleFunctions consoleFunctions, 
            ILoggerService loggerService,
            IArchiveService archiveService
            ) : base(typeof(ModkitViewModel))
        {
            _consoleFunctions = consoleFunctions;
            _loggerService = loggerService;
            _archiveService = archiveService;



            //var disposable = _archiveService.Connect()
            //    .LimitSizeTo(10000)
            //    .Transform(_ => new FileEntryViewModel(_))
            //    .ObserveOn(RxApp.MainThreadScheduler)
            //    .Bind(out _bindingData)
            //    .Subscribe();

            var disposable = _archiveService.ConnectHierarchy()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _bindingHData)
                .Subscribe();
        }

    }

    public class FileEntryViewModel : ReactiveObject
    {
        private readonly FileEntry _fileEntry;

        public FileEntryViewModel(FileEntry fileEntry)
        {
            _fileEntry = fileEntry;
        }

        public string Name => _fileEntry.ShortName;
        public string Extension => _fileEntry.Extension;
        public string FullName => _fileEntry.Name;
        public string Archive => _fileEntry.Archive.Name;
        public ulong Key => _fileEntry.Key;

    }
}
