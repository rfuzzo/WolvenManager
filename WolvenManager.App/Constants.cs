using System;
using System.IO;

namespace WolvenManager.App
{
    public static class Constants
    {
        public enum RoutingIDs
        {
            Mods,
            Modkit, 
            Mod,
            Search, //disabled
            Settings
        }

        public const string AppDataFolder = "REDModding";



        public const string ProductName = "WolvenManager";
        public const string AssemblyName = ProductName + ".UI.dll";
        public const string AppProductName = ProductName + ".UI";
        public const string AppName = ProductName + ".UI.exe";
        public const string UpdaterName = "WolvenManager.Installer.exe";

        public const string UpdateUrl = "https://github.com/rfuzzo/WolvenManager/releases/latest/download/";

    }
}
