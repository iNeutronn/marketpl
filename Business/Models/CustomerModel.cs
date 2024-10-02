using Business.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class CustomerModel
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Surname { get; set; }
        [PastDate(ErrorMessage = "Birth date cannot be in the future and must be at least 100 years ago.")]
        public DateTime BirthDate { get; set; }
        [Range(0,double.MaxValue)]
        public int DiscountValue { get; set; }
        public ICollection<int> ReceiptsIds { get; set; }
    }
}
