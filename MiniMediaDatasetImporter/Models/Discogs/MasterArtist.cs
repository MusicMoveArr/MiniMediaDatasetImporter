using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class MasterArtist
{
    [XmlElement("id")]
    public int Id { get; set; }
    
    [XmlElement("name")]
    public string Name { get; set; }
    
    [XmlElement("join")]
    public string Join { get; set; }
}