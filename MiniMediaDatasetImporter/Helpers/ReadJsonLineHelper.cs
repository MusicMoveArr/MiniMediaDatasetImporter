using System.Text.Json;
using System.Text.Json.Serialization;

namespace MiniMediaDatasetImporter.Helpers;

public class ReadJsonLineHelper
{
    public static async Task ReadByLineAsync<T>(string file, Func<T, Task> action)
    {
        using TextReader reader = new StreamReader(file);

        string? line = string.Empty;
        while (!string.IsNullOrWhiteSpace(line = await reader.ReadLineAsync()))
        {
            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.PropertyNameCaseInsensitive = true;
                
                var deserialized = JsonSerializer.Deserialize<T>(line, options);
                await action(deserialized);
            }
            catch (Exception e)
            {
                
            }
        }
    }
}