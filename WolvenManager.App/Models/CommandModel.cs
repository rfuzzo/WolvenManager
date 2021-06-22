using System;
using System.Collections.Generic;
using System.CommandLine;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using CP77Tools.Tasks;
using Microsoft.Extensions.Options;
using ReactiveUI;
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


        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions)
        {
            var folders = Folders.Split(';');
            var archives = Archives.Split(';');
            var Path = folders.Concat(archives).ToArray();

            await Task.Run(() => consoleFunctions.ArchiveTask(Path, Pattern, Regex, Diff, List));
        }
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


        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions)
        {
            var folders = Folders.Split(';');
            var archives = Archives.Split(';');
            var Path = folders.Concat(archives).Select(_ => _.TrimStart('\"').TrimEnd('\"')).ToArray();

            await Task.Run(() =>
                consoleFunctions.UnbundleTask(Path.ToArray(), Outpath, Pattern, Regex, Hash, DEBUG_decompress));
        }
    }
}
