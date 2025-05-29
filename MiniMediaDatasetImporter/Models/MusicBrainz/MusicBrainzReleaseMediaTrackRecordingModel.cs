using System.Text.Json.Serialization;
using MiniMediaDatasetImporter.Models.MusicBrainz.MusicBrainzRecordings;

namespace MiniMediaDatasetImporter.Models.MusicBrainz;

public class MusicBrainzReleaseMediaTrackRecordingModel
{
    public string? Title { get; set; }
    public int? Length { get; set; }
    
    [JsonPropertyName("first-release-date")]
    public string? FirstReleaseDate { get; set; }
    public bool Video { get; set; }
    public string? Id { get; set; }
    
    [JsonPropertyName("artist-credit")]
    public List<MusicBrainzArtistCreditModel> ArtistCredit { get; set; } = new List<MusicBrainzArtistCreditModel>();
}