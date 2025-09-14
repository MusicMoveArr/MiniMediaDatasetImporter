namespace MiniMediaDatasetImporter.Models.DiscogsDto;

public class DiscogsReleaseCompanyDto
{
    public int ReleaseId { get; set; }
    public int CompanyId { get; set; }
    public string Name { get; set; }
    public string EntityType { get; set; }
    public string EntityTypeName { get; set; }
    public string ResourceUrl { get; set; }
}