using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WolvenManager.App.Attributes;

namespace WolvenManager.App.ViewModels.PageViewModels
{
    [RoutingUrl(Constants.RoutingIDs.Mods)]
    public class ModListViewModel : PageViewModel
    {
        public ModListViewModel() : base(typeof(ModListViewModel))
        {
           

            //InstallModCommand = ReactiveCommand.CreateFromTask(InstallMod);
            //ToggleSidebarCommand = ReactiveCommand.Create(() =>
            //{
            //    IsSideBarVisible = !IsSideBarVisible;
            //});



            // load all archives
            var basedir = Environment.CurrentDirectory;
            

            // check some stuff
            var modDir = Path.Combine(basedir, "archive", "pc", "mod");
            if (Directory.Exists(modDir))
            {
                var archives = Directory.GetFiles(modDir, "*.archive", SearchOption.AllDirectories);
                foreach (var archivePath in archives)
                {
                    BindingData.Add(new ArchiveViewModel(archivePath));
                }




            }
        }

        


        #region properties
        
        [Reactive]
        public ArchiveViewModel SelectedModViewModel { get; set; }

        [Reactive] public ObservableCollection<object> SelectedModViewModels { get; set; } = new();



        [Reactive]
        public ObservableCollection<ArchiveViewModel> BindingData { get; set; } = new();

        public ReactiveCommand<Unit, Unit> InstallModCommand { get; }


        #endregion

    }
}
