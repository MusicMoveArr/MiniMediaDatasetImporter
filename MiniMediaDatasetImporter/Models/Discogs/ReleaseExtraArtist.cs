using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class ReleaseExtraArtist
{
    [XmlElement("id")]
    public int Id { get; set; }
    
    [XmlElement("name")]
    public string? Name { get; set; }
    
    [XmlElement("anv")]
    public string? Anv { get; set; }
    
    [XmlElement("role")]
    public string? Role { get; set; }
}