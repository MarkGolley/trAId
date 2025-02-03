using Newtonsoft.Json.Linq;
using Timer = System.Timers.Timer;

class Program
{
    private static Timer _timer;
    private static List<string> _tickerSymbols = ["SMCI", "TSLA"];
    
    static async Task Main(string[] args)
    {
        _timer = new Timer(60000);
        _timer.Elapsed += async (sender, e) => await FetchStockData();
        _timer.Start();
        
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }

    private static async Task FetchStockData()
    {
        var apiKey = GetApiKey();
        foreach (var tickerSymbol in _tickerSymbols)
        {
            var url =  $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={tickerSymbol}&interval=5min&apikey={apiKey}";

            using var client = new HttpClient();
            try
            {
                Console.WriteLine($"Fetching data for ticker...{tickerSymbol}");
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                ProcessData(responseBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
            }
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
    
    private static void ProcessData(string responseBody)
    {
        var json = JObject.Parse(responseBody);
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

        CheckAlerts(symbol, double.Parse(close));
    }

    private static void CheckAlerts(string symbol, double currentPrice)
    {
        var threshold = 150.0;

        if (currentPrice > threshold)
        {
            Console.WriteLine($"ALERT: {symbol} price is above the threshold! Current price: {currentPrice}");
        }
    }
}