﻿namespace Play.Inventory.Service
{
    public class Dtos
    {
        public record GrantItemDto(Guid UserId, Guid CatalogItemId, int Quantity);
        public record InventoryItemDto(Guid CatalogItemId, int Quantity, string Name, string Description, DateTimeOffset AcquiredDate);
        public record CatalogItemDto(Guid Id, string Name, string Description);


    }
}
