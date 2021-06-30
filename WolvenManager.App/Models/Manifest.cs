using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolvenManager.App.Models
{
    public class Manifest
    {
        public string Version { get; set; }
        public string AssemblyName { get; set; }
        public Dictionary<string, string> FileHashes { get; set; }
    }
}
