using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DynamicData;
using ReactiveUI;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.TreeView;
using WolvenManager.App.Arguments;
using WolvenManager.App.Models;
using WolvenManager.App.Utility;
using WolvenManager.App.ViewModels;
using WolvenManager.App.ViewModels.Dialogs;
using WolvenManager.App.ViewModels.PageViewModels;
using WolvenManager.UI.Implementations;
using WolvenManager.UI.Views.Dialogs;
using DropPosition = Syncfusion.UI.Xaml.Grid.DropPosition;

namespace WolvenManager.UI.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class ModListView : ReactiveUserControl<ModListViewModel>
    {
        public ModListView()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel)
                .BindTo(this, x => x.DataContext);



            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.BindingData,
                        view => view.ModList.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.IsSideBarVisible,
                        view => view.Properties.Visibility)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                        viewModel => viewModel.InstallModCommand,
                        view => view.InstallModButton)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.ToggleSidebarCommand,
                        view => view.SidebarToggleButton)
                    .DisposeWith(disposables);

                Observable
                    .FromEventPattern(ModList.RowDragDropController, nameof(ModList.RowDragDropController.Dropped))
                    .Subscribe(_ => OnRowDropped(_.Sender, _.EventArgs as GridRowDroppedEventArgs));

                //ModList.RowDragDropController.Dropped += OnRowDropped;
            });

            InteractionHelpers.ModViewModelInteraction.RegisterHandler(
                async interaction =>
                {
                    var action = await this.DisplayModSortDialog(interaction.Input);

                    interaction.SetOutput(action);
                });
        }

        private async Task<ZipModifyArgs> DisplayModSortDialog(IEnumerable<ModFileModel> input)
        {
            var inputDialog = new ModFilesValidationView(new ModFilesValidationViewModel(input));
            if (inputDialog.ShowDialog() == true)
            {
                var output = inputDialog.GetOutput();
                return new ZipModifyArgs(input, output);
            }
            return new ZipModifyArgs(null, null);
        }


        private void OnRowDropped(object sender, GridRowDroppedEventArgs e)
        {
            if (e.DropPosition == DropPosition.None)
            {
                return;
            }
            if (e.Data.GetData("Records") is not ObservableCollection<object> records)
            {
                return;
            }
            if (ModList.DataContext is not ModListViewModel model)
            {
                return;
            }
            var targetRecord = model.BindingData[(int)e.TargetRecord];

            ModList.BeginInit();

            var translation = model.BindingData.Select(_ => _.LoadOrder).OrderBy(_ => _).ToList();
            var draggingRecords = records.Select(_ => (_ as ModViewModel).LoadOrder).ToList();
            foreach (var item in draggingRecords)
            {
                translation.Remove(item);
            }
            // Find the target record index after removing the records
            int targetIndex = translation.IndexOf(targetRecord.LoadOrder);
            int insertionIndex = e.DropPosition == DropPosition.DropAbove ? targetIndex : targetIndex + 1;
            insertionIndex = insertionIndex < 0 ? 0 : insertionIndex;
            // Insert dragging records to the target position
            for (int i = draggingRecords.Count - 1; i >= 0; i--)
            {
                translation.Insert(insertionIndex, draggingRecords[i]);
            }

            foreach (var modViewModel in model.BindingData)
            {
                var oldOrder = modViewModel.LoadOrder;
                var newOrder = translation.IndexOf(oldOrder);
                modViewModel.LoadOrder = newOrder;
            }

            
            ModList.EndInit();
        }
    }
}
