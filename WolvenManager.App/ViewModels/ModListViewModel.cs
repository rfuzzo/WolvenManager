using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using Splat;
using WolvenManager.App.Attributes;
using WolvenManager.App.Services;

namespace WolvenManager.App.ViewModels
{
    [RoutingUrl(Constants.Constants.RoutingIDs.Main)]
    public class ModListViewModel : PageViewModel
    {
        

        


        public ModListViewModel(IScreen screen = null) : base(typeof(ModListViewModel), screen)
        {
            var profileService = Locator.Current.GetService<IProfileService>();

            var disposable = profileService
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out bindingData)
                .Subscribe();
        }

        #region properties


        public readonly ReadOnlyObservableCollection<ModViewModel> bindingData;

        #endregion

    }
}
