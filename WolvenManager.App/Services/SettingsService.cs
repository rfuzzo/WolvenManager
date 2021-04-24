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
    public class SettingsService : ReactiveObject, ISettingsService
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

        public SettingsService()
        {
            this.WhenAnyValue(x => x.IsLibraryEnabled)
                .Subscribe(async x => 
                {
                    if (IsLoaded)
                    {
                        await Save();
                    }
                });
        }

        #region properties
        [Reactive]
        public string GamePath { get; set; }
        [Reactive]
        public string DepotPath { get; set; }
        [Reactive]
        public string CurrentProfile { get; set; }

        [Reactive]
        public bool IsLibraryEnabled { get; set; }


        [JsonIgnore]
        public bool IsLoaded { get; set; }





        public string ScriptsDir
        {
            get
            {
                if (string.IsNullOrEmpty(GamePath))
                {
                    return null;
                }

                var path = Path.Combine(GamePath, "r6", "scripts");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        public string ModsDir
        {
            get
            {
                if (string.IsNullOrEmpty(GamePath))
                {
                    return null;
                }

                var path = Path.Combine(GamePath, "archive", "pc", "mod");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        public static string DefaultDepotPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "WolvenModManager", "Library");

        #endregion


        #region methods





        public static SettingsService Load()
        {
            SettingsService config = new();

            try
            {
                if (File.Exists(ConfigurationPath))
                {
                    var jsonString = File.ReadAllText(ConfigurationPath);
                    config = JsonSerializer.Deserialize<SettingsService>(jsonString);
                }
            }
            catch (Exception)
            {
                
            }

            // defaults
            Directory.CreateDirectory(DefaultDepotPath);
            config ??= new()
            {
                IsLibraryEnabled = true,
                DepotPath = DefaultDepotPath
            };

            config.IsLoaded = true;
            return config;
        }

        public IObservable<bool> IsValid()
        {
            return this.WhenAnyValue(
                x => x.GamePath,
                (gamepath) =>
                    !string.IsNullOrEmpty(gamepath) && 
                    Directory.Exists(gamepath)
            );
        }

        public async Task Save()
        {
            await using var createStream = File.Create(ConfigurationPath);
            await JsonSerializer.SerializeAsync(createStream, this);
        }

        public void CheckSelf(SettingsService config)
        {
            if (!Directory.Exists(config.GamePath))
            {

            }


            this.DepotPath = config.DepotPath;
            this.GamePath = config.GamePath;
        }

        #endregion methods



        

    }
}
