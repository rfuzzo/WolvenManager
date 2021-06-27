using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.CommandLine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

namespace WolvenManager.Models
{
    public abstract class CommandModel
    {
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
                .Where(_ => !string.IsNullOrEmpty(_))
                .Select(_ => _.TrimStart('\"').TrimEnd('\"'))
                .ToList());

            return result.ToArray();
        }

        protected T[] GetEnumArgs<T>(ObservableCollection<object> input) where T : Enum => input.Cast<T>().ToArray();
    }


    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Archives), typeof(MultiFilePathEditor))]
    public class ArchiveCommandModel : CommandModel
    {
        public override string Name => "archive";

        [Display(Order = 1)]
        [Category("Required - Input (One or more)")]
        public string Folders { get; set; } = "";
        [Display(Order = 2)]
        [Category("Required - Input (One or more)")]
        public string Archives { get; set; } = "";

        [Display(Order = 3)]
        [Category("Required - Option (One or more)")]
        public bool List { get; set; }
        [Display(Order = 4)]
        [Category("Required - Option (One or more)")]
        public bool Diff { get; set; }

        [Category("Optional")]
        public string Pattern { get; set; }
        [Category("Optional")]
        public string Regex { get; set; }


        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) => await Task.Run(() =>
            consoleFunctions.ArchiveTask(GetPathArgs(Folders, Archives), Pattern, Regex, Diff, List));
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Archives), typeof(MultiFilePathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    public class UnbundleCommandModel : CommandModel
    {
        public override string Name => "unbundle";

        [Display(Order = 1)]
        [Category("Required - Input (One or more)")]
        public string Folders { get; set; } = "";
        [Display(Order = 2)]
        [Category("Required - Input (One or more)")]
        public string Archives { get; set; } = "";

        [Category("Optional")] public string Outpath { get; set; }
        [Category("Optional")] public string Pattern { get; set; }
        [Category("Optional")] public string Regex { get; set; }
        [Category("Optional")] public string Hash { get; set; }
        [Category("Optional")] public bool DEBUG_decompress { get; set; }


        // custom stuff

        public List<ERedExtension> Extensions { get; set; }



        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.UnbundleTask(GetPathArgs(Folders, Archives), GetPathArg(Outpath), Hash, Pattern, Regex, DEBUG_decompress));
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Files), typeof(MultiFilePathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    public class CR2WCommandCommandModel : CommandModel
    {
        public override string Name => "cr2w";

        [Display(Order = 1)] [Category("Required - Input (One or more)")] public string Folders { get; set; } = "";
        [Display(Order = 2)] [Category("Required - Input (One or more)")] public string Files { get; set; } = "";

        [Display(Order = 3)] [Category("Required - Option (One or more)")] public bool deserialize { get; set; }
        [Display(Order = 4)] [Category("Required - Option (One or more)")] public bool serialize { get; set; }

        [Category("Optional")] public string Outpath { get; set; }
        [Category("Optional")] public string Pattern { get; set; }
        [Category("Optional")] public string Regex { get; set; }
        [Category("Optional")] public ESerializeFormat format { get; set; }

        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.Cr2wTask(GetPathArgs(Folders, Files), GetPathArg(Outpath), deserialize, serialize, Pattern, Regex, format));
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Files), typeof(MultiFilePathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    [Editor(nameof(forcebuffers), typeof(EnumArrayEditor<ECookedFileFormat>))]
    public class ExportCommandCommandModel : CommandModel
    {
        public override string Name => "export";

        [Display(Order = 1)] [Category("Required - Input (One or more)")] public string Folders { get; set; } = "";
        [Display(Order = 2)] [Category("Required - Input (One or more)")] public string Files { get; set; } = "";
        [Category("Optional")] public string Outpath { get; set; }
        [Category("Optional")] public bool flip { get; set; }
        [Category("Optional")] public EUncookExtension uext { get; set; }
        [Category("Optional")] public ObservableCollection<object> forcebuffers { get; set; }

        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.ExportTask(GetPathArgs(Folders, Files), GetPathArg(Outpath), uext, flip, GetEnumArgs<ECookedFileFormat>(forcebuffers)));
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Files), typeof(MultiFilePathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    public class ImportCommandCommandModel : CommandModel
    {
        public override string Name => "import";

        [Display(Order = 1)] [Category("Required - Input (One or more)")] public string Folders { get; set; } = "";
        [Display(Order = 2)] [Category("Required - Input (One or more)")] public string Files { get; set; } = "";
        [Category("Optional")] public string Outpath { get; set; }
        [Category("Optional")] public bool keep { get; set; } = true;

        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.ImportTask(GetPathArgs(Folders, Files), GetPathArg(Outpath), keep));
    }

    [Editor(nameof(File), typeof(SingleFilePathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    public class OodleCommandCommandModel : CommandModel
    {
        public override string Name => "oodle";

        [Display(Order = 1)] [Category("Required - Input (One or more)")] public string File { get; set; } = "";
        [Display(Order = 2)] [Category("Required - Input (One or more)")] public string Outpath { get; set; }
        [Display(Order = 3)] [Category("Required - Option (One or more)")] public bool decompress { get; set; }
        [Display(Order = 4)] [Category("Required - Option (One or more)")] public bool compress { get; set; }

        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.OodleTask(GetPathArg(File), GetPathArg(Outpath), decompress, compress));
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    public class PackCommandCommandModel : CommandModel
    {
        public override string Name => "pack";

        [Display(Order = 1)] [Category("Required - Input (One or more)")] public string Folders { get; set; } = "";
        [Category("Optional")] public string Outpath { get; set; }

        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.PackTask(GetPathArgs(Folders), GetPathArg(Outpath)));
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Files), typeof(MultiFilePathEditor))]
    [Editor(nameof(OutputDirectory), typeof(SingleFolderPathEditor))]
    [Editor(nameof(RawOutputDirectory), typeof(SingleFolderPathEditor))]
    [Editor(nameof(Forcebuffers), typeof(EnumArrayEditor<ECookedFileFormat>))]
    public class UncookCommandCommandModel : CommandModel
    {
        public override string Name => "uncook";

        [Display(Order = 1)] [Category("Required - Input (One or more)")] public string Folders { get; set; } = "";
        [Display(Order = 2)] [Category("Required - Input (One or more)")] public string Files { get; set; } = "";
        [Category("Optional")] public string OutputDirectory { get; set; }
        [Category("Optional")] public string RawOutputDirectory { get; set; }
        [Category("Optional")] public string Pattern { get; set; }
        [Category("Optional")] public string Regex { get; set; }
        [Category("Optional")] public bool flip { get; set; }
        [Category("Optional")] public bool unbundle { get; set; }
        [Category("Optional")] public EUncookExtension uext { get; set; }
        [Category("Optional")] public ObservableCollection<object> Forcebuffers { get; set; } = new();
        public ulong Hash { get; set; }


        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions)
        {
            

            await Task.Run(() =>
                consoleFunctions.UncookTask(GetPathArgs(
                        Folders, Files), OutputDirectory, GetPathArg(RawOutputDirectory), uext, flip, Hash, Pattern, Regex,
                    unbundle, GetEnumArgs<ECookedFileFormat>(Forcebuffers)));
        }
    }

}
