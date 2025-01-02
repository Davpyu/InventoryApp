using System.ComponentModel.DataAnnotations;

namespace DotNetService.Domain.Inventory.Dtos
{
    public class InventoryUpdateDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int Quantity { get; set; }

        public static Models.Inventory Assign(InventoryUpdateDto data)
        {
            Models.Inventory res = new()
            {
                Name = data.Name,
                Quantity = data.Quantity,
            };

            return res;
        }
    }
}