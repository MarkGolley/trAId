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
            var json = JObject.Parse(responseBody);

            ExtractDataAndPrint(json);
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
    
    private static void ExtractDataAndPrint(JObject json)
    {
        var metaData = json["Meta Data"];
        var symbol = metaData["2. Symbol"].ToString();
        var lastRefreshed = metaData["3. Last Refreshed"].ToString();
        
        var timeSeries = json["Time Series (5min)"];
        var latestEntry = timeSeries[lastRefreshed];
        
        var open = latestEntry["1. open"]?.ToString();
        var high = latestEntry["2. high"]?.ToString();
        var low = latestEntry["3. low"]?.ToString();
        var close = latestEntry["4. close"]?.ToString();
        var volume = latestEntry["5. volume"]?.ToString();
        
        Console.WriteLine($"Symbol: {symbol}");
        Console.WriteLine($"Last Refreshed: {lastRefreshed}");
        Console.WriteLine($"Open: {open}");
        Console.WriteLine($"High: {high}");
        Console.WriteLine($"Low: {low}");
        Console.WriteLine($"Close: {close}");
        Console.WriteLine($"Volume: {volume}");
    }
}