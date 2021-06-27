using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolvenManager.App.Utility
{
    public static class CommonHelpers
    {
        public static bool IsMainFolder(string path) =>
            Directory.Exists(path) && new DirectoryInfo(path).Name.Equals("Cyberpunk 2077");

    }
}
