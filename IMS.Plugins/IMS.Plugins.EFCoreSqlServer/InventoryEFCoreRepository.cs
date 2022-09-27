using IMS.CoreBusiness;
using IMS.UseCases.PluginInterfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSqlServer
{
    public class InventoryEFCoreRepository : IInventoryRepository
    {
        private readonly IMSContext _db;

        public InventoryEFCoreRepository(IMSContext db)
        {
            _db = db;
        }

        public async Task AddInventoryAsync(Inventory inventory)
        {
            _db.Inventories.Add(inventory);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Inventory>> GetInventoriesByNameAsync(string name)
        {
            return await _db.Inventories.Where(x =>
            x.InventoryName.ToLower().IndexOf(name.ToLower()) >= 0).ToListAsync();
      
        }

        public async Task<Inventory> GetInventoryByIdAsync(int inventoryId)
        {
            var inventory = await _db.Inventories.FindAsync(inventoryId);
            if (inventory is not null) return inventory;

            return new();
        }

        public async Task UpdateInventoryAsync(Inventory inventory)
        {
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