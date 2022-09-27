using IMS.CoreBusiness;
using IMS.Plugins.EFCoreSqlServer;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSqlServer
{
    public class ProductEFCoreRepository : IProductRepository
    {
        private readonly IDbContextFactory<IMSContext> _contextFactory;

        public ProductEFCoreRepository(IDbContextFactory<IMSContext> contextFactory)
        {
            this._contextFactory = contextFactory;
        }

        public async Task AddProductAsync(Product product)
        {
            using var _db = _contextFactory.CreateDbContext();
            _db.Products.Add(product);
            PreserveInventory(product, _db);

            await _db.SaveChangesAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            using var _db = _contextFactory.CreateDbContext();

            return await _db.Products.Include(x => x.ProductInventories)
                .ThenInclude(x => x.Inventory)
                .FirstOrDefaultAsync(x => x.ProductId == productId);
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
        {
            using var _db = _contextFactory.CreateDbContext();

            return await _db.Products.Where(x => 
                x.ProductName.ToLower()
                .IndexOf(name.ToLower()) >= 0)
                .ToListAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            using var _db = _contextFactory.CreateDbContext();
            var prod = await _db.Products
                        .Include(x => x.ProductInventories)
                        .FirstOrDefaultAsync(x => x.ProductId == product.ProductId);

            if (prod is not null)
            {
                prod.ProductName = product.ProductName;
                prod.Price = product.Price;
                prod.Quantity = product.Quantity;
                prod.ProductInventories = product.ProductInventories;
            }

            PreserveInventory(product, _db);

            await _db.SaveChangesAsync();
        }

        private void PreserveInventory(Product product, IMSContext _db)
        {
            if (product?.ProductInventories is not null
                           && product.ProductInventories.Count > 0)
            {
                foreach (var prodInv in product.ProductInventories)
                {
                    if (prodInv.Inventory is not null)
                    {
                        _db.Entry(prodInv.Inventory).State = EntityState.Unchanged;
                    }
                }
            }
        }
    }
}
