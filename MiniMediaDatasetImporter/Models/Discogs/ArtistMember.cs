using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class ArtistMember
{
    
    [XmlElement("id")]
    public int Id { get; set; }
    
    [XmlElement("name")]
    public string Name { get; set; }
}