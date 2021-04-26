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
using WolvenManager.App.ViewModels;

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
        }
    }
}
