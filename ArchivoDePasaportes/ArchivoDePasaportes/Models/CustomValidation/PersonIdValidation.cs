using ArchivoDePasaportes.Dto;
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
            var person = (PersonDto)validationContext.ObjectInstance;
            if (person.CI == null || person.CI == "")
                return new ValidationResult("Por favor ingrese el CI");

            if (person.CI.Length != 11)
                return new ValidationResult("El CI debe contener 11 números");


            if (!long.TryParse(person.CI, out long _a))
                return new ValidationResult("El CI debe contener solo números");

            return ValidationResult.Success;
        }
    }
}
