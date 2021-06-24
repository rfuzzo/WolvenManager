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

        public ModkitViewModel(IConsoleFunctions consoleFunctions, ILoggerService loggerService) : base(typeof(ModkitViewModel))
        {
            _consoleFunctions = consoleFunctions;
            _loggerService = loggerService;

            Items = new ObservableCollection<CommandModel>
            {
                new ArchiveCommandModel (),
                new UnbundleCommandModel(),
                new CR2WCommandCommandModel(),
                new ExportCommandCommandModel(),
                new ImportCommandCommandModel(),
                new OodleCommandCommandModel(),
                new PackCommandCommandModel(),
                new UncookCommandCommandModel(),
            };

            var canExecute = this.WhenAnyValue(
                x => x.SelectedItem,
                (userName) =>
                    (CommandModel)userName != null);

            RunCommand = ReactiveCommand.CreateFromTask(RunAsync, canExecute);

        }

        private async Task RunAsync()
        {
            await SelectedItem.ExecuteAsync(_consoleFunctions);
        }


        #region properties

        public ReactiveCommand<Unit, Unit> RunCommand { get; }

        [Reactive]
        public ObservableCollection<CommandModel> Items { get; set; }

        [Reactive]
        public CommandModel SelectedItem { get; set; }

        #endregion


       

    }
}
