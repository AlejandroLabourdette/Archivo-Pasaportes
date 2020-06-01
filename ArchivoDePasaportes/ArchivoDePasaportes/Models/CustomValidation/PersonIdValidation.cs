using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Models.CustomValidation
{
    public class PersonIdValidation:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var person = (Person)validationContext.ObjectInstance;
            if (person.Id.Length != 11)
                return new ValidationResult("El CI debe contener 11 números");


            if (!long.TryParse(person.Id, out long _a))
                return new ValidationResult("El CI debe contener solo números");

            return ValidationResult.Success;
        }
    }
}
