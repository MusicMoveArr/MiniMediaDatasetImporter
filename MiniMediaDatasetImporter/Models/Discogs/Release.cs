using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Models.Discogs;

[XmlRoot("release")]
public class Release
{
    [XmlAttribute("id")]
    public int Id { get; set; }
    
    [XmlAttribute("status")]
    public string? Status { get; set; }
    
    [XmlElement("title")]
    public string? Title { get; set; }
    
    [XmlElement("country")]
    public string? Country { get; set; }
    
    [XmlElement("released")]
    public string? Released { get; set; }
    
    [XmlElement("notes")]
    public string? Notes { get; set; }
    
    [XmlElement("data_quality")]
    public string? DataQuality { get; set; }
    
    [XmlElement("master_id")]
    public ReleaseMasterId? MasterId { get; set; }
    
    [XmlArray("tracklist")]
    [XmlArrayItem("track")]
    public List<ReleaseTrack> TrackList { get; set; }
    
    [XmlArray("genres")]
    [XmlArrayItem("genre")]
    public List<ReleaseGenre> Genres { get; set; }
    
    [XmlArray("styles")]
    [XmlArrayItem("style")]
    public List<ReleaseStyle> Styles { get; set; }
    
    [XmlArray("formats")]
    [XmlArrayItem("format")]
    public List<ReleaseFormat> Formats { get; set; }
    
    [XmlArray("artists")]
    [XmlArrayItem("artist")]
    public List<ReleaseArtist> Artists { get; set; }
    
    [XmlArray("labels")]
    [XmlArrayItem("label")]
    public List<ReleaseLabel> Labels { get; set; }
    
    [XmlArray("extraartists")]
    [XmlArrayItem("artist")]
    public List<ReleaseExtraArtist> ExtraArtists { get; set; }
    
    [XmlArray("identifiers")]
    [XmlArrayItem("identifier")]
    public List<ReleaseIdentifier> Identifiers { get; set; }
    
    [XmlArray("videos")]
    [XmlArrayItem("video")]
    public List<ReleaseVideo> Videos { get; set; }
    
    [XmlArray("companies")]
    [XmlArrayItem("company")]
    public List<ReleaseCompany> Companies { get; set; }
}