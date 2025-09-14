using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class ReleaseGenre
{
    [XmlText]
    public string Genre { get; set; }
}