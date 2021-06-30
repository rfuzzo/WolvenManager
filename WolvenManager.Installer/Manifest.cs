using System.Collections.Generic;

namespace WolvenManager.Installer
{
    public class Manifest
    {
        public string Version { get; set; }
        public string AssemblyName { get; set; }
        public Dictionary<string, string> FileHashes { get; set; }
    }
}
