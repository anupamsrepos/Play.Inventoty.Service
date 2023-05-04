using Play.Inventory.Service.Entities;
using static Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service
{
    public static class Entensions
    {
        public static InventoryItemDto AsDto(this InventoryItem item, string name, string desctiption)
        {
            return new InventoryItemDto(item.CatalogItemId, item.Quantity, name, desctiption, item.AcquiredDate);
        }
    }
}
