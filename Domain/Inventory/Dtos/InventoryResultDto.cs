namespace DotNetService.Domain.Inventory.Dtos
{
    public class InventoryResultDto : Models.Inventory
    {

        public InventoryResultDto(Models.Inventory inventory)
        {
            Id = inventory.Id;
            Name = inventory.Name;
            Quantity = inventory.Quantity;
            CreatedAt = inventory.CreatedAt;
            UpdatedAt = inventory.UpdatedAt;
        }

        public static List<InventoryResultDto> MapRepo(List<Models.Inventory> data)
        {
            return data?.Select(data => new InventoryResultDto(data)).ToList();
        }
    }
}