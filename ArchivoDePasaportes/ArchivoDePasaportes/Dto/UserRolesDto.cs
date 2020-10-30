using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Dto
{
    public class UserRolesDto
    {
        public string Id { get; set; }
        [Display(Name = "Correo")]
        public string Email { get; set; }
        [Display(Name = "Nombre")]
        public string Name { get; set; }
        [Display(Name = "Primer Apellido")]
        public string LastName { get; set; }
        [Display(Name = "Segundo Apellido")]
        public string SecondLastName { get; set; }
        [Display(Name = "Ocupación Laboral")]
        public string Occupation { get; set; }
        public bool IsManager { get; set; }
        public bool IsUser { get; set; }
    }
}
