using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WolvenManager.App.Attributes;
using CP77Tools.Tasks;
using DynamicData;
using DynamicData.Binding;
using Splat;
using Syncfusion.Windows.PropertyGrid;
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



        public ModkitViewModel(
            IConsoleFunctions consoleFunctions,
            ILoggerService loggerService
        ) : base(typeof(ModkitViewModel))
        {
            _consoleFunctions = consoleFunctions;
            _loggerService = loggerService;

            Items = new ObservableCollection<CommandModel>
            {
                new UnbundleCommandModel(_settingsService, _notificationService),
                new UncookCommandCommandModel(_settingsService, _notificationService),

                new PackCommandCommandModel(_settingsService, _notificationService),

                new ExportCommandCommandModel(_settingsService, _notificationService),
                new ImportCommandCommandModel(_settingsService, _notificationService),

                new ArchiveCommandModel(_settingsService, _notificationService),
                new CR2WCommandCommandModel(_settingsService, _notificationService),

                new OodleCommandCommandModel(_settingsService, _notificationService),
            };

            var canExecute = this.WhenAnyValue(
                x => x.SelectedItem,
                (userName) =>
                    (CommandModel)userName != null);

            RunCommand = ReactiveCommand.CreateFromTask(RunAsync, canExecute);

            SelectedItem = Items.First();

        }

        private async Task RunAsync()
        {
            await SelectedItem.ExecuteAsync(_consoleFunctions);
        }


        #region properties

        public ReactiveCommand<Unit, Unit> RunCommand { get; }

        [Reactive]
        public ObservableCollection<CommandModel> Items { get; set; }

        //[Reactive]
        //public CommandModel SelectedItem { get; set; }
        private CommandModel _selectedItem;
        public CommandModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                this.RaiseAndSetIfChanged(ref this._selectedItem, value);
                this.RaisePropertyChanged(nameof(TextBlockText));
            }
        }

        public string TextBlockText => SelectedItem != null ? $"Run {SelectedItem.Name}" : "Run";

        #endregion




    }
}
