using static Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Clients
{
    public class CatalogClient
    {
        private readonly HttpClient httpClient;

        public CatalogClient(HttpClient _httpClient)
        {
            this.httpClient = _httpClient;
        }

        public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsync()
        {
            var items = await httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/items");
            return items;
        }
    }
}
