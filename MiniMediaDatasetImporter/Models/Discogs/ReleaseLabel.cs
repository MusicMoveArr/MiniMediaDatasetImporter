using System.Xml.Serialization;
    

namespace MiniMediaDatasetImporter.Models.Discogs;

public class ReleaseLabel
{
    [XmlAttribute("id")]
    public int Id { get; set; }
    
    [XmlAttribute("name")]
    public string? Name { get; set; }
    
    [XmlAttribute("catno")]
    public string? Catno { get; set; }
}