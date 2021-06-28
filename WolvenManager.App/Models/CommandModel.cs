using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.CommandLine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reactive.Joins;
using System.Security.RightsManagement;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CP77Tools.Tasks;
using Microsoft.Extensions.Options;
using ReactiveUI;
using WolvenKit.Common;
using WolvenKit.Common.DDS;
using WolvenKit.RED4.CR2W.Archive;
using WolvenManager.App.Editors;
using WolvenManager.App.Services;

namespace WolvenManager.Models
{
    public abstract class CommandModel : ReactiveObject
    {
        protected readonly ISettingsService _settingsService;
        private readonly INotificationService _notificationService;

        
        public CommandModel(ISettingsService settingsService,
            INotificationService notificationService)
        {
            _settingsService = settingsService;
            _notificationService = notificationService;
        }


        [Browsable(false)]
        public abstract string Name { get; }

        public abstract Task ExecuteAsync(IConsoleFunctions consoleFunctions);

        public override string ToString() => Name;

        protected string GetPathArg(string input) => string.IsNullOrEmpty(input) ? "" : input.TrimStart('\"').TrimEnd('\"');

        protected string[] GetPathArgs(params string[] inputs)
        {
            List<string> result = new();
            result = inputs
                .Aggregate(result, (current, list) => current.Concat(list.Split(';'))
                .Where(_ => !string.IsNullOrEmpty(_) && !string.IsNullOrWhiteSpace(_) )
                .Select(_ => _.TrimStart('\"').TrimEnd('\"'))
                .ToList());

            return result.ToArray();
        }

        protected T[] GetEnumArgs<T>(ObservableCollection<object> input) where T : Enum => input.Cast<T>().ToArray();
    }


    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Archives), typeof(MultiFilePathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    [Editor(nameof(Extensions), typeof(EnumArrayEditor<ERedExtension>))]
    [Editor(nameof(VanillaArchives), typeof(EnumArrayEditor<EVanillaArchives>))]
    public class UnbundleCommandModel : CommandModel
    {
        public UnbundleCommandModel(ISettingsService settingsService, INotificationService notificationService) : base(
            settingsService, notificationService)
        {
            settingsService
                .WhenAnyValue(x => x.LocalModFolder)
                .Subscribe(s => this.RaisePropertyChanged(nameof(Outpath)));
        }


        private string _outpath;
        public override string Name => "unbundle";

        [Display(Order = 1)]
        [Category("Required - Input (One or more)")]
        [Description("Input folder path. Can be a folder or a list of folders")]
        public string Folders { get; set; } = "";

        [Display(Order = 2)]
        [Category("Required - Input (One or more)")]
        [Description("Input archive path. Can be an archive or a list of archives")]
        public string Archives { get; set; } = "";

        [Category("Optional")]
        [Description("Output directory.")]
        public string Outpath
        {
            get => _settingsService.IsModIntegrationEnabled ? _settingsService.LocalModFolder : _outpath;
            set
            {
                if (!_settingsService.IsModIntegrationEnabled)
                {
                    _outpath = value;
                }
                else
                {
                    _settingsService.LocalModFolder = value;
                }
            }
        }

        [Category("Optional")]
        [Description("Use optional search pattern (e.g. *.ink). If both regex and pattern is defined, pattern will be prioritized.")]
        public string Pattern { get; set; }

        [Category("Optional")]
        [Description("Use optional regex pattern. If both regex and pattern is defined, pattern will be prioritized.")]
        public string Regex { get; set; }
        [Category("Optional")] public string Hash { get; set; }
        [Category("Optional")] public bool DEBUG_decompress { get; set; }


        // custom stuff

        public ObservableCollection<object> Extensions { get; set; } = new();

        public ObservableCollection<object> VanillaArchives { get; set; } = new();

        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions)
        {
            var inputs = GetPathArgs(Folders, Archives).ToList();
            var chosenArchives = GetEnumArgs<EVanillaArchives>(VanillaArchives);
            foreach (var archive in chosenArchives)
            {
                var fullArchivePath = Path.Combine(_settingsService.GetArchiveDirectoryPath(), $"{archive}.archive");
                if (!inputs.Contains(fullArchivePath))
                {
                    inputs.Add(fullArchivePath);
                }
            }

            var chosenextension = GetEnumArgs<ERedExtension>(Extensions);
            if (chosenextension.Any())
            {
                var innerRegex = "";
                foreach (var format in chosenextension)
                {
                    innerRegex += $".*\\.{format}|";
                }

                Regex = $"^{innerRegex.TrimEnd('|')}$";
            }
            

            await Task.Run(() =>
                consoleFunctions.UnbundleTask(inputs.ToArray(), GetPathArg(Outpath), Hash, Pattern, Regex,
                    DEBUG_decompress));
        }

    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Archives), typeof(MultiFilePathEditor))]
    [Editor(nameof(OutputDirectory), typeof(SingleFolderPathEditor))]
    [Editor(nameof(RawOutputDirectory), typeof(SingleFolderPathEditor))]
    [Editor(nameof(Forcebuffers), typeof(EnumArrayEditor<ECookedFileFormat>))]
    [Editor(nameof(VanillaArchives), typeof(EnumArrayEditor<EVanillaArchives>))]
    [Editor(nameof(Extensions), typeof(EnumArrayEditor<ERedExtension>))]
    public class UncookCommandCommandModel : CommandModel
    {
        private string _outputDirectory;
        private string _rawOutputDirectory;
        public override string Name => "uncook";

        [Display(Order = 1)]
        [Category("Required - Input (One or more)")]
        [Description("Input folder path. Can be a folder or a list of folders")]
        public string Folders { get; set; } = "";

        [Display(Order = 2)]
        [Category("Required - Input (One or more)")]
        [Description("Input archive path. Can be an archive or a list of archives")]
        public string Archives { get; set; } = "";

        [Category("Optional")]
        [Description("Output directory.")]
        public string OutputDirectory
        {
            get => _settingsService.IsModIntegrationEnabled ? _settingsService.LocalModFolder : _outputDirectory;
            set
            {
                if (!_settingsService.IsModIntegrationEnabled)
                {
                    _outputDirectory = value;
                }
                else
                {
                    _settingsService.LocalModFolder = value;
                }
            }
        }

        [Category("Optional")]
        [Description("Optional seperate directory to extract raw files to.")]
        public string RawOutputDirectory
        {
            get => _settingsService.IsModIntegrationEnabled
                ? string.IsNullOrEmpty(_settingsService.LocalRawFolder) || !Directory.Exists(_settingsService.LocalRawFolder)
                    ? _settingsService.LocalModFolder
                    : _settingsService.LocalRawFolder
                : _rawOutputDirectory;
            set
            {
                if (!_settingsService.IsModIntegrationEnabled)
                {
                    _rawOutputDirectory = value;
                }
                else
                {
                    _settingsService.LocalRawFolder = value;
                }
            }
        }

        [Category("Optional")]
        [Description("Use optional search pattern (e.g. *.ink). If both regex and pattern is defined, pattern will be prioritized.")]
        public string Pattern { get; set; }

        [Category("Optional")]
        [Description("Use optional regex pattern. If both regex and pattern is defined, pattern will be prioritized.")]
        public string Regex { get; set; }

        [Category("Optional")]
        [Description("Flips textures vertically")]
        public bool flip { get; set; }

        [Category("Optional")]
        [Description("Also unbundle files.")]
        public bool unbundle { get; set; }

        [Category("Optional")]
        [Description("Format to uncook textures into (tga, bmp, jpg, png, dds), DDS by default")]
        public EUncookExtension uext { get; set; }

        [Category("Optional")]
        [Description("Force uncooking to buffers for given extension. e.g. mesh")]
        public ObservableCollection<object> Forcebuffers { get; set; } = new();

        [Category("Optional")]
        [Description("Extract single file with a given hash.")]
        public ulong Hash { get; set; }


        public ObservableCollection<object> VanillaArchives { get; set; } = new();

        public ObservableCollection<object> Extensions { get; set; } = new();


        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions)
        {
            var inputs = GetPathArgs(Folders, Archives).ToList();
            var chosenArchives = GetEnumArgs<EVanillaArchives>(VanillaArchives);
            foreach (var archive in chosenArchives)
            {
                var fullArchivePath = Path.Combine(_settingsService.GetArchiveDirectoryPath(), $"{archive}.archive");
                if (!inputs.Contains(fullArchivePath))
                {
                    inputs.Add(fullArchivePath);
                }
            }

            var chosenextension = GetEnumArgs<ERedExtension>(Extensions);
            if (chosenextension.Any())
            {
                var innerRegex = "";
                foreach (var format in chosenextension)
                {
                    innerRegex += $".*\\.{format}|";
                }

                Regex = $"^{innerRegex.TrimEnd('|')}$";
            }

            await Task.Run(() =>
                consoleFunctions.UncookTask(inputs.ToArray(), OutputDirectory, GetPathArg(RawOutputDirectory), uext, flip, Hash, Pattern, Regex,
                    unbundle, GetEnumArgs<ECookedFileFormat>(Forcebuffers)));
        }

        public UncookCommandCommandModel(ISettingsService settingsService, INotificationService notificationService) :
            base(settingsService, notificationService)
        {
            settingsService
                .WhenAnyValue(x => x.LocalModFolder)
                .Subscribe(s => this.RaisePropertyChanged(nameof(OutputDirectory)));
            settingsService
                .WhenAnyValue(x => x.LocalRawFolder)
                .Subscribe(s => this.RaisePropertyChanged(nameof(RawOutputDirectory)));
        }
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Files), typeof(MultiFilePathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    [Editor(nameof(forcebuffers), typeof(EnumArrayEditor<ECookedFileFormat>))]
    public class ExportCommandCommandModel : CommandModel
    {
        public override string Name => "export";

        [Display(Order = 1)]
        [Category("Required - Input (One or more)")]
        [Description("Input folder path. Can be a folder or a list of folders")]
        public string Folders { get; set; } = "";

        [Display(Order = 2)]
        [Category("Required - Input (One or more)")]
        [Description("Input file path. Can be a file or a list of files")]
        public string Files { get; set; } = "";

        private string _outpath;
        [Category("Optional")]
        [Description("Output directory.")]
        public string Outpath
        {
            get => _settingsService.IsModIntegrationEnabled ? _settingsService.LocalModFolder : _outpath;
            set
            {
                if (!_settingsService.IsModIntegrationEnabled)
                {
                    _outpath = value;
                }
                else
                {
                    _settingsService.LocalModFolder = value;
                }
            }
        }

        [Category("Optional")]
        [Description("Flips textures vertically")]
        public bool flip { get; set; }

        [Category("Optional")]
        [Description("Format to uncook textures into (tga, bmp, jpg, png, dds), DDS by default")]
        public EUncookExtension uext { get; set; }

        [Category("Optional")]
        [Description("Force uncooking to buffers for given extension. e.g. mesh")]
        public ObservableCollection<object> forcebuffers { get; set; }

        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.ExportTask(GetPathArgs(Folders, Files), GetPathArg(Outpath), uext, flip, GetEnumArgs<ECookedFileFormat>(forcebuffers)));

        public ExportCommandCommandModel(ISettingsService settingsService, INotificationService notificationService) :
            base(settingsService, notificationService)
        {
            settingsService
                .WhenAnyValue(x => x.LocalModFolder)
                .Subscribe(s => this.RaisePropertyChanged(nameof(Outpath)));
        }
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Files), typeof(MultiFilePathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    public class ImportCommandCommandModel : CommandModel
    {
        public override string Name => "import";

        [Display(Order = 1)]
        [Category("Required - Input (One or more)")]
        [Description("Input folder path. Can be a folder or a list of folders")]
        public string Folders
        {
            get => _settingsService.IsModIntegrationEnabled
                ? string.IsNullOrEmpty(_settingsService.LocalRawFolder) || !Directory.Exists(_settingsService.LocalRawFolder)
                    ? _settingsService.LocalModFolder
                    : _settingsService.LocalRawFolder
                : _folders;
            set
            {
                if (!_settingsService.IsModIntegrationEnabled)
                {
                    _folders = value;
                }
                else
                {
                    _settingsService.LocalRawFolder = value;
                }
            }
        }

        [Display(Order = 2)]
        [Category("Required - Input (One or more)")]
        [Description("Input file path. Can be a file or a list of files")]
        public string Files { get; set; } = "";

        private string _outpath;
        private string _folders = "";

        [Category("Optional")]
        [Description("Output directory. If used with --keep, this is the folder for the redengine files to rebuild.")]
        public string Outpath
        {
            get => _settingsService.IsModIntegrationEnabled ? _settingsService.LocalModFolder : _outpath;
            set
            {
                if (!_settingsService.IsModIntegrationEnabled)
                {
                    _outpath = value;
                }
                else
                {
                    _settingsService.LocalModFolder = value;
                }
            }
        }

        [Category("Optional")]
        [Description("Optionally keep existing CR2W files intact and only append the buffer.")]
        public bool keep { get; set; } = true;

        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.ImportTask(GetPathArgs(Folders, Files), GetPathArg(Outpath), keep));

        public ImportCommandCommandModel(ISettingsService settingsService, INotificationService notificationService) :
            base(settingsService, notificationService)
        {
            settingsService
                .WhenAnyValue(x => x.LocalModFolder)
                .Subscribe(s => this.RaisePropertyChanged(nameof(Outpath)));
            settingsService
                .WhenAnyValue(x => x.LocalRawFolder)
                .Subscribe(s => this.RaisePropertyChanged(nameof(_folders)));
        }
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    public class PackCommandCommandModel : CommandModel
    {
        public override string Name => "pack";

        private string _folders;
        [Display(Order = 1)]
        [Category("Required - Input (One or more)")]
        [Description("Input folder path. Can be a folder or a list of folders")]
        public string Folders
        {
            get => _settingsService.IsModIntegrationEnabled ? _settingsService.LocalModFolder : _folders;
            set
            {
                if (!_settingsService.IsModIntegrationEnabled)
                {
                    _folders = value;
                }
                else
                {
                    _settingsService.LocalModFolder = value;
                }
            }
        }

        [Category("Optional")]
        [Description("Output directory.")]
        public string Outpath { get; set; }

        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.PackTask(GetPathArgs(Folders), GetPathArg(Outpath)));

        public PackCommandCommandModel(ISettingsService settingsService, INotificationService notificationService) :
            base(settingsService, notificationService)
        {
            settingsService
                .WhenAnyValue(x => x.LocalModFolder)
                .Subscribe(s => this.RaisePropertyChanged(nameof(Folders)));
        }
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Archives), typeof(MultiFilePathEditor))]
    [Editor(nameof(VanillaArchives), typeof(EnumArrayEditor<EVanillaArchives>))]
    public class ArchiveCommandModel : CommandModel
    {
        public ArchiveCommandModel(ISettingsService settingsService, INotificationService notificationService) : base(settingsService, notificationService) { }

        public override string Name => "archive";

        [Display(Order = 1)]
        [Category("Required - Input (One or more)")]
        [Description("Input folder path. Can be a folder or a list of folders")]
        public string Folders { get; set; } = "";

        [Display(Order = 2)]
        [Category("Required - Input (One or more)")]
        [Description("Input archive path. Can be an archive or a list of archives")]
        public string Archives { get; set; } = "";

        [Display(Order = 3)]
        [Category("Required - Option (One or more)")]
        [Description("List all files in archive")]
        public bool List { get; set; }

        [Display(Order = 4)]
        [Category("Required - Option (One or more)")]
        [Description("Dump archive json.")]
        public bool Info { get; set; }

        [Category("Optional")]
        [Description("Use optional search pattern (e.g. *.ink). If both regex and pattern is defined, pattern will be prioritized.")]
        public string Pattern { get; set; }

        [Category("Optional")]
        [Description("Use optional regex pattern. If both regex and pattern is defined, pattern will be prioritized.")]
        public string Regex { get; set; }

        public ObservableCollection<object> VanillaArchives { get; set; } = new();




        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions)
        {
            var inputs = GetPathArgs(Folders, Archives).ToList();
            var chosenArchives = GetEnumArgs<EVanillaArchives>(VanillaArchives);
            foreach (var archive in chosenArchives)
            {
                var fullArchivePath = Path.Combine(_settingsService.GetArchiveDirectoryPath(), $"{archive}.archive");
                if (!inputs.Contains(fullArchivePath))
                {
                    inputs.Add(fullArchivePath);
                }
            }

            await Task.Run(() =>
                consoleFunctions.ArchiveTask(GetPathArgs(Folders, Archives), Pattern, Regex, Info, List));
        }
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Files), typeof(MultiFilePathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    public class CR2WCommandCommandModel : CommandModel
    {
        public override string Name => "cr2w";

        [Display(Order = 1)]
        [Category("Required - Input (One or more)")]
        [Description("Input folder path. Can be a folder or a list of folders")]
        public string Folders { get; set; } = "";

        [Display(Order = 2)]
        [Category("Required - Input (One or more)")]
        [Description("Input file path. Can be a file or a list of files")]
        public string Files { get; set; } = "";

        [Display(Order = 3)]
        [Category("Required - Option (One or more)")]
        [Description("Create a CR2W file from json or xml")]
        public bool deserialize { get; set; }

        [Display(Order = 4)]
        [Category("Required - Option (One or more)")]
        [Description("Serialize the CR2W file to json or xml.")]
        public bool serialize { get; set; }


        private string _outpath;
        [Category("Optional")]
        [Description("Output directory.")]
        public string Outpath
        {
            get => _settingsService.IsModIntegrationEnabled ? _settingsService.LocalModFolder : _outpath;
            set
            {
                if (!_settingsService.IsModIntegrationEnabled)
                {
                    _outpath = value;
                }
                else
                {
                    _settingsService.LocalModFolder = value;
                }
            }
        }

        [Category("Optional")]
        [Description("Use optional search pattern (e.g. *.ink). If both regex and pattern is defined, pattern will be prioritized.")]
        public string Pattern { get; set; }

        [Category("Optional")]
        [Description("Use optional regex pattern. If both regex and pattern is defined, pattern will be prioritized.")]
        public string Regex { get; set; }

        [Category("Optional")]
        [Description("Use optional serialization format. Options are json and xml")]
        public ESerializeFormat format { get; set; }

        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.Cr2wTask(GetPathArgs(Folders, Files), GetPathArg(Outpath), deserialize, serialize, Pattern, Regex, format));

        public CR2WCommandCommandModel(ISettingsService settingsService, INotificationService notificationService) :
            base(settingsService, notificationService)
        {
            settingsService
                .WhenAnyValue(x => x.LocalModFolder)
                .Subscribe(s => this.RaisePropertyChanged(nameof(Outpath)));
        }
    }

    [Editor(nameof(File), typeof(SingleFilePathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    public class OodleCommandCommandModel : CommandModel
    {
        public override string Name => "oodle";

        [Display(Order = 1)]
        [Category("Required - Input (One or more)")]
        [Description("Input file path")]
        public string File { get; set; } = "";

        [Display(Order = 2)]
        [Category("Required - Input (One or more)")]
        [Description("Output directory.")]
        public string Outpath { get; set; }

        [Display(Order = 3)]
        [Category("Required - Option (One or more)")]
        [Description("Decompress with oodle kraken.")]
        public bool decompress { get; set; }

        [Display(Order = 4)]
        [Category("Required - Option (One or more)")]
        [Description("Compress with oodle kraken.")]
        public bool compress { get; set; }

        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.OodleTask(GetPathArg(File), GetPathArg(Outpath), decompress, compress));

        public OodleCommandCommandModel(ISettingsService settingsService, INotificationService notificationService) : base(settingsService, notificationService) { }
    }



}
