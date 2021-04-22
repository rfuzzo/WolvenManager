using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;

namespace WolvenManager.App.Services
{
    public interface IAppSettingsService
    {
        #region Properties
        string GamePath { get; set; }
        string DepotPath { get; set; }
        string CurrentProfile { get; set; }

        public string ScriptsDir { get; }

        #endregion Properties


        #region Methods

        public void Save();
        public bool Load();

        #endregion Methods

        IObservable<bool> IsValid();
    }
}
