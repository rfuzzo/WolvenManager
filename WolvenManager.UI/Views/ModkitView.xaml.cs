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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;
using Syncfusion.UI.Xaml.NavigationDrawer;
using Syncfusion.Windows.PropertyGrid;
using WolvenKit.Common;
using WolvenKit.RED4.Archive;
using WolvenManager.App.Services;
using WolvenManager.App.ViewModels.PageViewModels;
using WolvenManager.Models;
using WolvenManager.UI.Editors;

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

                // button
                this.BindCommand(ViewModel,
                        viewModel => viewModel.RunCommand,
                        view => view.RunButton)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.TextBlockText,
                        view => view.RunTextBlock.Text)
                    .DisposeWith(disposables);

            });

        }

        private void PropertyGrid_OnAutoGeneratingPropertyGridItem(object sender, AutoGeneratingPropertyGridItemEventArgs e)
        {
            // hide certain inherited properties from propertygrid
            if (e.DisplayName is
                nameof(ReactiveObject.ThrownExceptions) or
                nameof(ReactiveObject.Changed) or
                nameof(ReactiveObject.Changing))
            {
                e.Cancel = true;
            }

            // Generate special editors for the properties for which default is not ok
            if (e.OriginalSource is PropertyItem { } propertyItem)
            {
                switch (propertyItem.DisplayName)
                {
                    case nameof(OodleCommandCommandModel.File):
                        propertyItem.Editor = new SingleFilePathEditor();
                        break;
                    case nameof(ExportCommandCommandModel.Files):
                    case nameof(UnbundleCommandModel.Archives):
                        propertyItem.Editor = new MultiFilePathEditor();
                        break;
                    case nameof(UnbundleCommandModel.Outpath):
                    case nameof(UncookCommandCommandModel.RawOutputDirectory):
                        propertyItem.Editor = new SingleFolderPathEditor();
                        break;
                    case nameof(UnbundleCommandModel.Folders):
                        propertyItem.Editor = new MultiFolderPathEditor();
                        break;
                    case nameof(UnbundleCommandModel.Extensions):
                        propertyItem.Editor = new EnumArrayEditor<ERedExtension>();
                        break;
                    case nameof(UnbundleCommandModel.VanillaArchives):
                        propertyItem.Editor = new EnumArrayEditor<EVanillaArchives>();
                        break;
                    case nameof(UncookCommandCommandModel.Forcebuffers):
                        propertyItem.Editor = new EnumArrayEditor<ECookedFileFormat>();
                        break;
                }
            }
        }
    }
}
