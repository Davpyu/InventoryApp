using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetService.Models
{
    [Serializable]
    public class Base
    {
        [Column(name: "created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column(name: "created_by")] 
        public Guid? CreatedBy { get; set; }
        
        [Column(name: "created_by_username")] 
        public string CreatedByUsername { get; set; } = string.Empty;

        [Column(name: "updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column(name: "updated_by")]
        public Guid? UpdatedBy { get; set; }
        
        [Column(name: "updated_by_username")] 
        public string UpdatedByUsername { get; set; } = string.Empty;
        
        [Column(name: "deleted_at")]
        public DateTime? DeletedAt { get; set; }
        
        [Column(name: "deleted_by")]
        public Guid? DeletedBy { get; set; }
        
        [Column(name: "deleted_by_username")] 
        public string DeletedByUsername { get; set; } = string.Empty;
    }
}