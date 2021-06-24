using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WolvenManager.App.Attributes;
using WolvenManager.App.Services;
using System.Reactive.Linq;
using DynamicData;

namespace WolvenManager.App.ViewModels.PageViewModels
{
    [RoutingUrl(Constants.RoutingIDs.Mods)]
    public class ModListViewModel : PageViewModel
    {
        private readonly IArchiveService _archiveService;


        private readonly ReadOnlyObservableCollection<ArchiveViewModel> _modArchiveCollection;
        public ReadOnlyObservableCollection<ArchiveViewModel> ModArchiveCollection => _modArchiveCollection;

        public ModListViewModel(
            IArchiveService archiveService
        ) : base(typeof(ModListViewModel))
        {
            _archiveService = archiveService;

            RefreshModCommand = ReactiveCommand.CreateFromTask(Refresh);
            //ToggleSidebarCommand = ReactiveCommand.Create(() =>
            //{
            //    IsSideBarVisible = !IsSideBarVisible;
            //});

            var disposable = _archiveService.ConnectModArchives()
                .Transform(_ => new ArchiveViewModel(_))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _modArchiveCollection)
                .Subscribe();
        }

        #region properties

        [Reactive]
        public ArchiveViewModel SelectedModViewModel { get; set; }

        [Reactive] public ObservableCollection<object> SelectedModViewModels { get; set; } = new();



        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> RefreshModCommand { get; }

        private Task Refresh()
        {

            return Task.CompletedTask;
        }

        #endregion

    }
}
