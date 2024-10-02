using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public int ProductCategoryId { get; set; }
        public string CategoryName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string ProductName { get; set; }
        [Range(0,double.MaxValue)]
        public decimal Price { get; set; }
        public ICollection<int> ReceiptDetailIds { get; set; }
    }
}
