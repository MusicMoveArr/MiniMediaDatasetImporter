using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class ReleaseFormat
{
    [XmlAttribute("name")]
    public string? Name { get; set; }
    
    [XmlAttribute("qty")]
    public int Quantity { get; set; }
    
    [XmlAttribute("text")]
    public string? Text { get; set; }
    
    [XmlArray("descriptions")]
    [XmlArrayItem("description")]
    public List<ReleaseFormatDescription> Descriptions { get; set; }
}