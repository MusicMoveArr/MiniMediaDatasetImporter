using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class ReleaseCompany
{
    [XmlElement("id")]
    public int Id { get; set; }
    
    [XmlElement("name")]
    public string? Name { get; set; }
    
    [XmlElement("entity_type")]
    public string? EntityType { get; set; }
    
    [XmlElement("entity_type_name")]
    public string? EntityTypeName { get; set; }
    
    [XmlElement("resource_url")]
    public string? ResourceUrl { get; set; }
}