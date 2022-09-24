using System.ComponentModel.DataAnnotations;

namespace IMS.Webapp.ViewModels
{
    public class ProduceViewModel
    {
        [Required]
        public string ProductionNumber { get; set; } = string.Empty;
        
        [Required]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "You have to select a product.")]
        public int ProductId { get; set; }
        
        [Required]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Quantity has to be greater than 1.")]
        public int QuantityToProduce { get; set; }
    }
}
