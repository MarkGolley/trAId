using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class Program
{
    static async Task Main(string[] args)
    {
        var apiKey = GetApiKey();
        var tickerSymbol = "SMCI";
        var url =  $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={tickerSymbol}&interval=5min&apikey={apiKey}";

        using var client = new HttpClient();
        try
        {
            Console.WriteLine("Fetching tickers...");
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching data: {ex.Message}");
        }
    }

    private static string? GetApiKey()
    {
        try
        {
            var json = File.ReadAllText("appsettings.json");
            var config = JObject.Parse(json);
            return config["apikey"]?.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading appsettings.json: {ex.Message}");
        }
        return null;
    }
}