namespace Discord
{
    public class RichPresenceData
    {
        public string Details { get; set; }
        public string State { get; set; }
        public string LargeImageKey { get; set; }
        public string LargeImageText { get; set; }
        public List<RichPresenceButton> Buttons { get; set; } = [];
    }
}
