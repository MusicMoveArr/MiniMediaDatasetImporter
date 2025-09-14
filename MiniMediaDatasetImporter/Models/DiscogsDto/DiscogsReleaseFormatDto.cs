namespace MiniMediaDatasetImporter.Models.DiscogsDto;

public class DiscogsReleaseFormatDto
{
    public Guid ReleaseFormatUuId { get; set; } = Guid.NewGuid();
    public int ReleaseId { get; set; }
    public string? Name { get; set; }
    public int Quantity { get; set; }
    public string? Text { get; set; }
}