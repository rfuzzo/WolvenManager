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

            };

            RunCommand = ReactiveCommand.CreateFromTask(RunAsync, CanRunAsync);


           

        }

        public IObservable<bool> CanRunAsync { get; set; }

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
