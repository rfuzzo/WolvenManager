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
using ReactiveUI;
using Syncfusion.UI.Xaml.NavigationDrawer;
using WolvenManager.App.ViewModels.PageViewModels;

namespace WolvenManager.UI.Views
{
    /// <summary>
    /// Interaction logic for ModkitView.xaml
    /// </summary>
    public partial class ModkitView : ReactiveUserControl<ModkitViewModel>
    {
        public ModkitView()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel)
                .BindTo(this, x => x.DataContext);

            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Items,
                        view => view.navigationDrawer.ItemsSource)
                    .DisposeWith(disposables);
                this.Bind(ViewModel,
                        viewModel => viewModel.SelectedItem,
                        view => view.navigationDrawer.SelectedItem)
                    .DisposeWith(disposables);
                this.Bind(ViewModel,
                        viewModel => viewModel.SelectedItem,
                        view => view.PropertyGrid.SelectedObject)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.LogEntries,
                        view => view.ListView.ItemsSource)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel,
                        viewModel => viewModel.RunCommand,
                        view => view.RunButton)
                    .DisposeWith(disposables);

            });

        }
    }
}
