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
using WolvenManager.App.Services;
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

            this.WhenActivated(disposables =>
            {
                // title
                this.OneWayBind(ViewModel, 
                        x => x.UrlPathSegment, 
                        x => x.TitleBlock.Content)
                    .DisposeWith(disposables);

                // GameDirTextBox
                this.Bind(ViewModel,
                        viewModel => viewModel._settingsService.RED4ExecutablePath,
                        view => view.GameDirTextBox.Text)
                    .DisposeWith(disposables);
                this.BindValidation(ViewModel,
                        vm => vm._settingsService.RED4ExecutablePath,
                        view => view.GameDirTextBoxValidationLabel.Content)
                    .DisposeWith(disposables);

                // commands
                this.BindCommand(ViewModel,
                        viewModel => viewModel.BrowseCommand,
                        view => view.GameDirButton)
                    .DisposeWith(disposables);

                
            });
        }
    }
}
