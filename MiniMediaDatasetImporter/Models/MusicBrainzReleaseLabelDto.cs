namespace MiniMediaDatasetImporter.Models;

public class MusicBrainzReleaseLabelDto
{
    public Guid ReleaseId { get; set; }
    public Guid LabelId { get; set; }
    public string CatalogNumber { get; set; }
}