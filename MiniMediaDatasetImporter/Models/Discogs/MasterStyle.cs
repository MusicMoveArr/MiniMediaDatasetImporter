using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class MasterStyle
{
    [XmlText]
    public string Style { get; set; }
}