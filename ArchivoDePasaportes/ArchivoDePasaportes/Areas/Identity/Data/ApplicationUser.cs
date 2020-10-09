using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string SecondLastName { get; set; }
        public string Occupation { get; set; }
    }
}
