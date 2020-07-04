using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Models
{
    public class Country
    {
        public byte Id { get; set; }

        [Required]
        public string Iso { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
