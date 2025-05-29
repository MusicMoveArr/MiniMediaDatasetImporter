namespace MiniMediaDatasetImporter.Models;

public class MusicBrainzReleaseTrackArtistDto
{
    public Guid ReleaseTrackId { get; set; }
    public Guid ArtistId { get; set; }
    public string JoinPhrase { get; set; }
    public int Index { get; set; }
}