using System.ComponentModel.DataAnnotations;

namespace DotNetService.Models
{
    public class Inventory : BaseModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}