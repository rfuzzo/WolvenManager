using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;

namespace WolvenManager.App.Services
{
    public interface ISettingsDto
    {
        public string RED4ExecutablePath { get; set; }
        public string LocalModFolder { get; set; }
        public string LocalRawFolder { get; set; }
        public bool IsModIntegrationEnabled { get; set; }
    }

    public interface ISettingsService : ISettingsDto
    {
        #region Methods

        public Task SaveAsync();

        string GetGameRootPath();
        string GetScriptsDirectoryPath();
        string GetModsDirectoryPath();
        string GetArchiveDirectoryPath();
        string GetAppData();

        string GetOodlePath();

        #endregion Methods

        IObservable<bool> IsValid { get; }
        
    }
}
