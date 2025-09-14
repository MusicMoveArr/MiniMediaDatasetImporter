using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

[XmlRoot("master")]
public class Master
{
    [XmlAttribute("id")]
    public string Id { get; set; }
    
    [XmlElement("main_release")]
    public int MainRelease { get; set; }
    
    [XmlArray("artists")]
    [XmlArrayItem("artist")]
    public List<MasterArtist> Artists { get; set; }
    
    [XmlArray("genres")]
    [XmlArrayItem("genre")]
    public List<MasterGenre> Genres { get; set; }
    
    [XmlArray("styles")]
    [XmlArrayItem("style")]
    public List<MasterStyle> Styles { get; set; }
    
    [XmlElement("year")]
    public int Year { get; set; }
    
    [XmlElement("title")]
    public string Title { get; set; }
    
    [XmlElement("data_quality")]
    public string DataQuality { get; set; }
    
    [XmlArray("videos")]
    [XmlArrayItem("video")]
    public List<MasterVideo> Videos { get; set; }
}