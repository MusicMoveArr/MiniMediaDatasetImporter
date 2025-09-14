using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class ReleaseVideo
{
    [XmlAttribute("duration")]
    public string? Duration { get; set; }
    
    [XmlAttribute("embed")]
    public bool Embed { get; set; }
    
    [XmlAttribute("src")]
    public string? Source { get; set; }
    
    [XmlElement("title")]
    public string? Title { get; set; }
    
    [XmlElement("description")]
    public string? Description { get; set; }
}