using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
            RawDirBrowseCommand = ReactiveCommand.Create<string>(RawDirBrowseExecute);
            ModDirOpenCommand = ReactiveCommand.Create<string>(ModDirOpenExecute);
            RawDirOpenCommand = ReactiveCommand.Create<string>(RawDirOpenExecute);
        }

        #region properties


        public ReactiveCommand<string, Unit> ModDirBrowseCommand { get; }
        public ReactiveCommand<string, Unit> RawDirBrowseCommand { get; }
        public ReactiveCommand<string, Unit> ModDirOpenCommand { get; }
        public ReactiveCommand<string, Unit> RawDirOpenCommand { get; }

        #endregion

        #region methods

        /// <summary>
        /// Show the given folder in the windows explorer.
        /// </summary>
        /// <param name="path">The file/folder to show.</param>
        public static void ShowFolderInExplorer(string path)
        {
            if (Directory.Exists(path))
            {
                Process.Start("explorer.exe", "\"" + path + "\"");
            }
        }

        private void ModDirOpenExecute(string param) => ShowFolderInExplorer(_settingsService.LocalModFolder);

        private void RawDirOpenExecute(string param) => ShowFolderInExplorer(_settingsService.LocalRawFolder);

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
        private void RawDirBrowseExecute(string param)
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

            _settingsService.LocalRawFolder = dir;
        }

        #endregion
    }
}
