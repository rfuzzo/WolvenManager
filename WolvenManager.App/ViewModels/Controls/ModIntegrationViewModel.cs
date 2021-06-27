using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using WolvenKit.Common.Services;
using DynamicData;
using Microsoft.WindowsAPICodePack.Dialogs;
using ReactiveUI.Fody.Helpers;
using WolvenManager.App.Services;

namespace WolvenManager.App.ViewModels.Controls
{
    public class ModIntegrationViewModel : MainViewModel
    {
        private readonly ILoggerService _loggerService;
        public readonly ISettingsService _settingsService;

        

        public ModIntegrationViewModel(
            ILoggerService loggerService,
            ISettingsService settingsService
            )
        {
            _loggerService = loggerService;
            _settingsService = settingsService;

            ModDirBrowseCommand = ReactiveCommand.Create<string>(ModDirBrowseExecute);
        }

        #region properties


        public ReactiveCommand<string, Unit> ModDirBrowseCommand { get; }
        //[Reactive] public bool IsEnabled { get; set; }

        #endregion

        #region methods

        private void ModDirBrowseExecute(string param)
        {
            var openFolder = new CommonOpenFileDialog
            {
                AllowNonFileSystemItems = true,
                Multiselect = false,
                IsFolderPicker = true,
                Title = "Select folders"
            };

            if (openFolder.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            var dir = openFolder.FileNames.FirstOrDefault();
            if (string.IsNullOrEmpty(dir))
            {
                return;
            }

            _settingsService.LocalModFolder = dir;
        }

        #endregion
    }
}
