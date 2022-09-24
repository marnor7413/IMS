using IMS.CoreBusiness;
using IMS.UseCases.Activities.Interfaces;
using IMS.UseCases.PluginInterfaces;

namespace IMS.UseCases.Activities
{
    public class ProduceProductUseCase : IProduceProductUseCase
    {
        private readonly IProductTransactionRepository _productTransactionRepository;
        private readonly IProductRepository _productRepository;

        public ProduceProductUseCase(IProductTransactionRepository ProductTransactionRepository,
            IProductRepository productRepository)
        {
            _productTransactionRepository = ProductTransactionRepository;
            _productRepository = productRepository;
        }
        public async Task ExecuteAsync(string productionNumber,
            Product product,
            int quantity,
            string doneBy)
        {
            // add transaction record
            await _productTransactionRepository.ProduceAsync(productionNumber, product, quantity, doneBy);

            //decrease quantity inventories

            //update quantity of product
            product.Quantity += quantity;
            await _productRepository.UpdateProductAsync(product);
        }
    }
}
