using Newtonsoft.Json.Linq;

namespace trAId.Utils;

public class ApiKey
{
    public static string? GetApiKey()
    {
        try
        {
            var json = File.ReadAllText("C:\\Users\\markg\\RiderProjects\\trAId\\trAId\\appsettings.json");
            var config = JObject.Parse(json);
            return config["AlphaVantageApiKey"]?.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading appsettings.json: {ex.Message}");
        }
        return null;
    }
    
}