using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class ReleaseMasterId
{
    [XmlAttribute("is_main_release")]
    public bool IsMainRelease { get; set; }
    
    [XmlText]
    public int Id { get; set; }
}