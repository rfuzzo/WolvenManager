﻿using System;
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
using ReactiveUI;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.TreeView;
using WolvenKit.Common;
using WolvenKit.RED4.CR2W.Archive;
using WolvenManager.App.ViewModels.PageViewModels;

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
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.SelectedFiles,
                        view => view.DataGrid.ItemsSource)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                        viewModel => viewModel.BindingData,
                        view => view.dataPager.Source)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                        viewModel => viewModel.BindingHData,
                        view => view.LeftNavigation.ItemsSource)
                    .DisposeWith(disposables);
                this.Bind(ViewModel,
                        viewModel => viewModel.SelectedItem,
                        view => view.LeftNavigation.SelectedItem)
                    .DisposeWith(disposables);

            });

        }

        private void SfTreeView_OnSelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            if (!e.AddedItems.Any())
            {
                return;
            }

            if (ViewModel != null)
            {
                var model = e.AddedItems.First() as GameFileTreeNode;
                ViewModel.SelectedFiles = model.Files.Values.SelectMany(_ => _)
                    .Select(_ => new FileEntryViewModel(_ as FileEntry));
            }
        }

        private void LeftNavigation_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = e.AddedItems.ToList<GameFileTreeNode>().ToList();

            if (!list.Any())
            {
                return;
            }

            if (ViewModel != null)
            {
                var model = list.First();
                ViewModel.SelectedFiles = model.Files.Values.SelectMany(_ => _)
                    .Select(_ => new FileEntryViewModel(_ as FileEntry));
            }
        }
    }
}
