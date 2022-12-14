using IMS.CoreBusiness;
using IMS.Webapp.ViewModelsValidations;
using System.ComponentModel.DataAnnotations;

namespace IMS.Webapp.ViewModels
{
    public class SellViewModel
    {
        [Required]
        public string SalesOrderNumber { get; set; } = string.Empty;
        
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Sell_EnsureEnoughProductQuantity]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage ="Quantity has to be greater than 0.")]
        public int QuantityToSell { get; set; }

        [Required]
        [Range(minimum: 0, maximum: int.MaxValue, ErrorMessage = "Price has to be greater or equal to 0.")]
        public double UnitPrice { get; set; }

        public Product? Product { get; set; } = null;
    }
}
