using System;
using System.Collections.Generic;
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
using AdonisUI.Controls;
using ReactiveUI;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.TreeView;
using WolvenManager.App.Arguments;
using WolvenManager.App.Models;
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
        #region fields

        private bool _isClosed = false;

        #endregion

        public ModFilesValidationView(ModFilesValidationViewModel vm)
        {
            InitializeComponent();
            this.Owner = Application.Current.MainWindow;
            ViewModel = vm;
            DataContext = ViewModel;

            this.Closed += DialogWindowClosed;

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

                ViewModel.WhenAnyValue(_ => _.BoundCollection)
                    .Subscribe(_ => TreeView.ExpandAll());

                Observable
                    .FromEventPattern(TreeView, nameof(TreeView.ItemDragStarting))
                    .Subscribe(_ => OnItemDragStarting(_.Sender, _.EventArgs as TreeViewItemDragStartingEventArgs));
                Observable
                    .FromEventPattern(TreeView, nameof(TreeView.ItemDropping))
                    .Subscribe(_ => OnItemDropping(_.Sender, _.EventArgs as TreeViewItemDroppingEventArgs));

            });
        }

        #region properties

        public ModFilesValidationViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (ModFilesValidationViewModel)value;
        }

        #endregion

        #region events

        void DialogWindowClosed(object sender, EventArgs e)
        {
            this._isClosed = true;
        }

        private static void OnItemDragStarting(object sender, TreeViewItemDragStartingEventArgs e)
        {
            if (e.DraggingNodes[0].Content is FileSystemInfoViewModel
            {
                Name: "r6" or "r6/scripts" or "archive/pc/mod" or "archive/pc" or "archive"
            })
            {
                e.Cancel = true;
            }
        }

        private static void OnItemDropping(object sender, TreeViewItemDroppingEventArgs e)
        {
            if (e.TargetNode.Content is FileSystemInfoViewModel
            {
                Name: "r6" or "r6/scripts" or "archive"
            })
            {
                e.Handled = true;
            }
        }

        #endregion

        public IEnumerable<ModFileModel> GetOutput()
        {
            return ViewModel.GetOutput();


        }
    }
}
