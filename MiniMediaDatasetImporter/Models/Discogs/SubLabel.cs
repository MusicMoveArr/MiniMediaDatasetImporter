using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class SubLabel
{
    [XmlAttribute("id")]
    public int Id { get; set; }
    
    [XmlText]
    public string Name { get; set; }
}