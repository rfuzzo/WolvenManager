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

        public const string AssemblyName = "WolvenManager.UI.dll";

        public const string UpdateUrl = "https://github.com/rfuzzo/WolvenManager/releases/latest/download/";

    }
}
