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
        public IEnumerable<ModFileModel> Input { get; set; }
        public IEnumerable<ModFileModel> Output { get; set; }

        public ZipModifyArgs(IEnumerable<ModFileModel> input, IEnumerable<ModFileModel> output)
        {
            this.Input = input;
            this.Output = output;
        }
    }
}
