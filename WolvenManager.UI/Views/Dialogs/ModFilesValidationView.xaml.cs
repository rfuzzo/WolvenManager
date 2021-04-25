using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
using AdonisUI.Controls;
using ReactiveUI;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.TreeView;
using WolvenManager.App.ViewModels;
using WolvenManager.App.ViewModels.Dialogs;
using WolvenManager.App.ViewModels.PageViewModels;

namespace WolvenManager.UI.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ModFilesValidationDialog.xaml
    /// </summary>
    public partial class ModFilesValidationView : AdonisWindow, IViewFor<ModFilesValidationViewModel>
    {
        public ModFilesValidationViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ModFilesValidationViewModel)value;
        }

        public ModFilesValidationView(ModFilesValidationViewModel vm)
        {
            InitializeComponent();
            ViewModel = vm;
            DataContext = ViewModel;


            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.BoundCollection,
                        view => view.TreeView.ItemsSource)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                        viewModel => viewModel.OkCommand,
                        view => view.ButtonOK)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.CancelCommand,
                        view => view.ButtonCancel)
                    .DisposeWith(disposables);

            });

            TreeView.ItemDropping += OnRowDropped;
            TreeView.ItemDragStarting += ItemDragStarting;
        }

        private void ItemDragStarting(object sender, TreeViewItemDragStartingEventArgs e)
        {
            

        }

        private void OnRowDropped(object sender, TreeViewItemDroppingEventArgs e)
        {
            

        }
    }
}
