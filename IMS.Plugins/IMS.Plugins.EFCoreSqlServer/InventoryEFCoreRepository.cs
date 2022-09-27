using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSqlServer
{
    public class InventoryEFCoreRepository : IInventoryRepository
    {
        private readonly IDbContextFactory<IMSContext> _contextFactory;

        public InventoryEFCoreRepository(IDbContextFactory<IMSContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task AddInventoryAsync(Inventory inventory)
        {
            using var _db = _contextFactory.CreateDbContext();
            _db.Inventories.Add(inventory);

            await _db.SaveChangesAsync();

        }

        public async Task<IEnumerable<Inventory>> GetInventoriesByNameAsync(string name)
        {
            using var _db = _contextFactory.CreateDbContext();

            return await _db.Inventories.Where(x =>
                x.InventoryName.ToLower().IndexOf(name.ToLower()) >= 0).ToListAsync();

        }

        public async Task<Inventory> GetInventoryByIdAsync(int inventoryId)
        {
            using var _db = _contextFactory.CreateDbContext();
            var inventory = await _db.Inventories.FindAsync(inventoryId);
            if (inventory is not null) return inventory;

            return new();
        }

        public async Task UpdateInventoryAsync(Inventory inventory)
        {
            using var _db = _contextFactory.CreateDbContext();
            var inv = await _db.Inventories.FindAsync(inventory.InventoryId);
            if (inv is not null)
            {
                inv.InventoryName = inventory.InventoryName;
                inv.Price = inventory.Price;
                inv.Quantity = inventory.Quantity;

                await _db.SaveChangesAsync();
            };
        }
    }
}