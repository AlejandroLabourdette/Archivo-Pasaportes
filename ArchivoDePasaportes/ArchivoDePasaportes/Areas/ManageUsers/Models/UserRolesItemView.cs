using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Areas.Identity.Data;

namespace ArchivoDePasaportes.Areas.ManageUsers.Models
{
    public class UserRolesItemView
    {
        public ApplicationUser User { get; set; }
        public Dictionary<string, bool> Roles { get; set; }
    }
}
