using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Syncfusion.UI.Xaml.Grid;
using WolvenManager.App.ViewModels;
using WolvenManager.App.ViewModels.PageViewModels;
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
                

                this.Bind(ViewModel,
                        viewModel => viewModel.SelectedModViewModel,
                        view => view.ModList.SelectedItem)
                    .DisposeWith(disposables);

                 this.Bind(ViewModel,
                        viewModel => viewModel.SelectedModViewModels,
                        view => view.ModList.SelectedItems)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                        viewModel => viewModel.InstallModCommand,
                        view => view.InstallModButton)
                    .DisposeWith(disposables);
               

                //Observable
                //    .FromEventPattern(ModList.RowDragDropController, nameof(ModList.RowDragDropController.Dropped))
                //    .Subscribe(_ => OnRowDropped(_.Sender, _.EventArgs as GridRowDroppedEventArgs));

                //ModList.RowDragDropController.Dropped += OnRowDropped;
            });
        }

        


        //private void OnRowDropped(object sender, GridRowDroppedEventArgs e)
        //{
        //    if (e.DropPosition == DropPosition.None)
        //    {
        //        return;
        //    }
        //    if (e.Data.GetData("Records") is not ObservableCollection<object> records)
        //    {
        //        return;
        //    }
        //    if (ModList.DataContext is not ModListViewModel model)
        //    {
        //        return;
        //    }
        //    var targetRecord = model.BindingData[(int)e.TargetRecord];

        //    ModList.BeginInit();

        //    var translation = model.BindingData.Select(_ => _.LoadOrder).OrderBy(_ => _).ToList();
        //    var draggingRecords = records.Select(_ => (_ as ModViewModel).LoadOrder).ToList();
        //    foreach (var item in draggingRecords)
        //    {
        //        translation.Remove(item);
        //    }
        //    // Find the target record index after removing the records
        //    int targetIndex = translation.IndexOf(targetRecord.LoadOrder);
        //    int insertionIndex = e.DropPosition == DropPosition.DropAbove ? targetIndex : targetIndex + 1;
        //    insertionIndex = insertionIndex < 0 ? 0 : insertionIndex;
        //    // Insert dragging records to the target position
        //    for (int i = draggingRecords.Count - 1; i >= 0; i--)
        //    {
        //        translation.Insert(insertionIndex, draggingRecords[i]);
        //    }

        //    foreach (var modViewModel in model.BindingData)
        //    {
        //        var oldOrder = modViewModel.LoadOrder;
        //        var newOrder = translation.IndexOf(oldOrder);
        //        modViewModel.LoadOrder = newOrder;
        //    }

            
        //    ModList.EndInit();
        //}
    }
}
