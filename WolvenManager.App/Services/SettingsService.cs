using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Text.Json;
using System.Text.Json.Serialization;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WolvenKit.Common.Tools.Oodle;
using WolvenManager.Models;

namespace WolvenManager.App.Services
{
    public class SettingsService : ReactiveObject, ISettingsService
    {
        #region fields

        private string OodlePath;

        #endregion

        public SettingsService()
        {
            this.WhenAnyPropertyChanged(
                    nameof(GamePath)
                )
                .Subscribe(async _ =>
                {
                    if (_isLoaded)
                    {
                        await Save();
                    }
                });
        }

        #region commands

        #endregion



        #region properties

        [Reactive]
        public string GamePath { get; set; }

        [JsonIgnore] private bool _isLoaded;


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
        public string AppData
        {
            get
            {
                var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                var path = Path.Combine(appdata, "WolvenManager");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        #endregion


        #region methods





        public static SettingsService Load()
        {
            SettingsService config = null;

            try
            {
                if (File.Exists(Constants.ConfigurationPath))
                {
                    var jsonString = File.ReadAllText(Constants.ConfigurationPath);
                    config = JsonSerializer.Deserialize<SettingsService>(jsonString);
                }
            }
            catch (Exception)
            {
                
            }

            // defaults
            config ??= new SettingsService()
            {

            };
            config._isLoaded = true;
            return config;
        }

        public IObservable<bool> IsValid
        {
            get
            {
                var oodlePath = Path.Combine(GamePath, "bin", "x64", "oo2ext_7_win64.dll");
                return this.WhenAnyValue(
                    x => x.GamePath,
                    (gamepath) => !string.IsNullOrEmpty(gamepath) && Directory.Exists(gamepath) && File.Exists(oodlePath)
                );
            }
        }

        public async Task Save()
        {
            await using var createStream = File.Create(Constants.ConfigurationPath);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            await JsonSerializer.SerializeAsync(createStream, this, options);
        }

        public void CheckSelf(SettingsService config)
        {
            if (!Directory.Exists(config.GamePath))
            {

            }

            this.GamePath = config.GamePath;
        }

        #endregion methods



        

    }
}
