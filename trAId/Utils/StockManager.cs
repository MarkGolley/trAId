using Newtonsoft.Json;
using trAId.Models;

namespace trAId.Utils;

public class StockManager
{
    private const string FilePath = "stocks.json";

    // Load stock data from file
    public static List<StockOwnership>? LoadStocks()
    {
        if (!File.Exists(FilePath)) return [];

        var json = File.ReadAllText(FilePath);
        return JsonConvert.DeserializeObject<List<StockOwnership>>(json);
    }

    // Save stock data to file
    public static void SaveStocks(List<StockOwnership> stocks)
    {
        var json = JsonConvert.SerializeObject(stocks, Formatting.Indented);
        File.WriteAllText(FilePath, json);
    }

    public static void ViewStocks(List<StockOwnership> stocks)
    {
        Console.WriteLine("\nYour Stocks:");
        foreach (var stock in stocks)
        {
            Console.WriteLine(
                $"Symbol: {stock.Symbol}, Quantity: {stock.Quantity}, Purchase Price: {stock.PurchasePrice}, Stock Goals: {stock.StockGoals}");
        }
    }

    public static void AddStock(List<StockOwnership> stocks)
    {
        Console.Write("Enter stock symbol: ");
        var symbol = Console.ReadLine();

        Console.Write("Enter quantity: ");
        if (!int.TryParse(Console.ReadLine(), out var quantity) || quantity <= 0)
        {
            Console.WriteLine("Invalid quantity.");
            return;
        }

        Console.Write("Enter purchase price: ");
        if (!double.TryParse(Console.ReadLine(), out var price) || price <= 0)
        {
            Console.WriteLine("Invalid price.");
            return;
        }

        Console.Write("Enter target price: ");
        if (!double.TryParse(Console.ReadLine(), out var targetPrice) || targetPrice <= 0)
        {
            Console.WriteLine("Invalid target price.");
            return;
        }

        Console.Write("Enter stop loss price: ");
        if (!double.TryParse(Console.ReadLine(), out var stopLossPrice) || stopLossPrice <= 0)
        {
            Console.WriteLine("Invalid stop loss price.");
            return;
        }

        stocks.Add(new StockOwnership
        {
            Symbol = symbol, Quantity = quantity, PurchasePrice = price,
            StockGoals = new StockGoals { TargetPrice = targetPrice, StopLossPrice = stopLossPrice }
        });
        Console.WriteLine("Stock added successfully!");
    }

    public static void RemoveStock(List<StockOwnership> stocks)
    {
        Console.Write("Enter stock symbol to remove: ");
        var symbol = Console.ReadLine();

        var stock = stocks.Find(s => s.Symbol.Equals(symbol, StringComparison.OrdinalIgnoreCase));
        if (stock == null)
        {
            Console.WriteLine("Stock not found.");
            return;
        }

        stocks.Remove(stock);
        Console.WriteLine("Stock removed successfully!");
    }
}