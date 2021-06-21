using System.Linq;
using System.Reactive;
using Microsoft.WindowsAPICodePack.Dialogs;
using ReactiveUI;
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

        #region properties
        

        public ReactiveCommand<string, Unit> BrowseCommand { get; }

        #endregion



        public SettingsViewModel(IScreen screen = null) : base(typeof(SettingsViewModel), screen)
        {
            


            BrowseCommand = ReactiveCommand.Create<string>(BrowseFolderExecute);

            
            
        }

        #region methods
        
        private void BrowseFolderExecute(string param)
        {
            var openFolder = new CommonOpenFileDialog {AllowNonFileSystemItems = true, Multiselect = false, IsFolderPicker = true, Title = "Select folders"};
            if (openFolder.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            // get all the directories in selected dirctory
            var dir = openFolder.FileNames.FirstOrDefault();

            if (string.IsNullOrEmpty(dir))
            {
                return;
            }

            switch (param)
            {
                case GameDirParameter:
                    _settingsService.GamePath = dir;
                    break;
                default:
                    break;
            }

            _settingsService.Save();
        }

        #endregion

        #region events

        

        #endregion

    }
}
