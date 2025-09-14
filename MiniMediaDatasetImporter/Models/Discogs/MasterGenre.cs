using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class MasterGenre
{
    [XmlText]
    public string Genre { get; set; }
}