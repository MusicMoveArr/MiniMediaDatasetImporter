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
    
    public async Task<List<Guid>> GetArtistIdsToInsertAsync(List<Guid> artistIds)
    {
        string query = @"SELECT ArtistId
                         FROM musicbrainz_artist
                         WHERE ArtistId = ANY(@artistIds)";
        
        await using var conn = new NpgsqlConnection(_connectionString);

        var existingArtistIds = await conn
            .QueryAsync<Guid>(query, new
            {
                artistIds
            });

        return artistIds
            .Except(existingArtistIds)
            .ToList();
    }
}