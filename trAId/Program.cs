using Newtonsoft.Json.Linq;
using trAId.Models;
using trAId.Utils;
using Timer = System.Timers.Timer;

class Program
{
    private static Timer _timer;
    private static List<StockOwnership>? _stockOwnership;
    
    static async Task Main(string[] args)
    {
        _stockOwnership = StockManager.LoadStocks();
        
        await FetchStockData();
        StockMenuInteraction(_stockOwnership);

        _timer = new Timer(60000);
        _timer.Elapsed += async (sender, e) => await FetchStockData();
        _timer.Start();
        
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
        StockManager.SaveStocks(_stockOwnership);
    }

    private static void StockMenuInteraction(List<StockOwnership> stocks)
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nStock Ownership Menu:");
            Console.WriteLine("1. View Stocks");
            Console.WriteLine("2. Add Stock");
            Console.WriteLine("3. Remove Stock");
            Console.WriteLine("4. Exit");
            Console.Write("Choose an option: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    StockManager.ViewStocks(stocks);
                    break;
                case "2":
                    StockManager.AddStock(stocks);
                    break;
                case "3":
                    StockManager.RemoveStock(stocks);
                    break;
                case "4":
                    StockManager.SaveStocks(stocks);
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private static async Task FetchStockData()
    {
        var apiKey = ApiKey.GetApiKey();
        foreach (var stock in _stockOwnership)
        {
            var url =  $"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={stock.Symbol}&interval=5min&apikey={apiKey}";

            using var client = new HttpClient();
            try
            {
                Console.WriteLine($"Fetching data for ticker...{stock}");
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
        var stock = _stockOwnership.FirstOrDefault(s => s.Symbol == symbol);
        
        if (stock != null)
        {
            double? gainPercentage = (currentPrice - stock.PurchasePrice) / stock.PurchasePrice * 100;
            Console.WriteLine($"ALERT: {symbol} is up by {gainPercentage:F2}% from your purchase price!");

            if (currentPrice > stock.StockGoals.TargetPrice)
            {
                Console.WriteLine($"Sell alert: {symbol} has reached the target price of {stock.StockGoals.TargetPrice}. Consider selling or increase target price!");
            }
            
            if (gainPercentage >= 10)
            {
                Console.WriteLine($"Sell alert: {symbol} has gained 10% or more!");
            }
            else if (gainPercentage <= -5)
            {
                Console.WriteLine($"Loss alert: {symbol} has dropped by 5% or more!");
            }
        }
    }
}