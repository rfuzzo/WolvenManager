using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using WolvenManager.Models;

namespace WolvenManager.App.Services
{
    public class SettingsService : ReactiveValidationObject, ISettingsService
    {
        #region fields

        private bool _isLoaded;

        #endregion

        public SettingsService()
        {
            this.WhenAnyPropertyChanged(
                    nameof(IsModIntegrationEnabled)
                )
                .Subscribe(_ =>
                {
                    if (_isLoaded)
                    {
                        Save();
                    }
                });

            this.ValidationRule(
                self => self.RED4ExecutablePath,
                name =>
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        return false;
                    }

                    if (!File.Exists(name))
                    {
                        return false;
                    }

                    RED4ExecutablePath = name;
                    Save();
                    return true;

                }, "File must be \"Cyberpunk2077.exe\"");

            this.ValidationRule(
                self => self.LocalModFolder,
                name =>
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        return false;
                    }

                    if (!Directory.Exists(name))
                    {
                        return false;
                    }

                    LocalModFolder = name;
                    Save();
                    return true;

                }, "Not a valid folder path");
            this.ValidationRule(
                self => self.LocalRawFolder,
                name =>
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        return false;
                    }

                    if (!Directory.Exists(name))
                    {
                        return false;
                    }

                    LocalRawFolder = name;
                    Save();
                    return true;

                }, "Not a valid folder path");
        }

        #region commands

        #endregion



        #region properties

        [Reactive] public string RED4ExecutablePath { get; set; }



        [Reactive] public bool IsModIntegrationEnabled { get; set; }

        [Reactive] public string LocalModFolder { get; set; }

        [Reactive] public string LocalRawFolder { get; set; }

        #endregion


        #region methods

        private static string GetAppDataFolder()
        {
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            var path = Path.Combine(appdata, Constants.AppDataFolder, Constants.ProductName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        private static string GetConfigurationPath() => Path.Combine(GetAppDataFolder(), "config.json");


        public string GetAppData() => GetAppDataFolder();
        public string GetTempDir()
        {
            var path = Path.Combine(GetAppDataFolder(), "Temp");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }


        public string GetOodlePath() => string.IsNullOrEmpty(GetGameRootPath()) ? null : Path.Combine(GetGameRootPath(), "bin", "x64", "oo2ext_7_win64.dll");

        public string GetScriptsDirectoryPath()
        {
            if (string.IsNullOrEmpty(GetGameRootPath()))
            {
                return null;
            }

            var path = Path.Combine(GetGameRootPath(), "r6", "scripts");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public string GetArchiveDirectoryPath()
        {
            if (string.IsNullOrEmpty(GetGameRootPath()))
            {
                return null;
            }

            var path = Path.Combine(GetGameRootPath(), "archive", "pc", "content");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public string GetModsDirectoryPath()
        {
            if (string.IsNullOrEmpty(GetGameRootPath()))
            {
                return null;
            }

            var path = Path.Combine(GetGameRootPath(), "archive", "pc", "mod");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public string GetGameRootPath()
        {
            if (string.IsNullOrEmpty(RED4ExecutablePath))
            {
                return null;
            }

            var fi = new FileInfo(RED4ExecutablePath);
            return fi.Directory is { Parent.Parent: { } } ? Path.Combine(fi.Directory.Parent.Parent.FullName) : null;
        }




        public static SettingsService Load()
        {
            SettingsService config = null;

            try
            {
                if (File.Exists(GetConfigurationPath()))
                {
                    var jsonString = File.ReadAllText(GetConfigurationPath());
                    var dto = JsonSerializer.Deserialize<SettingsDto>(jsonString);
                    if (dto != null)
                    {
                        config = FromDto(dto);
                    }
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




        public IObservable<bool> IsValid =>
            this.WhenAnyValue(
                x => x.RED4ExecutablePath,
                (exepath) => !string.IsNullOrEmpty(exepath) &&
                              File.Exists(exepath)
            );

        public void Save()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            var json = JsonSerializer.Serialize(new SettingsDto(this), options);
            File.WriteAllText(GetConfigurationPath(), json);
        }

        public async Task SaveAsync()
        {
            await using var createStream = File.Create(GetConfigurationPath());
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            await JsonSerializer.SerializeAsync(createStream, new SettingsDto(this), options);
        }

        #endregion methods

        private static SettingsService FromDto(SettingsDto dto)
        {
            var config = new SettingsService()
            {
                RED4ExecutablePath = dto.RED4ExecutablePath,
                LocalModFolder = dto.LocalModFolder,
                IsModIntegrationEnabled = dto.IsModIntegrationEnabled,
                LocalRawFolder = dto.LocalRawFolder,
            };
            return config;
        }

    }

    public class SettingsDto : ISettingsDto
    {
        private readonly SettingsService _settings;

        public SettingsDto()
        {

        }

        public SettingsDto(SettingsService settings)
        {
            _settings = settings;

            RED4ExecutablePath = _settings.RED4ExecutablePath;
            LocalModFolder = _settings.LocalModFolder;
            IsModIntegrationEnabled = _settings.IsModIntegrationEnabled;
            LocalRawFolder = _settings.LocalRawFolder;
        }

        public string RED4ExecutablePath { get; set; }
        public string LocalModFolder { get; set; }
        public string LocalRawFolder { get; set; }
        public bool IsModIntegrationEnabled { get; set; }
    }
}
