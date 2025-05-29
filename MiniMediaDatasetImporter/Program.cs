using CliFx;

namespace MiniMediaDatasetImporter;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            await new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                .Build()
                .RunAsync(args);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}