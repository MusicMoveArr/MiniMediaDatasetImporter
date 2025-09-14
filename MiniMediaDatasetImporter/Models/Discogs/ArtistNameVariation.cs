using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class ArtistNameVariation
{
    [XmlText]
    public string Name { get; set; }
}