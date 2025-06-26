using System.ComponentModel.DataAnnotations;

namespace FinexaApi.Models.RequestModel
{
    public class WalletItemUpdateRequestModel
    {
        [Required]
        public decimal Quantity { get; set; }

        [Required]
        public decimal PurchasePrice { get; set; }
    }
}