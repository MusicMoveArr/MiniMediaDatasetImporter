namespace MiniMediaDatasetImporter.Models.DiscogsDto;

public class DiscogsMasterDto
{
    public int Id { get; set; }
    public int MainReleaseId { get; set; }
    public int Year { get; set; }
    public string Title { get; set; }
    public string DataQuality { get; set; }
    public DateTime LastSyncTime { get; set; }
}