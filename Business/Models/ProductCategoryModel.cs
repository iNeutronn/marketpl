using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class ProductCategoryModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string CategoryName { get; set; }
        public ICollection<int> ProductIds { get; set; }
    }
}
