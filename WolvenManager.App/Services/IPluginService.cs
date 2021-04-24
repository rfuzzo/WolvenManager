using System.Threading.Tasks;
using Octokit;

namespace WolvenManager.App.Services
{
    public interface IPluginService
    {
        public Task<bool> Init();


        public bool AddRepository(string https);

        public Task<bool> CheckForUpdatesAsync(string https);

    }
}
