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
using Splat;
using Syncfusion.UI.Xaml.NavigationDrawer;
using Syncfusion.Windows.PropertyGrid;
using WolvenManager.App.ViewModels.Controls;
using WolvenManager.App.ViewModels.PageViewModels;

namespace WolvenManager.UI.Views
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : ReactiveUserControl<LogViewModel>
    {
        public LogView()
        {
            InitializeComponent();

            ViewModel = Locator.Current.GetService<LogViewModel>();
            DataContext = ViewModel;

            this.WhenActivated(disposables =>
            {

                // LogView
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.LogEntries,
                        view => view.ListView.ItemsSource)
                    .DisposeWith(disposables);

            });
        }
    }
}
