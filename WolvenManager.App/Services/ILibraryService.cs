using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using WolvenManager.App.Models;
using WolvenManager.App.ViewModels;

namespace WolvenManager.App.Services
{
    public interface ILibraryService
    {
        public IObservable<IChangeSet<ModModel, string>> Connect();

        public void AddModToLibrary(ModModel model);
    }
}
