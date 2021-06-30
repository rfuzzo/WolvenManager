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

        public const string AssemblyName = "WolvenManager.UI.dll";

        public const string GithubRepo = "https://github.com/rfuzzo/WolvenManager";
        public const string RemoteManifest = GithubRepo + "/releases/latest/download/manifest.json";
    }
}
