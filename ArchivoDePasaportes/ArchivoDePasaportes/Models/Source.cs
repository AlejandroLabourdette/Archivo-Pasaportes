using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Models
{
    public class Source
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Es necesario el nombre de la instalación")]
        [Display(Name="Nombre")]
        public string Name { get; set; }

        [Display(Name="Descripción")]
        public string Description { get; set; }
    }
}

/*
Genius
Love, Death & Robots
Fleabag
Black Sails
Dr. House
Vis a Vis (temp 3 y 4)
Dark
House of Cards
Sharp Objects
Lucifer
Big Little Lies
Sherlock
The Crown
True Detective
Fargo
When they See Us (Asi nos ven)
Mindhunter
*/



