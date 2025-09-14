namespace MiniMediaDatasetImporter.Models.DiscogsDto;

public class DiscogsReleaseDto
{
    public int ReleaseId { get; set; }
    public string Status { get; set; }
    public string Title { get; set; }
    public string Country { get; set; }
    public string Released { get; set; }
    public string Notes { get; set; }
    public string DataQuality { get; set; }
    public bool IsMainRelease { get; set; }
    public int MasterId { get; set; }
}