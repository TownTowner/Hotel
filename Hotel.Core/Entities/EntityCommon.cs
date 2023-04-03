using System.ComponentModel.DataAnnotations;

namespace Hotel.Core
{
    public class EntityCommon : EntityBase
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Code { get; set; }
    }
}
