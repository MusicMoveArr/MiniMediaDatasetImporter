using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

[XmlRoot("label")]
public class Label
{
    [XmlElement("id")]
    public int Id { get; set; }
    
    [XmlElement("name")]
    public string? Name { get; set; }
    
    [XmlElement("data_quality")]
    public string? DataQuality { get; set; }
    
    [XmlElement("contactinfo")]
    public string? ContactInfo { get; set; }
    
    [XmlElement("profile")]
    public string? Profile { get; set; }
    
    [XmlArray("urls")]
    [XmlArrayItem("url")]
    public List<ArtistUrl> Urls { get; set; }
    
    [XmlArray("sublabels")]
    [XmlArrayItem("label")]
    public List<SubLabel> SubLabels { get; set; }
}