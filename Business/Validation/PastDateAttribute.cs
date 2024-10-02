using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Business.Validation
{
    public class PastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!IsValid(value))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
        public override bool IsValid(object value)
        {
            var date = (DateTime)value;

            if (date > DateTime.Now || date < DateTime.Now.AddYears(-100))
            {
                return false;
            }

            return true;
        }
    }
}
