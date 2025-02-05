namespace trAId.Models;

/// <summary>
/// A class to represent the ownership details of an individual stock
/// </summary>
public class StockOwnership
{
    /// <summary>
    /// The ticker symbol (name) of the stock.
    /// </summary>
    public string Symbol { get; set; }
    
    /// <summary>
    /// The quantity of the named stock owned. 
    /// </summary>
    public double? Quantity { get; set; }
    
    /// <summary>
    /// The purchase price of the named stock owned. 
    /// </summary>
    public double? PurchasePrice { get; set; }
    
    /// <summary>
    /// The goals set for the named stock owned. 
    /// </summary>
    public StockGoals StockGoals { get; set; }
}

public class StockGoals
{
    /// <summary>
    /// The target price desired to achieve. 
    /// </summary>
    public double? TargetPrice { get; set; }
    
    /// <summary>
    /// The price to exit the named stock if the price falls below purchase price. 
    /// </summary>
    public double? StopLossPrice { get; set; }
    
}