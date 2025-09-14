using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

public class ArtistUrl
{
    [XmlText]
    public string Url { get; set; }
}