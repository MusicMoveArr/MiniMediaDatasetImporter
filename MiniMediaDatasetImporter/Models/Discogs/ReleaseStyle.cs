using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class ReleaseStyle
{
    [XmlText]
    public string Style { get; set; }
}