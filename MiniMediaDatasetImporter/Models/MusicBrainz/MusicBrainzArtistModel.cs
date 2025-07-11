using System.Text.Json.Serialization;
using MiniMediaDatasetImporter.Models.MusicBrainz.MusicBrainzRecordings;

namespace MiniMediaDatasetImporter.Models.MusicBrainz;

public class MusicBrainzArtistModel
{
    [JsonPropertyName("release-count")]
    public int? ReleaseCount { get; set; }
    
    [JsonPropertyName("artist-credit")]
    public List<MusicBrainzArtistCreditModel> ArtistCredit { get; set; } = new List<MusicBrainzArtistCreditModel>();
    
    public List<MusicBrainzArtistReleaseModel> Releases { get; set; } = new List<MusicBrainzArtistReleaseModel>();
    
    [JsonPropertyName("isrcs")]
    public List<string> ISRCS { get; set; } = new List<string>();
}