using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.TreeGrid;
using Syncfusion.UI.Xaml.TreeView;
using WolvenKit.Common;
using WolvenKit.Common.Model;
using WolvenKit.RED4.CR2W.Archive;
using WolvenManager.App.ViewModels.PageViewModels;
using SelectionChangedEventArgs = System.Windows.Controls.SelectionChangedEventArgs;

namespace WolvenManager.UI.Views
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : ReactiveUserControl<SearchViewModel>
    {
        public SearchView()
        {
            InitializeComponent();

            this.WhenAnyValue(x => x.ViewModel)
                .BindTo(this, x => x.DataContext);

            this.WhenActivated(disposables =>
            {
                // left nav
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.BindingHData,
                        view => view.LeftNavigation.ItemsSource)
                    .DisposeWith(disposables);
                this.Bind(ViewModel,
                        viewModel => viewModel.SelectedItem,
                        view => view.LeftNavigation.SelectedItem)
                    .DisposeWith(disposables);

                //right nav
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.SelectedFiles,
                        view => view.DataGrid.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.BindingData,
                        view => view.dataPager.Source)
                    .DisposeWith(disposables);
            });

        }

        //private void SfTreeView_OnSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        //{
        //    if (!e.AddedItems.Any())
        //    {
        //        return;
        //    }

        //    if (ViewModel == null)
        //    {
        //        return;
        //    }

        //    if (e.AddedItems.First() is RedFileSystemModel model)
        //    {
        //        ViewModel.SelectedFiles = model.Files
        //            .Select(_ => )
        //            .Select(_ => new FileEntryViewModel(_ as FileEntry));
        //    }
        //}

        private void LeftNavigation_OnSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            if (!e.AddedItems.Any())
            {
                return;
            }

            if (ViewModel == null)
            {
                return;
            }

            if (e.AddedItems.First() is RedFileSystemModel model )
            {
                //ViewModel.SelectedFiles = model.Files
                //    .Select(_ => new FileEntryViewModel(_ as FileEntry));
            }
        }

        private void dbg()
        {
            float z = 13.5f;
            bool sign = (BitConverter.GetBytes(z).First() & 1) == 1;
        }
    }
}
