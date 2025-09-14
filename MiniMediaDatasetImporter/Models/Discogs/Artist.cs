using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

[XmlRoot("artist")]
public class Artist
{
    [XmlElement("id")]
    public int Id { get; set; }
    
    [XmlElement("name")]
    public string? Name { get; set; }
    
    [XmlElement("realname")]
    public string? RealName { get; set; }
    
    [XmlElement("profile")]
    public string? Profile { get; set; }
    
    [XmlElement("data_quality")]
    public string? DataQuality { get; set; }
    
    
    [XmlArray("urls")]
    [XmlArrayItem("url")]
    public List<ArtistUrl> Urls { get; set; }
    
    [XmlArray("namevariations")]
    [XmlArrayItem("name")]
    public List<ArtistNameVariation> NameVariations { get; set; }
    
    [XmlArray("aliases")]
    [XmlArrayItem("name")]
    public List<ArtistAlias> Aliases { get; set; }
    
    //forgot to import
    [XmlArray("members")]
    public List<ArtistMember> Members { get; set; }
}