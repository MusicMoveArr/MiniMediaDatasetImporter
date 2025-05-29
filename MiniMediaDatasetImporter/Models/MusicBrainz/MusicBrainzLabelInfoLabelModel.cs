using System.Text.Json.Serialization;
using MiniMediaDatasetImporter.Repositories.MusicBrainz;

namespace MiniMediaDatasetImporter.Models.MusicBrainz;

public class MusicBrainzLabelInfoLabelModel
{
    public string Name { get; set; }
    public string? Disambiguation { get; set; }
    public string Id { get; set; }
    
    public string? Type { get; set; }
    public string? Country { get; set; }
    
    [JsonPropertyName("sort-name")]
    public string? SortName { get; set; }
    
    [JsonPropertyName("type-id")]
    public string? TypeId { get; set; }
    
    [JsonPropertyName("label-code")]
    public int? LabelCode { get; set; }
    
    [JsonPropertyName("life-span")]
    public MusicBrainzLabelInfoLabelLifeSpanModel? LifeSpan { get; set; }
    
    public MusicBrainzLabelInfoLabelAreaModel? Area { get; set; }
}