public class StockQuoteResponseModel
{
    public string Symbol { get; set; } = null!;
    public decimal CurrentPrice { get; set; }
    public decimal HighPrice { get; set; }
    public decimal LowPrice { get; set; }
    public decimal OpenPrice { get; set; }
    public decimal PreviousClose { get; set; }
    public long Timestamp { get; set; }
}
