using System.IO;
using System.Linq;
using System.Reactive;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using Splat;
using WolvenManager.App.Attributes;
using WolvenManager.App.Services;

namespace WolvenManager.App.ViewModels.PageViewModels
{
    [RoutingUrl(Constants.RoutingIDs.Settings)]
    public class SettingsViewModel : PageViewModel
    {

        #region fields
        public const string GameDirParameter = "GameDirParameter";
        public const string LibraryDirParameter = "LibraryDirParameter";
        #endregion

        public SettingsViewModel() : base(typeof(SettingsViewModel))
        {
            BrowseCommand = ReactiveCommand.Create<string>(BrowseFolderExecute);
        }

        #region properties


        public ReactiveCommand<string, Unit> BrowseCommand { get; }

        #endregion

        #region methods

        private void BrowseFolderExecute(string param)
        {
            var dlg = new CommonOpenFileDialog()
            {
                Multiselect = false,
            };
            dlg.Filters.Add(new CommonFileDialogFilter("Cyberpunk2077.exe (Cyberpunk2077.exe)", ".exe"));

            if (dlg.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            var pickedName = dlg.FileNames.FirstOrDefault();
            if (string.IsNullOrEmpty(pickedName))
            {
                return;
            }
            if (!Path.GetFileName(pickedName).Equals("Cyberpunk2077.exe", System.StringComparison.Ordinal))
            {
                return;
            }

            _settingsService.RED4ExecutablePath = pickedName;
        }

        #endregion

    }
}
