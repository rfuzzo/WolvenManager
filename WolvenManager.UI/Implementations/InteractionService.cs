using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ReactiveUI;
using WolvenManager.App.Services;
using WolvenManager.App.ViewModels;

namespace WolvenManager.UI.Implementations
{
    public class InteractionService : IInteractionService
    {
        public List<string> BrowseFiles(string filters = "*", string title = "Select files", bool multiselect = true)
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = multiselect,
                Title = title,
                Filter = filters
            };
            return dialog.ShowDialog() != DialogResult.OK 
                ? new List<string>() 
                : dialog.FileNames.ToList();
        }


    }
}
