using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;

namespace WolvenManager.App.Services
{
    public interface ISettingsService
    {
        #region Properties
        string GamePath { get; set; }

        public string ScriptsDir { get; }
        public string ModsDir { get; }

        #endregion Properties


        #region Methods

        public Task Save();
        //public bool Load();

        #endregion Methods

        IObservable<bool> IsValid { get; }
    }
}
