namespace MiniMediaDatasetImporter.Models;

public class MusicBrainzReleaseDto
{
    public Guid ArtistId { get; set; }
    public Guid ReleaseId { get; set; }
    public string Title { get; set; }
    public string Status { get; set; }
    public string StatusId { get; set; }
    public string Date { get; set; }
    public string Barcode { get; set; }
    public string Country { get; set; }
    public string Disambiguation { get; set; }
    public string Quality { get; set; }
}