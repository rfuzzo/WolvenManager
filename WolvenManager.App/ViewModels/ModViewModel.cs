using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using WolvenManager.App.Models;
using WolvenManager.App.Services;

namespace WolvenManager.App.ViewModels
{
    public class ModViewModel : MainViewModel
    {

        private readonly ISettingsService _settingsService;

        public ModViewModel(ModModel model)
        {
            _settingsService = Locator.Current.GetService<ISettingsService>();

            Model = model;

            this.WhenPropertyChanged(x => x.Enabled, false, () => true)
                .Subscribe(OnEnabledChanged);



            InstallModCommand = ReactiveCommand.Create(() =>
            {

            });
        }


        #region properties

        [Reactive]
        [Display(Order = 0)]
        public bool Enabled { get; set; }

        [Reactive]
        [Display(Order = 1)]
        public int LoadOrder { get; set; }

        // TODO: truly bind this
        public string Name => Model.Name;





        [Display(AutoGenerateField = false)]
        public string Id => Model.Id;

        [Display(AutoGenerateField = false)]
        public bool IsInLibrary => Model.IsInLibrary;

        [Display(AutoGenerateField = false)]
        public bool Installed => Model.Installed;

        [Reactive]
        public ModModel Model { get; set; }

        public IEnumerable<string> Files => Model.Files;

        public List<string> DisabledFiles { get; set; } = new();

        #endregion

        #region commands

        public ReactiveCommand<Unit, Unit> InstallModCommand { get; }

        #endregion

        private void OnEnabledChanged(PropertyValue<ModViewModel, bool> value)
        {
            var mod = value.Sender;
            if (value.Value)
            {
                // changed from disabled to enabled
                // run through all modfiles and enable
                foreach (var file in mod.Files)
                {
                    var gamefile = Path.Combine(_settingsService.GamePath, file);
                    var disabledfile = $"{gamefile}.disabled";
                    if (File.Exists(disabledfile))
                    {
                        if (!mod.DisabledFiles.Contains(file))
                            File.Move(disabledfile, gamefile);
                    }
                    // might be disabled
                    else if (mod.DisabledFiles.Contains(file))
                    {
                        // preserve disabled status
                        var doublydisabledFile = $"{disabledfile}.disabled";
                        if (File.Exists(doublydisabledFile))
                        {
                            File.Move(doublydisabledFile, disabledfile);
                        }
                    }
                    else
                    {
                        // something broke
                        // signal mod might be corrupted
                        //Debugger.Break();
                    }
                }
            }
            else
            {
                // changed from enabled to disabled
                // run through all modfiles and disable
                foreach (var file in mod.Files)
                {
                    var gamefile = Path.Combine(_settingsService.GamePath, file);
                    var disabledfile = $"{gamefile}.disabled";
                    if (File.Exists(gamefile))
                    {
                        File.Move(gamefile, disabledfile);
                    }
                    // might be disabled
                    else if (mod.DisabledFiles.Contains(file))
                    {
                        // preserve disabled status
                        var doublydisabledFile = $"{disabledfile}.disabled";
                        File.Move(disabledfile, doublydisabledFile);
                    }
                    else
                    {
                        // something broke
                        // signal mod might be corrupted
                        Debugger.Break();
                    }
                }
            }
        }


    }
}
