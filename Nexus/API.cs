using Newtonsoft.Json;

namespace Nexus
{
    public class API
    {
        private static readonly HttpClient client = new();
        public static int ApiCallCount { get; private set; } = 0;

        private static void IncrementApiCallCount()
        {
            ApiCallCount++;
        }

        public static async Task<string> GetTrendingModsJson(string apiKey, string gameDomain, int limit)
        {
            return await GetApiResponse(apiKey, $"https://api.nexusmods.com/v1/games/{gameDomain}/mods/trending.json?limit={limit}");
        }

        public static async Task<string> GetModFilesJson(string apiKey, string gameDomain, string modId)
        {
            return await GetApiResponse(apiKey, $"https://api.nexusmods.com/v1/games/{gameDomain}/mods/{modId}/files.json");
        }

        public static async Task<string> GetDownloadLink(string apiKey, string gameDomain, string modId, string fileId)
        {
            return await GetApiResponse(apiKey, $"https://api.nexusmods.com/v1/games/{gameDomain}/mods/{modId}/files/{fileId}/download_link.json");
        }

        public static async Task<string> GetMainFileDownloadLink(string apiKey, string gameDomain, string modId)
        {
            string modFilesJson = await GetModFilesJson(apiKey, gameDomain, modId);
            if (string.IsNullOrEmpty(modFilesJson))
            {
                return string.Empty;
            }

            ModFilesResponse? modFiles = JsonConvert.DeserializeObject<ModFilesResponse>(modFilesJson);
            ModFile? mainFile = modFiles?.Files?.Find(file => file.IsPrimary) ??
                                modFiles?.Files?.Find(file => file.CategoryName?.Equals("MAIN", StringComparison.OrdinalIgnoreCase) == true);

            if (mainFile == null)
            {
                Console.WriteLine("Main file not found");
                return string.Empty;
            }

            return await GetDownloadLink(apiKey, gameDomain, modId, mainFile.FileId.ToString());
        }

        private static async Task<string> GetApiResponse(string apiKey, string url)
        {
            SetApiKeyHeader(apiKey);

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                IncrementApiCallCount();
                _ = response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return string.Empty;
            }
        }

        private static void SetApiKeyHeader(string apiKey)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("apikey", apiKey);
        }

    }

    public class Mod
    {
        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("summary")]
        public required string Summary { get; set; }

        [JsonProperty("description")]
        public required string Description { get; set; }

        [JsonProperty("picture_url")]
        public required string PictureUrl { get; set; }

        [JsonProperty("mod_downloads")]
        public int ModDownloads { get; set; }

        [JsonProperty("mod_unique_downloads")]
        public int ModUniqueDownloads { get; set; }

        [JsonProperty("uid")]
        public long Uid { get; set; }

        [JsonProperty("mod_id")]
        public int ModId { get; set; }

        [JsonProperty("game_id")]
        public int GameId { get; set; }

        [JsonProperty("allow_rating")]
        public bool AllowRating { get; set; }

        [JsonProperty("domain_name")]
        public required string DomainName { get; set; }

        [JsonProperty("category_id")]
        public int CategoryId { get; set; }

        [JsonProperty("version")]
        public required string Version { get; set; }

        [JsonProperty("endorsement_count")]
        public int EndorsementCount { get; set; }

        [JsonProperty("created_timestamp")]
        public long CreatedTimestamp { get; set; }

        [JsonProperty("created_time")]
        public DateTime CreatedTime { get; set; }

        [JsonProperty("updated_timestamp")]
        public long UpdatedTimestamp { get; set; }

        [JsonProperty("updated_time")]
        public DateTime UpdatedTime { get; set; }

        [JsonProperty("author")]
        public required string Author { get; set; }

        [JsonProperty("uploaded_by")]
        public required string UploadedBy { get; set; }

        [JsonProperty("uploaded_users_profile_url")]
        public required string UploadedUsersProfileUrl { get; set; }

        [JsonProperty("contains_adult_content")]
        public bool ContainsAdultContent { get; set; }

        [JsonProperty("status")]
        public required string Status { get; set; }

        [JsonProperty("available")]
        public bool Available { get; set; }

        [JsonProperty("user")]
        public required User User { get; set; }

        [JsonProperty("endorsement")]
        public object? Endorsement { get; set; } = null;

        public Version ParsedVersion => ParseVersion(Version);

        private static Version ParseVersion(string versionString)
        {
            if (System.Version.TryParse(versionString, out Version? version))
            {
                return version;
            }
            return new Version(1, 0); // Default version if parsing fails
        }
    }

    public class User
    {
        [JsonProperty("member_id")]
        public int MemberId { get; set; }

        [JsonProperty("member_group_id")]
        public int MemberGroupId { get; set; }

        [JsonProperty("name")]
        public required string Name { get; set; }
    }


    public class ModFile
    {
        [JsonProperty("id")]
        public required List<int> Id { get; set; }

        [JsonProperty("uid")]
        public long Uid { get; set; }

        [JsonProperty("file_id")]
        public int FileId { get; set; }

        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("version")]
        public required string Version { get; set; }

        [JsonProperty("category_id")]
        public int CategoryId { get; set; }

        [JsonProperty("category_name")]
        public required string CategoryName { get; set; }

        [JsonProperty("is_primary")]
        public bool IsPrimary { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("file_name")]
        public required string FileName { get; set; }

        [JsonProperty("uploaded_timestamp")]
        public long UploadedTimestamp { get; set; }

        [JsonProperty("uploaded_time")]
        public DateTime UploadedTime { get; set; }

        [JsonProperty("mod_version")]
        public required string ModVersion { get; set; }

        [JsonProperty("external_virus_scan_url")]
        public required string ExternalVirusScanUrl { get; set; }

        [JsonProperty("description")]
        public required string Description { get; set; }

        [JsonProperty("size_kb")]
        public int SizeKb { get; set; }

        [JsonProperty("size_in_bytes")]
        public double? SizeInBytes { get; set; }

        [JsonProperty("changelog_html")]
        public required string ChangelogHtml { get; set; }

        [JsonProperty("content_preview_link")]
        public required string ContentPreviewLink { get; set; }
    }

    public class FileUpdate
    {
        [JsonProperty("old_file_id")]
        public int OldFileId { get; set; }

        [JsonProperty("new_file_id")]
        public int NewFileId { get; set; }

        [JsonProperty("old_file_name")]
        public required string OldFileName { get; set; }

        [JsonProperty("new_file_name")]
        public required string NewFileName { get; set; }

        [JsonProperty("uploaded_timestamp")]
        public long UploadedTimestamp { get; set; }

        [JsonProperty("uploaded_time")]
        public DateTime UploadedTime { get; set; }
    }

    public class ModFilesResponse
    {
        [JsonProperty("files")]
        public List<ModFile> Files { get; set; } = [];

        [JsonProperty("file_updates")]
        public List<FileUpdate> FileUpdates { get; set; } = [];
    }

    public class DownloadLink
    {
        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("short_name")]
        public required string ShortName { get; set; }

        [JsonProperty(nameof(URI))]
        public required string URI { get; set; }
    }
}