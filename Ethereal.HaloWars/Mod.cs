namespace Ethereal.HaloWars
{
    public class Mod
    {
        #region Properties
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string HWModPath { get; set; } = string.Empty;
        public string CustomVideoPath { get; set; } = string.Empty;
        public string CustomSavePath { get; set; } = string.Empty;
        #endregion
    }

}
