using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace WolvenManager.Installer
{
    class Program
    {
        static int Main(string[] args)
        {




            var rootCommand = new RootCommand
            {
                new Option<FileInfo>(
                    "--file-option",
                    "An option whose argument is parsed as a FileInfo")
            };

            rootCommand.Description = "My sample app";

            rootCommand.Handler = CommandHandler.Create<FileInfo>((fileOption) =>
            {
               
                Console.WriteLine($"The value for --file-option is: {fileOption?.FullName ?? "null"}");
            });

            return rootCommand.InvokeAsync(args).Result;
        }
    }
}
