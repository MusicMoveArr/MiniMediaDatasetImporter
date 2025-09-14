using System.Text.Json.Serialization;
using DapperBulkQueries.Common;
using DapperBulkQueries.Npgsql;
using MiniMediaDatasetImporter.Helpers;
using MiniMediaDatasetImporter.Models.Discogs;
using MiniMediaDatasetImporter.Models.DiscogsDto;
using MiniMediaDatasetImporter.Repositories;
using Newtonsoft.Json;
using Npgsql;
using Spectre.Console;

namespace MiniMediaDatasetImporter.Commands;

public class ImportDiscogsCommandHandler
{
    private readonly string _connectionString;
    private const int BulkInsert = 10000;

    private readonly List<string> _artistColumns = new()
    {
        "ArtistId", 
        "Name", 
        "RealName", 
        "Profile", 
        "DataQuality", 
        "lastsynctime"
    };
    private readonly List<string> _artistUrlColumns = new()
    {
        "ArtistId", 
        "Url"
    };
    private readonly List<string> _aliasColumns = new()
    {
        "Id", 
        "Name"
    };
    
    private readonly List<string> _artistAliasColumns = new()
    {
        "ArtistId", 
        "AliasId"
    };

    private readonly List<string> _labelColumns = new()
    {
        "LabelId", 
        "Name", 
        "ContactInfo", 
        "Profile", 
        "DataQuality"
    };

    private readonly List<string> _labelUrlColumns = new()
    {
        "LabelId", 
        "Url"
    };

    private readonly List<string> _labelSubLabelColumns = new()
    {
        "LabelId", 
        "SubLabelId"
    };

    private readonly List<string> _sublabelColumns = new()
    {
        "SubLabelId", 
        "Name"
    };

    private readonly List<string> _releasesColumns = new()
    {
        "ReleaseId", 
        "Status", 
        "Title", 
        "Country", 
        "Released", 
        "Notes", 
        "DataQuality", 
        "IsMainRelease", 
        "MasterId"
    };

    private readonly List<string> _releaseGenreColumns = new()
    {
        "ReleaseId", 
        "Genre"
    };

    private readonly List<string> _releaseStyleColumns = new()
    {
        "ReleaseId", 
        "Style"
    };

    private readonly List<string> _releaseFormatColumns = new()
    {
        "ReleaseFormatUuId", 
        "ReleaseId", 
        "Name", 
        "Quantity", 
        "Text"
    };

    private readonly List<string> _releaseFormatDescriptionColumns = new()
    {
        "ReleaseFormatUuId", 
        "Description"
    };

    private readonly List<string> _releaseArtistColumns = new()
    {
        "ReleaseId", 
        "ArtistId"
    };

    private readonly List<string> _releaseLabelColumns = new()
    {
        "ReleaseId", 
        "LabelId", 
        "Name", 
        "Catno"
    };

    private readonly List<string> _releaseExtraArtistColumns = new()
    {
        "ReleaseId", 
        "ArtistId", 
        "Anv", 
        "Role"
    };

    private readonly List<string> _releaseIdentifierColumns = new()
    {
        "ReleaseId", 
        "Description", 
        "Type", 
        "Value"
    };

    private readonly List<string> _releaseVideoColumns = new()
    {
        "ReleaseId", 
        "Duration", 
        "Embed", 
        "Source", 
        "Title", 
        "Description"
    };

    private readonly List<string> _releaseCompanyColumns = new()
    {
        "ReleaseId", 
        "CompanyId", 
        "Name", 
        "EntityType", 
        "EntityTypeName", 
        "ResourceUrl"
    };

    private readonly List<string> _releaseTrackColumns = new()
    {
        "ReleaseId", 
        "Title", 
        "Position", 
        "Duration"
    };
    
    public ImportDiscogsCommandHandler(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task ImportAsync(string file)
    {
        FileInfo fileInfo = new FileInfo(file);

        if (fileInfo.Name.EndsWith("_artists.xml"))
        {
            await ImportAristFileAsync(fileInfo);
        }
        else if (fileInfo.Name.EndsWith("_labels.xml"))
        {
            await ImportLabelFileAsync(fileInfo);
        }
        //else if (fileInfo.Name.EndsWith("_masters.xml"))
        //{
        //    await ImportMastersFileAsync(fileInfo);
        //}
        else if (fileInfo.Name.EndsWith("_releases.xml"))
        {
            await ImportReleaseFileAsync(fileInfo);
        }
        else
        {
            Console.WriteLine("File is not supported or named incorrectly");
        }
    }

    private async Task ImportAristFileAsync(FileInfo fileInfo)
    {
        var artistsToInsert = new List<DiscogsArtistDto>();
        var artistUrlsToInsert = new List<DiscogsArtistUrlDto>();
        var aliasToInsert = new List<DiscogsAliasDto>();
        var artistAliasToInsert = new List<DiscogsArtistAliasDto>();
        
        await using var conn = new NpgsqlConnection(_connectionString);
        int importCount = 0;
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync(Markup.Escape($"Importing artists"), async ctx =>
            {
                await ReadXmlHelper.ReadByLineAsync<Artist>(fileInfo.FullName, "artist", async (artistInfo) =>
                {
                    artistsToInsert.Add(new DiscogsArtistDto
                    {
                        ArtistId = artistInfo.Id,
                        Name = artistInfo.Name ?? string.Empty,
                        RealName = artistInfo.RealName ?? string.Empty,
                        Profile = artistInfo.Profile ?? string.Empty,
                        DataQuality = artistInfo.DataQuality ?? string.Empty,
                        lastsynctime = DateTime.Now,
                    });
                    
                    foreach (var url in artistInfo.Urls.Where(x => !string.IsNullOrWhiteSpace(x.Url)))
                    {
                        artistUrlsToInsert.Add(new DiscogsArtistUrlDto
                        {
                            ArtistId = artistInfo.Id,
                            Url = url.Url
                        });
                    }
                    
                    foreach (var alias in artistInfo.Aliases.Where(x => !string.IsNullOrWhiteSpace(x.Name)))
                    {
                        aliasToInsert.Add(new DiscogsAliasDto
                        {
                            Id = alias.Id,
                            Name = alias.Name
                        });
                        
                        artistAliasToInsert.Add(new DiscogsArtistAliasDto
                        {
                            ArtistId = artistInfo.Id,
                            AliasId = alias.Id
                        });
                    }

                    await BulkInsertAsync(artistsToInsert, conn, "discogs_artist", _artistColumns, BulkInsert);
                    await BulkInsertAsync(artistUrlsToInsert, conn, "discogs_artist_url", _artistUrlColumns, BulkInsert);
                    await BulkInsertAsync(aliasToInsert, conn, "discogs_alias", _aliasColumns, BulkInsert);
                    await BulkInsertAsync(artistAliasToInsert, conn, "discogs_artist_alias", _artistAliasColumns, BulkInsert);
                    
                    ctx.Status(Markup.Escape($"Imported {importCount++} artists"));
                });
            });

        await BulkInsertAsync(artistsToInsert, conn, "discogs_artist", _artistColumns, 0);
        await BulkInsertAsync(artistUrlsToInsert, conn, "discogs_artist_url", _artistUrlColumns, 0);
        await BulkInsertAsync(aliasToInsert, conn, "discogs_alias", _aliasColumns, 0);
        await BulkInsertAsync(artistAliasToInsert, conn, "discogs_artist_alias", _artistAliasColumns, 0);
        
    }

    private async Task ImportLabelFileAsync(FileInfo fileInfo)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        var labelsToInsert = new List<DiscogsLabelDto>();
        var labelSubLabelsToInsert = new List<DiscogsLabelSubLabelDto>();
        var labelUrlsToInsert = new List<DiscogsLabelUrlDto>();
        var sublabelsToInsert = new List<DiscogsSublabelDto>();
        
        int importCount = 0;
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync(Markup.Escape($"Importing labels"), async ctx =>
            {
                await ReadXmlHelper.ReadByLineAsync<Label>(fileInfo.FullName, "label", async (labelInfo) =>
                {
                    labelsToInsert.Add(new  DiscogsLabelDto
                    {
                        LabelId = labelInfo.Id,
                        Name = labelInfo.Name ?? string.Empty,
                        ContactInfo = labelInfo.ContactInfo ?? string.Empty,
                        DataQuality = labelInfo.DataQuality ?? string.Empty,
                        Profile = labelInfo.Profile ?? string.Empty
                    });

                    foreach (var subLabel in labelInfo.SubLabels)
                    {
                        labelSubLabelsToInsert.Add(new DiscogsLabelSubLabelDto
                        {
                            LabelId = labelInfo.Id,
                            SubLabelId = subLabel.Id
                        });
                        sublabelsToInsert.Add(new DiscogsSublabelDto
                        {
                            SubLabelId = labelInfo.Id,
                            Name = subLabel.Name
                        });
                    }

                    foreach (var url in labelInfo.Urls.Where(x => !string.IsNullOrWhiteSpace(x.Url)))
                    {
                        labelUrlsToInsert.Add(new DiscogsLabelUrlDto
                        {
                            LabelId = labelInfo.Id,
                            Url = url.Url
                        });
                    }
                    
                    await BulkInsertAsync(labelsToInsert, conn, "discogs_label", _labelColumns, BulkInsert);
                    await BulkInsertAsync(labelSubLabelsToInsert, conn, "discogs_label_sublabel", _labelSubLabelColumns, BulkInsert);
                    await BulkInsertAsync(labelUrlsToInsert, conn, "discogs_label_url", _labelUrlColumns, BulkInsert);
                    await BulkInsertAsync(sublabelsToInsert, conn, "discogs_sublabel", _sublabelColumns, BulkInsert);
                    
                    ctx.Status(Markup.Escape($"Imported {importCount++} labels"));
                });
            });
        
        await BulkInsertAsync(labelsToInsert, conn, "discogs_label", _labelColumns, 0);
        await BulkInsertAsync(labelSubLabelsToInsert, conn, "discogs_label_sublabel", _labelSubLabelColumns, 0);
        await BulkInsertAsync(labelUrlsToInsert, conn, "discogs_label_url", _labelUrlColumns, 0);
        await BulkInsertAsync(sublabelsToInsert, conn, "discogs_sublabel", _sublabelColumns, 0);
    }

    private async Task ImportReleaseFileAsync(FileInfo fileInfo)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        var releaseArtistsToInsert = new List<DiscogsReleaseArtistDto>();
        var releaseCompaniesToInsert = new List<DiscogsReleaseCompanyDto>();
        var releasesToInsert = new List<DiscogsReleaseDto>();
        var releaseExtraArtistsToInsert = new List<DiscogsReleaseExtraArtistDto>();
        var releaseFormatDescriptionsToInsert = new List<DiscogsReleaseFormatDescriptionDto>();
        var releaseFormatsToInsert = new List<DiscogsReleaseFormatDto>();
        var releaseGenresToInsert = new List<DiscogsReleaseGenreDto>();
        var releaseIdentifiersToInsert = new List<DiscogsReleaseIdentifierDto>();
        var releaseLabelsToInsert = new List<DiscogsReleaseLabelDto>();
        var releaseStylesToInsert = new List<DiscogsReleaseStyleDto>();
        var releaseVideosToInsert = new List<DiscogsReleaseVideoDto>();
        var releaseTracksToInsert = new List<DiscogsReleaseTrackDto>();
        
        int importReleaseCount = 0;
        int importTracksCount = 0;
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync(Markup.Escape($"Importing releases"), async ctx =>
            {
                await ReadXmlHelper.ReadByLineAsync<Release>(fileInfo.FullName, "release", async (releaseInfo) =>
                {
                    releasesToInsert.Add(new DiscogsReleaseDto
                    {
                        ReleaseId = releaseInfo.Id,
                        DataQuality = releaseInfo.DataQuality ?? string.Empty,
                        Country = releaseInfo.Country ?? string.Empty,
                        IsMainRelease = releaseInfo.MasterId?.IsMainRelease ?? false,
                        MasterId = releaseInfo.MasterId?.Id ?? 0,
                        Notes = releaseInfo.Notes ?? string.Empty,
                        Released = releaseInfo.Released ?? string.Empty,
                        Status = releaseInfo.Status ?? string.Empty,
                        Title = releaseInfo.Title ?? string.Empty,
                    });

                    foreach (var track in releaseInfo.TrackList.Where(x => !string.IsNullOrWhiteSpace(x.Title)))
                    {
                        releaseTracksToInsert.Add(new  DiscogsReleaseTrackDto
                        {
                            ReleaseId = releaseInfo.Id,
                            Title = track.Title ?? string.Empty,
                            Duration = track.Duration ?? string.Empty,
                            Position = track.Position ?? string.Empty,
                        });
                        importTracksCount++;
                    }

                    foreach (var artist in releaseInfo.Artists)
                    {
                        releaseArtistsToInsert.Add(new DiscogsReleaseArtistDto
                        {
                            ReleaseId = releaseInfo.Id,
                            ArtistId = artist.Id
                        });
                    }

                    foreach (var company in releaseInfo.Companies)
                    {
                        releaseCompaniesToInsert.Add(new DiscogsReleaseCompanyDto
                        {
                            Name = company.Name ?? string.Empty,
                            ReleaseId = releaseInfo.Id,
                            CompanyId = company.Id,
                            EntityType = company.EntityType ?? string.Empty,
                            EntityTypeName = company.EntityTypeName ?? string.Empty,
                            ResourceUrl = company.ResourceUrl ?? string.Empty
                        });
                    }

                    foreach (var artist in releaseInfo.ExtraArtists)
                    {
                        releaseExtraArtistsToInsert.Add(new DiscogsReleaseExtraArtistDto
                        {
                            ReleaseId = releaseInfo.Id,
                            ArtistId = artist.Id,
                            Anv = artist.Anv ?? string.Empty,
                            Role = artist.Role ?? string.Empty,
                        });
                    }

                    foreach (var format in releaseInfo.Formats)
                    {
                        var releaseFormat = new DiscogsReleaseFormatDto
                        {
                            ReleaseId = releaseInfo.Id,
                            Name = format.Name ?? string.Empty,
                            Quantity = format.Quantity,
                            Text = format.Text ?? string.Empty,
                        };
                        releaseFormatsToInsert.Add(releaseFormat);
                        
                        foreach (var description in format.Descriptions.Where(x => !string.IsNullOrWhiteSpace(x.Description)))
                        {
                            releaseFormatDescriptionsToInsert.Add(new DiscogsReleaseFormatDescriptionDto
                            {
                                ReleaseFormatUuId = releaseFormat.ReleaseFormatUuId,
                                Description = description.Description
                            });
                        }
                    }

                    foreach (var genre in releaseInfo.Genres.Where(x => !string.IsNullOrWhiteSpace(x.Genre)))
                    {
                        releaseGenresToInsert.Add(new  DiscogsReleaseGenreDto
                        {
                            ReleaseId = releaseInfo.Id,
                            Genre = genre.Genre
                        });
                    }

                    foreach (var style in releaseInfo.Styles.Where(x => !string.IsNullOrWhiteSpace(x.Style)))
                    {
                        releaseStylesToInsert.Add(new  DiscogsReleaseStyleDto
                        {
                            ReleaseId = releaseInfo.Id,
                            Style = style.Style
                        });
                    }

                    foreach (var identifier in releaseInfo.Identifiers)
                    {
                        releaseIdentifiersToInsert.Add(new  DiscogsReleaseIdentifierDto
                        {
                            ReleaseId = releaseInfo.Id,
                            Description = identifier.Description ?? string.Empty,
                            Type = identifier.Type ?? string.Empty,
                            Value = identifier.Value ?? string.Empty
                        });
                    }

                    foreach (var label in releaseInfo.Labels)
                    {
                        releaseLabelsToInsert.Add(new DiscogsReleaseLabelDto
                        {
                            ReleaseId = releaseInfo.Id,
                            Name = label.Name ?? string.Empty,
                            LabelId = label.Id,
                            Catno = label.Catno ?? string.Empty,
                        });
                    }

                    foreach (var video in releaseInfo.Videos)
                    {
                        releaseVideosToInsert.Add(new DiscogsReleaseVideoDto
                        {
                            ReleaseId = releaseInfo.Id,
                            Description = video.Description ?? string.Empty,
                            Title = video.Title ?? string.Empty,
                            Duration = video.Duration ?? string.Empty,
                            Embed = video.Embed,
                            Source = video.Source ?? string.Empty,
                        });
                    }
                    
                    await BulkInsertAsync(releaseArtistsToInsert, conn, "discogs_release_artist", _releaseArtistColumns, BulkInsert);
                    await BulkInsertAsync(releaseCompaniesToInsert, conn, "discogs_release_company", _releaseCompanyColumns, BulkInsert);
                    await BulkInsertAsync(releasesToInsert, conn, "discogs_release", _releasesColumns, BulkInsert);
                    await BulkInsertAsync(releaseExtraArtistsToInsert, conn, "discogs_release_extra_artist", _releaseExtraArtistColumns, BulkInsert);
                    await BulkInsertAsync(releaseFormatDescriptionsToInsert, conn, "discogs_release_format_description", _releaseFormatDescriptionColumns, BulkInsert);
                    await BulkInsertAsync(releaseFormatsToInsert, conn, "discogs_release_format", _releaseFormatColumns, BulkInsert);
                    await BulkInsertAsync(releaseGenresToInsert, conn, "discogs_release_genre", _releaseGenreColumns, BulkInsert);
                    await BulkInsertAsync(releaseIdentifiersToInsert, conn, "discogs_release_identifier", _releaseIdentifierColumns, BulkInsert);
                    await BulkInsertAsync(releaseLabelsToInsert, conn, "discogs_release_label", _releaseLabelColumns, BulkInsert);
                    await BulkInsertAsync(releaseStylesToInsert, conn, "discogs_release_style", _releaseStyleColumns, BulkInsert);
                    await BulkInsertAsync(releaseVideosToInsert, conn, "discogs_release_video", _releaseVideoColumns, BulkInsert);
                    await BulkInsertAsync(releaseTracksToInsert, conn, "discogs_release_track", _releaseTrackColumns, BulkInsert);
                    
                    ctx.Status(Markup.Escape($"Importing {importReleaseCount++} releases, {importTracksCount} Tracks"));
                });
            });

        
        await BulkInsertAsync(releaseArtistsToInsert, conn, "discogs_release_artist", _releaseArtistColumns, 0);
        await BulkInsertAsync(releaseCompaniesToInsert, conn, "discogs_release_company", _releaseCompanyColumns, 0);
        await BulkInsertAsync(releasesToInsert, conn, "discogs_release", _releasesColumns, 0);
        await BulkInsertAsync(releaseExtraArtistsToInsert, conn, "discogs_release_extra_artist", _releaseExtraArtistColumns, 0);
        await BulkInsertAsync(releaseFormatDescriptionsToInsert, conn, "discogs_release_format_description", _releaseFormatDescriptionColumns, 0);
        await BulkInsertAsync(releaseFormatsToInsert, conn, "discogs_release_format", _releaseFormatColumns, 0);
        await BulkInsertAsync(releaseGenresToInsert, conn, "discogs_release_genre", _releaseGenreColumns, 0);
        await BulkInsertAsync(releaseIdentifiersToInsert, conn, "discogs_release_identifier", _releaseIdentifierColumns, 0);
        await BulkInsertAsync(releaseLabelsToInsert, conn, "discogs_release_label", _releaseLabelColumns, 0);
        await BulkInsertAsync(releaseStylesToInsert, conn, "discogs_release_style", _releaseStyleColumns, 0);
        await BulkInsertAsync(releaseVideosToInsert, conn, "discogs_release_video", _releaseVideoColumns, 0);
        await BulkInsertAsync(releaseTracksToInsert, conn, "discogs_release_track", _releaseTrackColumns, 0);
        
    }
    
    private async Task ImportMastersFileAsync(FileInfo fileInfo)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        
        int importReleaseCount = 0;
        int importTracksCount = 0;
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .StartAsync(Markup.Escape($"Importing releases"), async ctx =>
            {
                await ReadXmlHelper.ReadByLineAsync<Master>(fileInfo.FullName, "master", async (masterInfo) =>
                {
                    
                    ctx.Status(Markup.Escape($"Importing {importReleaseCount++} masters"));
                });
            });
    }

    private async Task BulkInsertAsync<DtoType>(
        List<DtoType> source, 
        NpgsqlConnection conn,
        string tableName, 
        List<string> columns, 
        int minimumInsert) where DtoType : class
    {
        if (source.Count > minimumInsert)
        {
            await conn.ExecuteBulkInsertAsync(
                tableName,
                source,
                columns,
                onConflict: OnConflict.DoNothing);
                        
            source.Clear();
        }
    }
}