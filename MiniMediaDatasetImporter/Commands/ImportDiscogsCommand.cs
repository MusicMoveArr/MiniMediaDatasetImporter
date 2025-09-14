using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace MiniMediaDatasetImporter.Commands;

[Command("importdiscogs", Description = "Import Discogs dataset")]
public class ImportDiscogsCommand : ICommand
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
        EnvironmentVariable = "IMPORTDISCOGS_FILE")]
    public string File { get; set; }
    
    public async ValueTask ExecuteAsync(IConsole console)
    {
        var handler = new ImportDiscogsCommandHandler(ConnectionString);

        await handler.ImportAsync(File);
    }
}