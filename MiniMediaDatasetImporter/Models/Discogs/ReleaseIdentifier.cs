using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class ReleaseIdentifier
{
    [XmlAttribute("description")]
    public string? Description { get; set; }
    
    [XmlAttribute("type")]
    public string? Type { get; set; }
    
    [XmlAttribute("value")]
    public string? Value { get; set; }
}