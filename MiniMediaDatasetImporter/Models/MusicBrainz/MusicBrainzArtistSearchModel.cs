using System.Text.Json.Serialization;

namespace MiniMediaDatasetImporter.Models.MusicBrainz;

public class MusicBrainzArtistSearchModel
{
    [JsonPropertyName("artists")]
    public List<MusicBrainzArtistInfoModel> Artists { get; set; }
}