using System.Xml.Serialization;

namespace Data
{
    public class HWMod
    {
        [XmlAttribute("ManifestVersion")]
        public int ManifestVersion { get; set; }

        [XmlAttribute("ModID")]
        public required string ModID { get; set; }

        public required RequiredData RequiredData { get; set; }
        public OptionalData OptionalData { get; set; }
    }

    public class RequiredData
    {
        [XmlAttribute("Title")]
        public required string Title { get; set; }

        [XmlAttribute("Author")]
        public required string Author { get; set; }

        [XmlAttribute("Version")]
        public required string Version { get; set; }
    }

    public class OptionalData
    {
        public string BannerArt { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
    }
}
