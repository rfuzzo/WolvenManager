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

namespace WolvenManager.Models
{
    public abstract class CommandModel
    {
        [Browsable(false)]
        public abstract string Name { get; }

        public abstract Task ExecuteAsync(IConsoleFunctions consoleFunctions);

        public override string ToString() => Name;
    }


    public class ArchiveCommandModel : CommandModel
    {
        public override string Name => "archive";

        public string[] Path { get; set; }
        public string Pattern { get; set; }
        public string Regex { get; set; }
        public bool List { get; set; }
        public bool Diff { get; set; }


        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() => consoleFunctions.ArchiveTask(Path, Pattern, Regex, Diff, List));
    }

    public class UnbundleCommandModel : CommandModel
    {
        public override string Name => "unbundle";

        public string[] Path { get; set; }
        public string Outpath { get; set; }
        public string Pattern { get; set; }
        public string Regex { get; set; }
        public string Hash { get; set; }
        public bool DEBUG_decompress { get; set; }


        public override async Task ExecuteAsync(IConsoleFunctions consoleFunctions) =>
            await Task.Run(() => consoleFunctions.UnbundleTask(Path, Outpath, Pattern, Regex, Hash, DEBUG_decompress));
    }
}
