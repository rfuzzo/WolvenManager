using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DynamicData;
using Octokit;
using Splat;
using WolvenManager.App.Exceptions;

namespace WolvenManager.App.Services
{
    /// <summary>
    /// A service to manage GitHub repository subscriptions
    /// </summary>
    public class PluginService : IPluginService
    {
        #region fields

        private readonly INotificationService _notificationService;

        private readonly GitHubClient _client = new(new ProductHeaderValue("WolvenModManager"));

        private List<string> _shippedAddons = new();
        private List<string> _userAddons = new();

        private static string ShippedAddonsListPath
        {
            get
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;
                var filename = Path.GetFileNameWithoutExtension(path);
                var dir = Path.GetDirectoryName(path);
                return Path.Combine(dir ?? "", filename + "addons.json");
            }
        }

        private static string UserAddonsListPath
        {
            get
            {
                var path = AppDomain.CurrentDomain.BaseDirectory;
                var filename = Path.GetFileNameWithoutExtension(path);
                var dir = Path.GetDirectoryName(path);
                return Path.Combine(dir ?? "", filename + "addons_user.json");
            }
        }

        #endregion

        public PluginService()
        {
            _notificationService = Locator.Current.GetService<INotificationService>();


        }

        #region properties
        /// <summary>
        /// List of all addons
        /// </summary>
        public List<string> Addons => _shippedAddons.Concat(_userAddons).Distinct().ToList();

        #endregion


        #region methods

        /// <summary>
        /// Loads the available Github Repositories and updates the repos
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Init() 
        {
            // first fetch shipped addons
            // then fetch user addons
            _shippedAddons = await FetchRepositoryList(ShippedAddonsListPath);
            _userAddons = await FetchRepositoryList(UserAddonsListPath);

            //dbg
            if (_userAddons.Count < 1)
            {
                _userAddons.Add("https://github.com/jac3km4/redscript.git");
                await SerializeUserAddonList();
            }

            return true;
        }

        /// <summary>
        /// Loads a list of repositories from a saved file
        /// </summary>
        /// <returns></returns>
        private async Task<List<string>> FetchRepositoryList(string configfile)
        {
            if (!File.Exists(configfile))
            {
                return new List<string>();
            }

            await using var openStream = File.OpenRead(configfile);
            return await JsonSerializer.DeserializeAsync<List<string>>(openStream);
        }

        /// <summary>
        /// Saves the current list of repositories
        /// </summary>
        /// <returns></returns>
        private async Task SerializeUserAddonList()
        {
            await using var createStream = File.Create(UserAddonsListPath);
            await JsonSerializer.SerializeAsync(createStream, _userAddons);
        }

        /// <summary>
        /// Translates a https gitHub string to owner and repo Name for the API
        /// </summary>
        /// <param name="https"></param>
        /// <exception cref="InvalidGitHubHttpsException"></exception>
        /// <returns></returns>
        private (string, string) GetGitHubDataFromHttps(string https)
        {
            try
            {
                //e.g. https://github.com/octokit/octokit.net.git
                var splits = https.Split('/');
                var repoName = splits.Last()[..^4];
                var repoOwner = splits[^1];

                return (repoOwner, repoName);
            }
            catch (Exception e)
            {
                throw new InvalidGitHubHttpsException();
            }
        }

        /// <summary>
        /// Adds a repository to the repository list
        /// </summary>
        /// <param name="https"></param>
        /// <returns></returns>
        public bool AddRepository(string https)
        {
            _userAddons.Add(https);

            return true;
        }

        /// <summary>
        /// Checks a given repository https for updates
        /// </summary>
        /// <param name="https"></param>
        /// <exception cref="ServiceLocationException"></exception>
        /// <returns></returns>
        public async Task<bool> CheckForUpdatesAsync(string https)
        {
            var profile = Locator.Current.GetService<IProfileService>();
            if (profile == null)
            {
                throw new ServiceLocationException(nameof(IProfileService));
            }

            try
            {
                var (repoOwner, repoName) = GetGitHubDataFromHttps(https);
                var releases = await _client.Repository.Release.GetAll(repoOwner, repoName);
                if (releases.Count > 0)
                {
                    var latest = releases.First();

                    // check if newer
                    

                    // ask if install
                    

                    // save version

                    return true;
                }

                return false;
            }
            catch (InvalidGitHubHttpsException e)
            {
                return false;
            }
            catch (ApiException apiException)
            {
                var msg = apiException.Message;
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        #endregion

    }
}
