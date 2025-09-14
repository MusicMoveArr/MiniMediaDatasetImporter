using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class ReleaseTrack
{
    [XmlElement("title")]
    public string? Title { get; set; }
    
    [XmlElement("position")]
    public string? Position { get; set; }
    
    [XmlElement("duration")]
    public string? Duration { get; set; }
}