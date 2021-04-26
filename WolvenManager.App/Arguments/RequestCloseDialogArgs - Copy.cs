using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolvenManager.App.Models;

namespace WolvenManager.App.Arguments
{
    public class ZipModifyArgs : EventArgs
    {
        public Dictionary<string, string> Output { get; set; }

        public ZipModifyArgs(Dictionary<string, string> output)
        {
            this.Output = output;
        }
    }
}
