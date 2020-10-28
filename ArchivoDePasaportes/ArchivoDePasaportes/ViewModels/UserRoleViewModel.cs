using ArchivoDePasaportes.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.ViewModels
{
    public class UserRoleViewModel
    {
        public List<UserRolesDto> UserRolesList{ get; set; }

        public string Names(UserRolesDto user)
        {
            return user.Name + " " + user.LastName + " " + user.SecondLastName;
        }
    }
}
