using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.Json;
using System.Text.Json.Serialization;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace WolvenManager.App.Services
{
    public class AppSettingsService : ReactiveObject, IAppSettingsService
    {
        #region fields

        private static string ConfigurationPath
        {
            get
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;
                var filename = Path.GetFileNameWithoutExtension(path);
                var dir = Path.GetDirectoryName(path);
                return Path.Combine(dir ?? "", filename + "config.json");
            }
        }

        #endregion

        #region properties
        [Reactive]
        public string GamePath { get; set; }
        [Reactive]
        public string DepotPath { get; set; }
        [Reactive]
        public string CurrentProfile { get; set; }


        public string ScriptsDir => Path.Combine(GamePath, "r6", "scripts");


        #endregion


        #region methods





        public bool Load()
        {
            AppSettingsService config;
            try
            {
                if (File.Exists(ConfigurationPath))
                {
                    var jsonString = File.ReadAllText(ConfigurationPath);
                    config = JsonSerializer.Deserialize<AppSettingsService>(jsonString);
                }
                else
                {
                    // Defaults
                    config = new AppSettingsService
                    {

                    };
                }
            }
            catch (Exception)
            {
                // Defaults
                config = new AppSettingsService
                {

                };
            }


            // check sanity
            CheckSelf(config);
            

            return true;
        }

        public IObservable<bool> IsValid()
        {
            var canExecute = this.WhenAnyValue(
                x => x.GamePath,
                (gamepath) =>
                    !string.IsNullOrEmpty(gamepath) && 
                    Directory.Exists(gamepath) 
                     
                    );

            return canExecute;
        }

        public async void Save()
        {
            await using var createStream = File.Create(ConfigurationPath);
            await JsonSerializer.SerializeAsync(createStream, this);
        }

        public void CheckSelf(AppSettingsService config)
        {
            // create default dir
            if (!Directory.Exists(config.DepotPath))
            {
                
            }

            if (!Directory.Exists(config.GamePath))
            {

            }


            this.DepotPath = config.DepotPath;
            this.GamePath = config.GamePath;
        }

        #endregion methods



        

    }
}
