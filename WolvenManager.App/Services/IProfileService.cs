using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using WolvenManager.App.ViewModels;

namespace WolvenManager.App.Services
{
    public interface IProfileService
    {
        public IObservable<IChangeSet<ModViewModel, string>> Connect();
    }
}
