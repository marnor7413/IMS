using IMS.Webapp.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace IMS.Webapp.ViewModelsValidations
{
    public class Sell_EnsureEnoughProductQuantity : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var sellViewModel = validationContext.ObjectInstance as SellViewModel;

            if (sellViewModel.Product is not null)
            {
                if (sellViewModel.Product.Quantity < sellViewModel.QuantityToSell)
                {
                    return new ValidationResult($"There isn't enough product inventory. There is only {sellViewModel.Product.Quantity} in the warehouse",
                        new [] { validationContext.MemberName ?? string.Empty });
                };
            };

            return ValidationResult.Success;
        }
    }
}
