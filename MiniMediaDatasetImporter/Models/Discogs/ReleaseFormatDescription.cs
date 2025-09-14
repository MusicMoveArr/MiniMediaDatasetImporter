using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class ReleaseFormatDescription
{
    [XmlText]
    public string Description { get; set; }
}