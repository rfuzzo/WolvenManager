using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using WolvenManager.App.ViewModels;

namespace WolvenManager.App.Services
{
    public interface IWatcherService
    {
        public bool IsSuspended { get; set; }

        public IObservable<IChangeSet<ModItemViewModel, string>> ConnectMods();

        public void RefreshAsync();
    }
}
