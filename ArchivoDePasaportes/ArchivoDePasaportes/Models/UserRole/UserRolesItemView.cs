using ArchivoDePasaportes.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Models
{
    public class UserRolesItemView
    {
        public ApplicationUser User { get; set; }
        public Dictionary<string, bool> Roles { get; set; }
    }
}
