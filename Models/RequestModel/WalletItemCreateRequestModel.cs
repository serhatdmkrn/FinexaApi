namespace FinexaApi.Models.RequestModel
{
    public class WalletItemCreateRequestModel
    {
        public string Type { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public DateTime? Date { get; set; } = DateTime.Now;
    }
}