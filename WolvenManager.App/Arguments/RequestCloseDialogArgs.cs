using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolvenManager.App.Arguments
{
    //https://stackoverflow.com/q/3801681
    public class RequestCloseDialogEventArgs : EventArgs
    {
        public bool DialogResult { get; set; }
        public RequestCloseDialogEventArgs(bool dialogresult)
        {
            this.DialogResult = dialogresult;
        }
    }
}
