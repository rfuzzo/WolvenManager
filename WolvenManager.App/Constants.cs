using System;
using System.IO;

namespace WolvenManager.App
{
    public static class Constants
    {
        public enum RoutingIDs
        {
            Main,
            //Library,
            //Extensions,
            Settings
        }

        public static string ConfigurationPath
        {
            get
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;
                var filename = Path.GetFileNameWithoutExtension(path);
                var dir = Path.GetDirectoryName(path);
                return Path.Combine(dir ?? "", filename + "config.json");
            }
        }

        public static string LibraryPath
        {
            get
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;
                var filename = Path.GetFileNameWithoutExtension(path);
                var dir = Path.GetDirectoryName(path);
                return Path.Combine(dir ?? "", filename + "lib.bin");
            }
        }

        public static string WorkingDir => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WorkingDir");

        
    }
}
