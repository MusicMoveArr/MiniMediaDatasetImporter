using CliFx.Infrastructure;
using DapperBulkQueries.Common;
using DapperBulkQueries.Npgsql;
using MiniMediaDatasetImporter.Helpers;
using MiniMediaDatasetImporter.Models;
using MiniMediaDatasetImporter.Models.MusicBrainz;
using MiniMediaDatasetImporter.Models.MusicBrainzDto;
using MiniMediaDatasetImporter.Repositories;
using Npgsql;
using Spectre.Console;

namespace MiniMediaDatasetImporter.Commands;

public class ImportMusicBrainzCommandHandler
{
    private readonly MusicBrainzArtistRepository _artistRepository;
    private readonly MusicBrainzLabelRepository _labelRepository;
    private readonly string _connectionString;
    
    
    private readonly List<string> _trackColumns = new()
    {
        "ReleaseTrackId", 
        "RecordingTrackId", 
        "Title", 
        "Status", 
        "StatusId", 
        "ReleaseId", 
        "Length", 
        "Number", 
        "Position", 
        "RecordingId", 
        "RecordingLength", 
        "RecordingTitle", 
        "RecordingVideo", 
        "MediaTrackCount", 
        "MediaFormat", 
        "MediaTitle", 
        "MediaPosition", 
        "Mediatrackoffset"
    };
    private readonly List<string> _trackArtistsColumns = new()
    {
        "ReleaseTrackId", 
        "ArtistId", 
        "JoinPhrase", 
        "Index"
    };
    private readonly List<string> _releasesColumns = new()
    {
        "ArtistId", 
        "ReleaseId", 
        "Title", 
        "Status", 
        "StatusId", 
        "Date", 
        "Barcode", 
        "Country", 
        "Disambiguation", 
        "Quality"
    };
    private readonly List<string> _releasesLabelColumns = new()
    {
        "ReleaseId", 
        "LabelId", 
        "CatalogNumber"
    };
    private readonly List<string> _artistColumns = new()
    {
        "ArtistId", 
        "ReleaseCount", 
        "Name", 
        "Type", 
        "Country", 
        "SortName", 
        "Disambiguation", 
        "LastSyncTime"
    };
    private readonly List<string> _labelColumns = new()
    {
        "LabelId", 
        "AreaId", 
        "Name", 
        "Disambiguation", 
        "LabelCode", 
        "Type", 
        "LifeSpanBegin", 
        "LifeSpanEnd", 
        "LifeSpanEnded", 
        "SortName", 
        "TypeId", 
        "Country"
    };
    
    public ImportMusicBrainzCommandHandler(string connectionString)
    {
        _connectionString = connectionString;
        _artistRepository = new MusicBrainzArtistRepository(connectionString);
        _labelRepository = new MusicBrainzLabelRepository(connectionString);
    }

    public async Task ImportAsync(string file)
    {
        FileInfo fileInfo = new FileInfo(file);
        switch (fileInfo.Name.ToLower())
        {
            case "artist":
                await ImportAristFileAsync(fileInfo);
                break;
            case "label":
                await ImportLabelFileAsync(fileInfo);
                break;
            case "release":
                await ImportReleaseFileAsync(fileInfo);
                break;
            default:
                Console.WriteLine("File is not supported or named incorrectly");
                break;
        }
    }

    private async Task ImportAristFileAsync(FileInfo fileInfo)
    {
        int importCount = 0;
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync(Markup.Escape($"Importing artists"), async ctx =>
            {
                List<MusicBrainzArtistInfoModel> artistModels = new List<MusicBrainzArtistInfoModel>();
                
                await ReadJsonLineHelper.ReadByLineAsync<MusicBrainzArtistInfoModel>(fileInfo.FullName, async (artistInfo) =>
                {
                    if (!Guid.TryParse(artistInfo.Id, out var artistId))
                    {
                        return;
                    }
                    
                    artistModels.Add(artistInfo);

                    if (artistModels.Count > 1000)
                    {
                        var artistIds = artistModels
                            .Select(artist => Guid.Parse(artist.Id))
                            .ToList();
                        artistIds = await _artistRepository.GetArtistIdsToInsertAsync(artistIds);
                        
                        await ProcessArtistsAsync(artistModels
                            .Where(artist => artistIds.Contains(Guid.Parse(artist.Id)))
                            .ToList());
                        artistModels.Clear();
                    }
                    
                    ctx.Status(Markup.Escape($"Imported {importCount++} artists"));
                });
                
                if (artistModels.Any())
                {
                    var artistIds = artistModels
                        .Select(artist => Guid.Parse(artist.Id))
                        .ToList();
                    artistIds = await _artistRepository.GetArtistIdsToInsertAsync(artistIds);
                        
                    await ProcessArtistsAsync(artistModels
                        .Where(artist => artistIds.Contains(Guid.Parse(artist.Id)))
                        .ToList());
                    artistModels.Clear();
                }
            });
    }

    private async Task ProcessArtistsAsync(List<MusicBrainzArtistInfoModel> artistModels)
    {
        var models = artistModels
            .Select(artistInfo => new MusicBrainzArtistDto
            {
                ArtistId = Guid.Parse(artistInfo.Id),
                ReleaseCount = 0,
                Name = artistInfo.Name ?? string.Empty,
                Type = artistInfo.Type ?? string.Empty,
                Country = artistInfo.Country ?? string.Empty,
                SortName = artistInfo.SortName ?? string.Empty,
                Disambiguation = artistInfo.Disambiguation ?? string.Empty,
                LastSyncTime = new DateTime(2000, 1, 1)
            }).ToList();
        
        if (models.Any())
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteBulkInsertAsync(
                "musicbrainz_artist",
                models,
                _artistColumns, 
                onConflict: OnConflict.DoNothing);
        }
    }
    

    private async Task ImportLabelFileAsync(FileInfo fileInfo)
    {
        int importCount = 0;
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync(Markup.Escape($"Importing labels"), async ctx =>
            {
                List<MusicBrainzLabelInfoLabelModel> labelModels = new List<MusicBrainzLabelInfoLabelModel>();
                await ReadJsonLineHelper.ReadByLineAsync<MusicBrainzLabelInfoLabelModel>(fileInfo.FullName, async (labelInfo) =>
                {
                    if (!Guid.TryParse(labelInfo.Id, out var labelId))
                    {
                        return;
                    }

                    labelModels.Add(labelInfo);

                    if (labelModels.Count > 1000)
                    {
                        var labelIds = labelModels
                            .Select(artist => Guid.Parse(artist.Id))
                            .ToList();
                        labelIds = await _labelRepository.GetLabelIdsToInsertAsync(labelIds);
                        
                        await ProcessLabelsAsync(labelModels
                            .Where(label => labelIds.Contains(Guid.Parse(label.Id)))
                            .ToList());
                        labelModels.Clear();
                    }
                    
                    ctx.Status(Markup.Escape($"Imported {importCount++} labels"));
                });
                
                if (labelModels.Any())
                {
                    var labelIds = labelModels
                        .Select(artist => Guid.Parse(artist.Id))
                        .ToList();
                    labelIds = await _labelRepository.GetLabelIdsToInsertAsync(labelIds);
                        
                    await ProcessLabelsAsync(labelModels
                        .Where(label => labelIds.Contains(Guid.Parse(label.Id)))
                        .ToList());
                    labelModels.Clear();
                }
            });
    }

    private async Task ProcessLabelsAsync(List<MusicBrainzLabelInfoLabelModel> labelModels)
    {
        var models = labelModels
            .Select(labelInfo => new MusicBrainzLabelDto
            {
                LabelId = Guid.Parse(labelInfo.Id),
                AreaId = Guid.TryParse(labelInfo.Area?.Id, out var areaId) ? areaId : Guid.Empty,
                Name = labelInfo.Name ?? string.Empty,
                Disambiguation = labelInfo.Disambiguation ?? string.Empty,
                LabelCode = labelInfo.LabelCode ?? 0,
                Type = labelInfo.Type ?? string.Empty,
                LifeSpanBegin = labelInfo.LifeSpan?.Begin ?? string.Empty,
                LifeSpanEnd = labelInfo.LifeSpan?.End ?? string.Empty,
                LifeSpanEnded = labelInfo.LifeSpan?.Ended ?? false,
                SortName = labelInfo.SortName ?? string.Empty,
                TypeId = labelInfo.TypeId ?? string.Empty,
                Country = labelInfo.Country ?? string.Empty,
                
            }).ToList();
        
        if (models.Any())
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteBulkInsertAsync(
                "musicbrainz_label",
                models,
                _labelColumns, 
                onConflict: OnConflict.DoNothing);
        }
    }

    private async Task ImportReleaseFileAsync(FileInfo fileInfo)
    {
        List<MusicBrainzReleaseTrackDto> tracksToInsert = new List<MusicBrainzReleaseTrackDto>();
        List<MusicBrainzReleaseTrackArtistDto> trackArtistsToInsert = new List<MusicBrainzReleaseTrackArtistDto>();
        List<MusicBrainzReleaseDto> releasesToInsert = new List<MusicBrainzReleaseDto>();
        List<MusicBrainzReleaseLabelDto> releaseLabelsToInsert = new List<MusicBrainzReleaseLabelDto>();
        
        int importReleaseCount = 0;
        int importTracksCount = 0;
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync(Markup.Escape($"Importing releases"), async ctx =>
            {
                await ReadJsonLineHelper.ReadByLineAsync<MusicBrainzArtistReleaseModel>(fileInfo.FullName, async (releaseInfo) =>
                {
                    if (!Guid.TryParse(releaseInfo.Id, out var releaseId))
                    {
                        return;
                    }
                    if (!Guid.TryParse(releaseInfo.ReleaseGroup.ArtistCredit.FirstOrDefault()?.Artist?.Id, out var primaryArtistId))
                    {
                        return;
                    }

                    foreach (var label in releaseInfo?.LabeLInfo ?? [])
                    {
                        if (Guid.TryParse(label.Label?.Id, out var labelId))
                        {
                            releaseLabelsToInsert.Add(new MusicBrainzReleaseLabelDto
                            {
                                CatalogNumber = label.CataLogNumber ?? string.Empty,
                                ReleaseId = releaseId,
                                LabelId = labelId
                            });
                        }
                    }
                    
                    releasesToInsert.Add(new MusicBrainzReleaseDto
                    {
                        ReleaseId = releaseId,
                        ArtistId = primaryArtistId,
                        Barcode = releaseInfo.Barcode,
                        Country = releaseInfo.Country,
                        Date = releaseInfo.Date,
                        Disambiguation = releaseInfo.Disambiguation,
                        Quality = releaseInfo.Quality,
                        Status = releaseInfo.Status,
                        StatusId = releaseInfo.StatusId,
                        Title = releaseInfo.Title,
                    });
                    
                    foreach (var media in releaseInfo.Media ?? [])
                    {
                        foreach (var track in media?.Tracks ?? [])
                        {
                            if (!Guid.TryParse(track.Id, out var trackId) ||
                                !Guid.TryParse(track?.Recording?.Id, out var trackRecordingId))
                            {
                                continue;
                            }
                            
                            int index = 0;
                            foreach(var credit in track.Recording.ArtistCredit)
                            {
                                if (Guid.TryParse(credit.Artist?.Id, out var artistId))
                                {
                                    trackArtistsToInsert.Add(new MusicBrainzReleaseTrackArtistDto
                                    {
                                        ArtistId = artistId,
                                        Index = index,
                                        JoinPhrase = credit.JoinPhrase,
                                        ReleaseTrackId = trackId
                                    });
                                }
                                index++;
                            }

                            tracksToInsert.Add(new MusicBrainzReleaseTrackDto
                            {
                                ReleaseTrackId = trackId, 
                                RecordingTrackId = trackRecordingId, 
                                Title = track.Title ?? string.Empty, 
                                Status = releaseInfo.Status ?? string.Empty, 
                                StatusId = releaseInfo.StatusId ?? string.Empty, 
                                ReleaseId = releaseId,
                                Length = track.Length ?? 0,
                                Number = track.Number ?? 0,
                                Position = track.Position ?? 0,
                                RecordingId = trackRecordingId,
                                RecordingLength = track.Recording.Length ?? 0,
                                RecordingTitle = track.Recording.Title ?? string.Empty,
                                RecordingVideo = track.Recording.Video,
                                MediaTrackCount = media?.TrackCount ?? 0,
                                MediaFormat = media?.Format ?? string.Empty,
                                MediaTitle = media?.Title ?? string.Empty,
                                MediaPosition = media?.Position ?? 0,
                                Mediatrackoffset = media?.TrackOffset ?? 0
                            });
                            importTracksCount++;
                        }
                    }

                    if (releasesToInsert.Count > 10000)
                    {
                        await using var conn = new NpgsqlConnection(_connectionString);
                        await conn.ExecuteBulkInsertAsync(
                            "musicbrainz_release_track",
                            tracksToInsert,
                            _trackColumns, 
                            onConflict: OnConflict.DoNothing);
                    
                        await conn.ExecuteBulkInsertAsync(
                            "musicbrainz_release_track_artist",
                            trackArtistsToInsert,
                            _trackArtistsColumns, 
                            onConflict: OnConflict.DoNothing);
                    
                        await conn.ExecuteBulkInsertAsync(
                            "musicbrainz_release",
                            releasesToInsert,
                            _releasesColumns, 
                            onConflict: OnConflict.DoNothing);
                    
                        await conn.ExecuteBulkInsertAsync(
                            "musicbrainz_release_label",
                            releaseLabelsToInsert,
                            _releasesLabelColumns, 
                            onConflict: OnConflict.DoNothing);
                        
                        tracksToInsert.Clear();
                        trackArtistsToInsert.Clear();
                        releasesToInsert.Clear();
                        releaseLabelsToInsert.Clear();
                    }

                    
                    ctx.Status(Markup.Escape($"Importing {importReleaseCount++} releases, {importTracksCount} Tracks"));
                });
            });

        if (releasesToInsert.Count > 0)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.ExecuteBulkInsertAsync(
                "musicbrainz_release_track",
                tracksToInsert,
                _trackColumns, 
                onConflict: OnConflict.DoNothing);
                    
            await conn.ExecuteBulkInsertAsync(
                "musicbrainz_release_track_artist",
                trackArtistsToInsert,
                _trackArtistsColumns, 
                onConflict: OnConflict.DoNothing);
                    
            await conn.ExecuteBulkInsertAsync(
                "musicbrainz_release",
                releasesToInsert,
                _releasesColumns, 
                onConflict: OnConflict.DoNothing);
                    
            await conn.ExecuteBulkInsertAsync(
                "musicbrainz_release_label",
                releaseLabelsToInsert,
                _releasesLabelColumns, 
                onConflict: OnConflict.DoNothing);
                        
            tracksToInsert.Clear();
            trackArtistsToInsert.Clear();
            releasesToInsert.Clear();
            releaseLabelsToInsert.Clear();
        }
    }
}