using MassTransit;
using Play.Catalog.Contracts;
using Play.Common;
using Play.Inventory.Service.Entities;

namespace Play.Inventory.Service.Consumers
{
    public class CatalogItemUpdatedDeleted : IConsumer<CatalogItemDeleted>
    {
        private readonly IRepository<CatalogItem> _catalogItemRepository;

        public CatalogItemUpdatedDeleted(IRepository<CatalogItem> repository)
        {
            this._catalogItemRepository = repository;
        }

        public async Task Consume(ConsumeContext<CatalogItemDeleted> context)
        {
            var message = context.Message;
            var item = await _catalogItemRepository.GetAsync(message.ItemId);
            if (item == null)
            {
                return;
            }
            else
            {   
                await _catalogItemRepository.RemoveAsync(message.ItemId);
            }

            
        }
    }
}
