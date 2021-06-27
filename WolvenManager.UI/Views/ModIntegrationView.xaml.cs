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
using ReactiveUI.Validation.Extensions;
using Splat;
using WolvenManager.App.ViewModels.Controls;

namespace WolvenManager.UI.Views
{
    /// <summary>
    /// Interaction logic for ModIntegrationView.xaml
    /// </summary>
    public partial class ModIntegrationView : ReactiveUserControl<ModIntegrationViewModel>
    {
        public ModIntegrationView()
        {
            InitializeComponent();

            ViewModel = Locator.Current.GetService<ModIntegrationViewModel>();
            DataContext = ViewModel;

            this.WhenActivated(disposables =>
            {
                // Main Panel
                this.Bind(ViewModel,
                        viewModel => viewModel._settingsService.IsModIntegrationEnabled,
                        view => view.MainPanel.IsEnabled)
                    .DisposeWith(disposables);

                this.Bind(ViewModel,
                        viewModel => viewModel._settingsService.IsModIntegrationEnabled,
                        view => view.IsEnabledCheckbox.IsChecked)
                    .DisposeWith(disposables);

                // Mod Folder Textbox
                this.Bind(ViewModel,
                        viewModel => viewModel._settingsService.LocalModFolder,
                        view => view.GameDirTextBox.Text)
                    .DisposeWith(disposables);
                this.BindValidation(ViewModel,
                        vm => vm._settingsService.RED4ExecutablePath,
                        view => view.GameDirTextBoxValidationLabel.Content)
                    .DisposeWith(disposables);

                // commands
                this.BindCommand(ViewModel,
                        viewModel => viewModel.ModDirBrowseCommand,
                        view => view.GameDirButton)
                    .DisposeWith(disposables);

            });
        }
    }
}
