using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Splat;
using WolvenManager.App.Attributes;
using WolvenManager.App.Models;
using WolvenManager.App.Services;

namespace WolvenManager.App.ViewModels.PageViewModels
{
    [RoutingUrl(Constants.RoutingIDs.Main)]
    public class ModListViewModel : PageViewModel
    {
        

        public ModListViewModel(IScreen screen = null) : base(typeof(ModListViewModel), screen)
        {
            var currentProfile = Locator.Current.GetService<IProfileService>();

            BindingData = currentProfile.Items;

            InstallModCommand = ReactiveCommand.Create(InstallMod);
        }

        private void InstallMod()
        {
            


        }

        #region properties


        public readonly ReadOnlyObservableCollection<ModViewModel> BindingData;

        public ReactiveCommand<Unit, Unit> InstallModCommand { get; }


        #endregion

    }
}
