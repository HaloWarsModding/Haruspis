using System.Windows.Controls;

using Newtonsoft.Json;

using Nexus;

using UI.Pages.Workshop.UserControls;

namespace UI.Pages.Workshop
{
    public partial class WorkshopPage : Page
    {
        public WorkshopPage()
        {
            InitializeComponent();
            CallAPI();
        }

        public async void CallAPI()
        {
            string apiKey = ApiKeyProvider.GetApiKey();
            string gameDomain = "halowarsdefinitiveedition";
            int limit = 10;

            string latestModsJson = await API.GetTrendingModsJson(apiKey, gameDomain, limit);

            // Deserialize to get mod IDs
            List<Nexus.Mod> latestMods = JsonConvert.DeserializeObject<List<Nexus.Mod>>(latestModsJson);

            // Fetch download link for the main file of each mod
            foreach (Nexus.Mod mod in latestMods)
            {
                if (string.IsNullOrEmpty(mod.Name) || mod.ModId <= 0)
                {
                    Console.WriteLine($"Skipping mod with missing details: Name='{mod.Name}', ID='{mod.ModId}'");
                    continue;
                }

                Console.WriteLine($"Processing Mod: {mod.Name}, ID: {mod.ModId}");
                string downloadLinkJson = await API.GetMainFileDownloadLink(apiKey, gameDomain, mod.ModId.ToString());
                if (!string.IsNullOrEmpty(downloadLinkJson))
                {
                    List<DownloadLink> downloadLinks = JsonConvert.DeserializeObject<List<DownloadLink>>(downloadLinkJson);
                    foreach (DownloadLink link in downloadLinks)
                    {
                        Console.WriteLine($"Mod: {mod.Name}, Version: {mod.ParsedVersion}, Download Link: {link.URI} ({link.Name})"); ;
                    }

                    NexusMod UMod = new(mod, downloadLinks[0]);

                    _ = FetchList.Items.Add(UMod);
                }
                else
                {
                    Console.WriteLine($"Failed to get download link for Mod: {mod.Name}, ID: {mod.ModId}");
                }
            }

            Console.WriteLine($"Total API calls made: {API.ApiCallCount}");
        }
    }
}
