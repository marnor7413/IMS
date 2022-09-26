using IMS.Webapp.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace IMS.Webapp.ViewModelsValidations
{
    public class Produce_EnsureEnoughInventoryQuantity : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var produceViewModel = validationContext.ObjectInstance as ProduceViewModel;
            if (produceViewModel is not null)
            {
                if (produceViewModel.Product is not null && produceViewModel.Product.ProductInventories is not null)
                {
                    foreach (var pi in produceViewModel.Product.ProductInventories)
                    {
                        if (pi.Inventory is not null
                            && pi.InventoryQuantity * produceViewModel.QuantityToProduce > pi.Inventory.Quantity)
                        {
                            return new ValidationResult($"The inventory ({pi.Inventory.InventoryName}) is not enough to produce {produceViewModel.QuantityToProduce} products",
                                new[] { validationContext.MemberName });
                        }
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
