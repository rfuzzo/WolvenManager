using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;
using SharpCompress.Readers;
using Splat;
using WolvenManager.App.Arguments;
using WolvenManager.App.Models;
using WolvenManager.App.Services;

namespace WolvenManager.App.Utility
{
    public static class ModInstallHelper
    {
        /// <summary>
        /// Installs a mod from a given archive
        /// </summary>
        /// <param name="zipInfo"></param>
        /// <returns></returns>
        public static async Task<ModModel> InstallMod(FileInfo zipInfo)
        {
            var settingsService = Locator.Current.GetService<ISettingsService>();
            var notificationService = Locator.Current.GetService<INotificationService>();
            var watcherService = Locator.Current.GetService<IWatcherService>();
            if (settingsService == null || watcherService == null)
            {
                return null;
            }

            var modname = Path.GetFileNameWithoutExtension(zipInfo.FullName);
            // sanitize modname
            modname = modname.Split('-').FirstOrDefault();
            //TODO: some user interaction


            using var sha256Hash = SHA256.Create();
            var hash = sha256Hash.ComputeHash(await File.ReadAllBytesAsync(zipInfo.FullName));
            // is alrady in lib


            var extractPath = Path.GetFullPath(settingsService.GamePath);
            if (!PathExtensions.IsPathDirectory(extractPath))
            {
                extractPath += Path.DirectorySeparatorChar;
            }

            var entries = TryGetEntries(zipInfo).ToList();
            var files = entries
                .Where(_ => !_.IsDirectory)
                .ToList();

            var mod = new ModModel()
            {

                Files = files.Select(_ => _.Name),
                Name = modname,
                ZipHash = hash
            };



            ZipModifyArgs recovery = null;
            // check if mod is malformed
            if ((files.Any(_ => Path.GetExtension(_.Name) == ".reds") && entries.All(_ => _.Name != "r6/scripts/")) ||
                (files.Any(_ => Path.GetExtension(_.Name) == ".archive") && entries.All(_ => _.Name != "archive/pc/mod/")) ||
                files.Any(_ => Path.GetExtension(_.Name) is not ".archive" or ".reds"))
            {
                // open mod fixer view
                notificationService?.Success("Mod is improperly packed, please fix any errors by moving the files manually.");
                recovery = await InteractionHelpers.ModViewModelInteraction.Handle(files);

                if (recovery.Output == null)
                {
                    // canceled
                    return null;
                }
            }

            // do something about existing files
            // TODO: rn they get simply overwritten

            //extract
            watcherService.IsSuspended = true;
            var success = false;
            switch (zipInfo.Extension)
            {
                case ".zip":
                    success = ExtractZipMod(zipInfo.FullName, extractPath, recovery);
                    break;
                case ".7z":
                    success = Extract7ZMod(zipInfo.FullName, extractPath, recovery);
                    break;
                default:
                    break;
            }
            watcherService.IsSuspended = false;

            if (success)
            {
                notificationService?.Success($"{modname} installed.");
            }
            else
            {
                notificationService?.Error($"Could not completely install mod {modname}");
            }

            mod.Installed = success;

            return mod;

        }

        private static IEnumerable<ModFileModel> TryGetEntries(FileInfo zipInfo)
        {
            try
            {
                switch (zipInfo.Extension)
                {
                    case ".zip":
                    {
                        using Stream stream = File.OpenRead(zipInfo.FullName);
                        using var reader = ReaderFactory.Open(stream);
                        var zipEntries = new List<ModFileModel>();
                        while (reader.MoveToNextEntry())
                        {
                            zipEntries.Add(new ModFileModel(reader.Entry.Key, reader.Entry.IsDirectory));
                        }

                        //r6/scripts/
                        //r6/scripts/holserByTap.reds
                        return zipEntries;
                    }
                    case ".7z":
                    {
                        using var archive = SevenZipArchive.Open(zipInfo.FullName);
                        var entries = archive.Entries
                            .Select(_ => _.IsDirectory
                                ? new ModFileModel($"{_.Key}/", _.IsDirectory)
                                : new ModFileModel(_.Key, _.IsDirectory))
                            .ToList();
                        return entries;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static bool ExtractZipMod(string zipPath, string gameDir, ZipModifyArgs e)
        {
            using Stream stream = File.OpenRead(zipPath);
            using var reader = ReaderFactory.Open(stream);
            while (reader.MoveToNextEntry())
            {
                if (reader.Entry.IsDirectory)
                {
                    continue;
                }

                var relativepath = reader.Entry.Key;

                // after a repackaging
                if (e != null && e.Output.ContainsKey(reader.Entry.Key))
                {
                    var newEntry = e.Output[reader.Entry.Key];
                    relativepath = newEntry;
                }

                var destinationPath = Path.GetFullPath(Path.Combine(gameDir, relativepath));
                if (!destinationPath.StartsWith(gameDir, StringComparison.Ordinal))
                {
                    continue;
                }

                try
                {
                    if (File.Exists(destinationPath))
                    {
                        File.Delete(destinationPath);
                    }

                    var dir = Path.GetDirectoryName(destinationPath);
                    reader.WriteEntryToDirectory(dir, new ExtractionOptions()
                    {
                        ExtractFullPath = false,
                        Overwrite = false
                    });
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool Extract7ZMod(string zipPath, string gameDir, ZipModifyArgs e)
        {
            using var archive = SevenZipArchive.Open(zipPath);
            var entries = archive.Entries;

            foreach (var entry in entries)
            {
                if (entry.IsDirectory)
                {
                    continue;
                }

                var relativepath = entry.Key;

                // after a repackaging
                if (e != null && e.Output.ContainsKey(entry.Key))
                {
                    var newEntry = e.Output[entry.Key];
                    relativepath = newEntry;
                }

                var destinationPath = Path.GetFullPath(Path.Combine(gameDir, relativepath));
                if (!destinationPath.StartsWith(gameDir, StringComparison.Ordinal))
                {
                    continue;
                }

                try
                {
                    if (File.Exists(destinationPath))
                    {
                        File.Delete(destinationPath);
                    }

                    using var s = entry.OpenEntryStream();
                    using var fs = new FileStream(destinationPath, FileMode.CreateNew);
                    s.CopyTo(fs);

                    //entry.WriteEntryToDirectory(destinationPath, new ExtractionOptions()
                    //{
                    //    ExtractFullPath = false,
                    //    Overwrite = false
                    //});
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
