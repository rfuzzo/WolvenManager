using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WolvenManager.App.Attributes;
using CP77Tools.Tasks;
using DynamicData;
using DynamicData.Binding;
using WolvenKit.Common.Services;
using WolvenManager.App.Services;
using WolvenManager.Models;

namespace WolvenManager.App.ViewModels.PageViewModels
{
    [RoutingUrl(Constants.RoutingIDs.Modkit)]
    public class ModkitViewModel : PageViewModel
    {
        private readonly IConsoleFunctions _consoleFunctions;
        private readonly ILoggerService _loggerService;

        private readonly ReadOnlyObservableCollection<LogEntry> _logEntries;
        public ReadOnlyObservableCollection<LogEntry> LogEntries => _logEntries;

        public ModkitViewModel(IConsoleFunctions consoleFunctions, ILoggerService loggerService) : base(typeof(ModkitViewModel))
        {
            _consoleFunctions = consoleFunctions;
            _loggerService = loggerService;

            Items = new ObservableCollection<CommandModel>
            {
                new ArchiveCommandModel (),
                new UnbundleCommandModel(),

            };

            RunCommand = ReactiveCommand.CreateFromTask(RunAsync);


            //filter, sort and populate reactive list,
            _loggerService.Connect() //connect to the cache
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _logEntries)
                .Subscribe();

        }

        private async Task RunAsync()
        {
            await SelectedItem.ExecuteAsync(_consoleFunctions);
        }


        #region properties


        [Reactive]
        public ObservableCollection<CommandModel> Items { get; set; }

        [Reactive]
        public CommandModel SelectedItem { get; set; }

        #endregion


        public ReactiveCommand<Unit, Unit> RunCommand { get; }

    }
}
