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
using Splat;
using WolvenManager.App.Arguments;
using WolvenManager.App.Services;
using WolvenManager.App.Utility;
using WolvenManager.App.ViewModels;
using WolvenManager.App.ViewModels.Dialogs;

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


        public MainWindow(AppViewModel vm = null)
        {
            ViewModel = vm ?? Locator.Current.GetService<AppViewModel>();
            DataContext = ViewModel;

            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.BindCommand(ViewModel,
                        viewModel => viewModel.RoutingCommand,
                        view => view.FilesButton)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.RoutingCommand,
                        view => view.ModkitButton)
                    .DisposeWith(disposables);

                // search
                this.BindCommand(ViewModel,
                        viewModel => viewModel.RoutingSearchCommand,
                        view => view.SearchButton)
                    .DisposeWith(disposables);
                // mods
                this.BindCommand(ViewModel,
                        viewModel => viewModel.RoutingModsCommand,
                        view => view.ModsButton)
                    .DisposeWith(disposables);


                // routing
                this.OneWayBind(ViewModel, 
                        x => x.Router, 
                        x => x.RoutedViewHost.Router)
                    .DisposeWith(disposables);

                // bottom content holder
                this.OneWayBind(ViewModel,
                    viewModel => viewModel.IsBottomContentVisible,
                    view => view.BottomContent.Visibility)
                    .DisposeWith(disposables);

                // settings
                this.BindCommand(ViewModel,
                        viewModel => viewModel.RoutingSettingsCommand,
                        view => view.SettingsMenuItem)
                    .DisposeWith(disposables);
                this.BindCommand(ViewModel,
                        viewModel => viewModel.CheckForUpdatesCommand,
                        view => view.CheckForUpdatesMenuItem)
                    .DisposeWith(disposables);
                //this.OneWayBind(ViewModel,
                //        viewModel => viewModel.SettingsIconName,
                //        view => view.SettingsIcon.Kind)
                //    .DisposeWith(disposables);
                

                // statusbar
                this.OneWayBind(ViewModel,
                        x => x.Progress,
                        x => x.StatusBarProgressBar.Progress)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel,
                        x => x.Version,
                        x => x.StatusBarVersionLabel.Content)
                    .DisposeWith(disposables);
            });


            //InteractionHelpers.ModViewModelInteraction.RegisterHandler(
            //    async interaction =>
            //    {
            //        //var action = await this.DisplayModSortDialog(interaction.Input);
            //        //interaction.SetOutput(action);
            //    });
        }


        //private async Task<ZipModifyArgs> DisplayModSortDialog(IEnumerable<ModFileModel> input)
        //{
        //    var inputDialog = new PackageResolverView(new PackageResolverViewModel(input))
        //    {
        //        Owner = Application.Current.MainWindow
        //    };
        //    this.Overlay.Opacity = 0.5;
        //    this.Overlay.Background = new SolidColorBrush(Colors.White);

        //    var output = new Dictionary<string, string>();
        //    if (inputDialog.ShowDialog() == true)
        //    {
        //        output = inputDialog.GetOutput().ToDictionary(_ => _.Name, _ => _.ComputedFullName);
        //    }

        //    this.Overlay.Opacity = 1;
        //    this.Overlay.Background = new SolidColorBrush(Colors.Transparent);

        //    return new ZipModifyArgs(output);
        //}
        private void SettingsButton_OnClick(object sender, RoutedEventArgs e) => SettingsContextMenu.IsOpen = true;
    }
}
