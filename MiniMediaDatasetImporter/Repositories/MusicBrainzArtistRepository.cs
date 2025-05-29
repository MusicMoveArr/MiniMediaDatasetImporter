using Dapper;
using Npgsql;

namespace MiniMediaDatasetImporter.Repositories;

public class MusicBrainzArtistRepository
{
    private readonly string _connectionString;
    public MusicBrainzArtistRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task UpsertArtistAsync(
        Guid artistId, 
        string artistName, 
        string? artistType,
        string? country,
        string? sortName,
        string? disambiguation)
    {
        if (string.IsNullOrWhiteSpace(artistType))
        {
            artistType = string.Empty;
        }
        if (string.IsNullOrWhiteSpace(country))
        {
            country = string.Empty;
        }
        if (string.IsNullOrWhiteSpace(sortName))
        {
            sortName = string.Empty;
        }
        if (string.IsNullOrWhiteSpace(disambiguation))
        {
            disambiguation = string.Empty;
        }
        
        string query = @"INSERT INTO MusicBrainz_Artist (ArtistId, 
                               Name, Type, Country, SortName, Disambiguation,  LastSyncTime)
                         VALUES (@artistId, @name, @type, @Country, @SortName, @Disambiguation, @lastSyncTime)
                         ON CONFLICT (ArtistId) 
                         DO UPDATE SET 
                             Name = EXCLUDED.Name, 
                             Type = EXCLUDED.Type, 
                             Country = EXCLUDED.Country, 
                             SortName = EXCLUDED.SortName, 
                             Disambiguation = EXCLUDED.Disambiguation";
        
        await using var conn = new NpgsqlConnection(_connectionString);

        await conn.ExecuteAsync(query, new
        {
            artistId,
            name = artistName,
            type = artistType,
            Country = country,
            SortName = sortName,
            Disambiguation = disambiguation,
            lastSyncTime = new DateTime(2000, 1, 1)
        });
    }
    
    public async Task<bool> ArtistExistsByIdAsync(Guid artistId)
    {
        string query = @"SELECT 1
                         FROM MusicBrainz_Artist artist
                         where artist.ArtistId = @artistId
                         limit 1";

        await using var conn = new NpgsqlConnection(_connectionString);

        return (await conn
            .ExecuteScalarAsync<int?>(query,
                param: new
                {
                    artistId
                })) == 1;
    }
}