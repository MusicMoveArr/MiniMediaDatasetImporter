namespace MiniMediaDatasetImporter.Models.MusicBrainzDto;

public class MusicBrainzArtistDto
{
    public Guid ArtistId { get; set; }
    public int ReleaseCount { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Country { get; set; }
    public string SortName { get; set; }
    public string Disambiguation { get; set; }
    public DateTime LastSyncTime { get; set; }
}