using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace MiniMediaDatasetImporter.Commands;

[Command("importmusicbrainz", Description = "Import JSON-Format dataset of MusicBrainz")]
public class ImportMusicBrainzCommand : ICommand
{
    [CommandOption("connection-string", 
        'C', 
        Description = "ConnectionString for Postgres database.", 
        EnvironmentVariable = "CONNECTIONSTRING",
        IsRequired = true)]
    public required string ConnectionString { get; init; }
    
    
    [CommandOption("file", 'f', 
        Description = "File to import", 
        IsRequired = false,
        EnvironmentVariable = "IMPORTMUSICBRAINZ_FILE")]
    public string File { get; set; }
    
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var handler = new ImportMusicBrainzCommandHandler(ConnectionString);

        await handler.ImportAsync(File);
    }
}