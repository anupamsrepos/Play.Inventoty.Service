using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Play.Common;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using static Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Controllers
{
    [Route("items")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<InventoryItem> itemRepositoty;
        private readonly CatalogClient catalogClient;

        public ItemsController(IRepository<InventoryItem> _itemRepository, CatalogClient catalogClient)
        {
            this.itemRepositoty = _itemRepository;
            this.catalogClient = catalogClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if(userId == Guid.Empty)
            {
                return BadRequest();
            }

            var catalogItems = await catalogClient.GetCatalogItemsAsync();
            var inventoryItemEntities = await itemRepositoty.GetAllAsync(item => item.UserId == userId);

            var inventoryItemsDto = inventoryItemEntities.Select(inventoryItem =>
            {
                var catalogItem = catalogItems.Single(catalogItem => catalogItem.Id == inventoryItem.CatalogItemId);
                return inventoryItem.AsDto(catalogItem.Name, catalogItem.Description);
            });

            //var items = (await itemRepositoty.GetAllAsync(item => item.UserId == userId))
            //    .Select(item => item.AsDto());

            return Ok(inventoryItemsDto);
        }

        [HttpPost]
        public async Task<ActionResult> PostAsync(GrantItemDto grantItemDto)
        {
            var inventoryItem = await itemRepositoty.GetAsync(
                item => item.UserId == grantItemDto.UserId && item.CatalogItemId == grantItemDto.CatalogItemId);

            if(inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemDto.CatalogItemId,
                    UserId = grantItemDto.UserId,
                    Quantity = grantItemDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await itemRepositoty.CreateAsync(inventoryItem);
            }
            else
            {
                inventoryItem.Quantity += grantItemDto.Quantity;
                await itemRepositoty.UpdateAsync(inventoryItem);
            }

            return Ok(inventoryItem);
        }
    }
}
