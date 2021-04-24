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
using WolvenManager.App.ViewModels;
using WolvenManager.App.ViewModels.PageViewModels;

namespace WolvenManager.UI.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : ReactiveUserControl<SettingsViewModel>
    {
        public SettingsView()
        {
            InitializeComponent();
            //ViewModel = new SettingsViewModel();

            this.WhenActivated(disposables =>
            {
                //props
                this.OneWayBind(ViewModel, 
                        x => x.UrlPathSegment, 
                        x => x.TitleBlock.Content)
                    .DisposeWith(disposables);


                this.Bind(ViewModel,
                    viewModel => viewModel.Settings.GamePath,
                    view => view.GameDirTextBox.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel,
                        viewModel => viewModel.Settings.DepotPath,
                        view => view.LibraryTextBox.Text)
                    .DisposeWith(disposables);


                this.Bind(ViewModel,
                        viewModel => viewModel.Settings.IsLibraryEnabled,
                        view => view.ModLibraryCheckBox.IsChecked)
                    .DisposeWith(disposables);
                this.Bind(ViewModel,
                        viewModel => viewModel.Settings.IsLibraryEnabled,
                        view => view.LibraryButton.IsEnabled)
                    .DisposeWith(disposables);
                this.Bind(ViewModel,
                        viewModel => viewModel.Settings.IsLibraryEnabled,
                        view => view.LibraryTextBox.IsEnabled)
                    .DisposeWith(disposables);




                // commands
                this.BindCommand(ViewModel,
                        viewModel => viewModel.BrowseCommand,
                        view => view.GameDirButton)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.BrowseCommand,
                        view => view.LibraryButton)
                    .DisposeWith(disposables);

            });
        }
    }
}
