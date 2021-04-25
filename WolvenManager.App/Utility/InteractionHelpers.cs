using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using WolvenManager.App.ViewModels;

namespace WolvenManager.App.Utility
{
    public static class InteractionHelpers
    {
        public static readonly Interaction<IEnumerable<string>, bool> ModViewModelInteraction = new();

    }
}
