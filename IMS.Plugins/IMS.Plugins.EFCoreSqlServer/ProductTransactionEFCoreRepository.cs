using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSqlServer
{
    public class ProductTransactionEFCoreRepository : IProductTransactionRepository
    {
        private readonly IProductRepository _productRepository;
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMSContext _db;

        public ProductTransactionEFCoreRepository(IProductRepository productRepository,
            IInventoryTransactionRepository inventoryTransactionRepository,
            IInventoryRepository inventoryRepository,
            IMSContext db)
        {
            _productRepository = productRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _inventoryRepository = inventoryRepository;
            _db = db;
        }

        public async Task<IEnumerable<ProductTransaction>> GetProductTransactionAsync(string productName, DateTime? dateFrom, DateTime? dateTo, ProductTransactionType? transactionType)
        {
            var query = from pt in _db.ProductTransactions
                        join prod in _db.Products on pt.ProductId equals prod.ProductId
                        where
                            (string.IsNullOrWhiteSpace(productName) || prod.ProductName.ToLower().IndexOf(productName.ToLower()) >= 0)
                            &&
                            (!dateFrom.HasValue || pt.TransactionDate >= dateFrom.Value.Date) &&
                            (!dateTo.HasValue || pt.TransactionDate <= dateTo.Value.Date) &&
                            (!transactionType.HasValue || pt.ActivityType == transactionType)
                        select pt;

            return await query.Include(x => x.Product).ToListAsync();
        }

        public async Task ProduceAsync(string productionNumber, Product product, int quantity, string doneBy)
        {
            var prod = await _productRepository.GetProductByIdAsync(product.ProductId);
            if (prod is not null )
            {
                foreach (var pi in prod.ProductInventories)
                {
                    if (pi.Inventory is not null)
                    {
                        //add inventory transaction
                        await _inventoryTransactionRepository.ProduceAsync(productionNumber,
                        pi.Inventory,
                        pi.InventoryQuantity * quantity,
                        doneBy,
                        -1);

                        // decrease the inventories
                        var inv = await _inventoryRepository.GetInventoryByIdAsync(pi.Inventory.InventoryId);
                        inv.Quantity -= pi.InventoryQuantity * quantity;
                        await _inventoryRepository.UpdateInventoryAsync(inv);
                    }
                }
            }

            // add product transaction
             _db.ProductTransactions.Add(new ProductTransaction
            {
                ProductionNumber = productionNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                ActivityType = ProductTransactionType.ProduceProduct,
                QuantityAfter = product.Quantity + quantity,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy
            });

            await _db.SaveChangesAsync();
        }

        public async Task SellProductAsync(string salesOrderNumber, Product product, int quantity, double unitPrice, string doneBy)
        {
            _db.ProductTransactions.Add(new ProductTransaction
            {
                ActivityType = ProductTransactionType.SellProduct,
                SONumber = salesOrderNumber,
                ProductId = product.ProductId,
                QuantityBefore = product.Quantity,
                QuantityAfter = product.Quantity - quantity,
                TransactionDate = DateTime.Now,
                DoneBy = doneBy,
                UnitPrice = unitPrice
            });

            await _db.SaveChangesAsync();
        }
    }
}
