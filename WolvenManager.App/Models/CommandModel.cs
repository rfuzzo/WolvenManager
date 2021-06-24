using System;
using System.Collections.Generic;
using System.CommandLine;
using System.ComponentModel;
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
    }


    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Archives), typeof(MultiFilePathEditor))]
    public class ArchiveCommandModel : CommandModel
    {
        public override string Name => "archive";

        public string Folders { get; set; } = "";
        public string Archives { get; set; } = "";
        public string Pattern { get; set; }
        public string Regex { get; set; }
        public bool List { get; set; }
        public bool Diff { get; set; }


        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) => await Task.Run(() => consoleFunctions.ArchiveTask(GetPathArgs(Folders, Archives), Pattern, Regex, Diff, List));
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Archives), typeof(MultiFilePathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    public class UnbundleCommandModel : CommandModel
    {
        public override string Name => "unbundle";

        public string Folders { get; set; } = "";
        public string Archives { get; set; } = "";
        public string Outpath { get; set; }
        public string Pattern { get; set; }
        public string Regex { get; set; }
        public string Hash { get; set; }
        public bool DEBUG_decompress { get; set; }


        // custom stuff

        public List<ERedExtension> Extensions { get; set; }



        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.UnbundleTask(GetPathArgs(Folders, Archives), Outpath, Hash, Pattern, Regex, DEBUG_decompress));
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Files), typeof(MultiFilePathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    public class CR2WCommandCommandModel : CommandModel
    {
        public override string Name => "cr2w";

        public string Folders { get; set; } = "";
        public string Files { get; set; } = "";
        public string Outpath { get; set; }
        public string Pattern { get; set; }
        public string Regex { get; set; }
        public bool deserialize { get; set; }
        public bool serialize { get; set; }
        public ESerializeFormat format { get; set; }

        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.Cr2wTask(GetPathArgs(Folders, Files), Outpath, deserialize, serialize, Pattern, Regex, format));
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Files), typeof(MultiFilePathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    public class ExportCommandCommandModel : CommandModel
    {
        public override string Name => "export";

        public string Folders { get; set; } = "";
        public string Files { get; set; } = "";
        public string Outpath { get; set; }
        public bool flip { get; set; }
        public EUncookExtension uext { get; set; }
        public ECookedFileFormat[] forcebuffers { get; set; }

        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.ExportTask(GetPathArgs(Folders, Files), Outpath, uext, flip, forcebuffers));
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Files), typeof(MultiFilePathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    public class ImportCommandCommandModel : CommandModel
    {
        public override string Name => "import";

        public string Folders { get; set; } = "";
        public string Files { get; set; } = "";
        public string Outpath { get; set; }
        public bool keep { get; set; }

        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.ImportTask(GetPathArgs(Folders, Files), Outpath, keep));
    }

    [Editor(nameof(File), typeof(SingleFilePathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    public class OodleCommandCommandModel : CommandModel
    {
        public override string Name => "oodle";

        public string File { get; set; } = "";
        public string Outpath { get; set; }
        public bool decompress { get; set; }
        public bool compress { get; set; }

        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.OodleTask(File, Outpath, decompress, compress));
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    public class PackCommandCommandModel : CommandModel
    {
        public override string Name => "pack";

        public string Folders { get; set; } = "";
        public string Outpath { get; set; }

        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.PackTask(GetPathArgs(Folders), Outpath));
    }

    [Editor(nameof(Folders), typeof(MultiFolderPathEditor))]
    [Editor(nameof(Files), typeof(MultiFilePathEditor))]
    [Editor(nameof(Outpath), typeof(SingleFolderPathEditor))]
    [Editor(nameof(raw), typeof(SingleFolderPathEditor))]
    [Editor(nameof(forcebuffers), typeof(EnumArrayEditor<ECookedFileFormat>))]
    public class UncookCommandCommandModel : CommandModel
    {
        public override string Name => "uncook";

        public string Folders { get; set; } = "";
        public string Files { get; set; } = "";
        public string Outpath { get; set; }
        public string raw { get; set; }
        public string Pattern { get; set; }
        public string Regex { get; set; }
        public bool flip { get; set; }
        public bool unbundle { get; set; }
        public EUncookExtension uext { get; set; }
        public List<ECookedFileFormat> forcebuffers { get; set; }
        public ulong Hash { get; set; }


        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() =>
                consoleFunctions.UncookTask(GetPathArgs(
                    Folders, Files), Outpath, raw, uext, flip, Hash, Pattern, Regex, unbundle, forcebuffers.ToArray()));
    }

}
