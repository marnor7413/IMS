using System.ComponentModel.DataAnnotations;

namespace IMS.CoreBusiness.Validators
{
    public class Product_EnsurePricesIsGreaterThanInventoriesCost: ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var product = validationContext.ObjectInstance as Product;
            if(product is not null)
            {
                if (!ValidatePricing(product))
                    return new ValidationResult($"The product's price is less than the inventories cost: {TotalInventoriesCost(product).ToString("c")}!",
                        new List<string>() { validationContext.MemberName ?? string.Empty} );
            }

            return ValidationResult.Success;
        }

        private double TotalInventoriesCost(Product product)
        {
            if(product is null || product.ProductInventories is null)
            {
                return 0;
            }

            double vadf = product.ProductInventories.Sum(x => x.Inventory?.Price * x.InventoryQuantity ?? 0 );
            return vadf;
        }

        private bool ValidatePricing(Product product)
        {
            if (product.ProductInventories is null || product.ProductInventories.Count <= 0) return true;

            double inventoryPriceSum = TotalInventoriesCost(product);
            if (inventoryPriceSum > product.Price) return false;

            return true;
        }
    }
}
