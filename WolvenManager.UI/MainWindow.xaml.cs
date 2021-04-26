using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
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
using WolvenManager.App.Arguments;
using WolvenManager.App.Models;
using WolvenManager.App.Utility;
using WolvenManager.App.ViewModels;
using WolvenManager.App.ViewModels.Dialogs;
using WolvenManager.UI.Views.Dialogs;

namespace WolvenManager.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : AdonisWindow, IViewFor<AppViewModel>
    {
        #region properties

        public AppViewModel ViewModel { get; set; }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (AppViewModel)value;
        }


        

        #endregion


        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new AppViewModel();
            DataContext = ViewModel;

            this.WhenActivated(disposableRegistration =>
            {
                this.BindCommand(ViewModel,
                        viewModel => viewModel.RoutingCommand,
                        view => view.MainButton)
                    .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.RoutingCommand,
                        view => view.ExtensionsButton)
                    .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.RoutingCommand,
                        view => view.LibraryButton)
                    .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.RoutingCommand,
                        view => view.ProfilesButton)
                    .DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.RoutingSettingsCommand,
                        view => view.SettingsButton)
                    .DisposeWith(disposableRegistration);


                // routing
                this.OneWayBind(ViewModel, 
                        x => x.Router, 
                        x => x.RoutedViewHost.Router)
                    .DisposeWith(disposableRegistration);

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
            var inputDialog = new PackageResolverView(new PackageResolverViewModel(input))
            {
                Owner = Application.Current.MainWindow
            };
            this.Overlay.Opacity = 0.5;
            this.Overlay.Background = new SolidColorBrush(Colors.White);

            var output = new Dictionary<string, string>();
            if (inputDialog.ShowDialog() == true)
            {
                output = inputDialog.GetOutput().ToDictionary(_ => _.Name, _ => _.ComputedFullName);
            }

            this.Overlay.Opacity = 1;
            this.Overlay.Background = new SolidColorBrush(Colors.Transparent);

            return new ZipModifyArgs(output);
        }
    }
}
