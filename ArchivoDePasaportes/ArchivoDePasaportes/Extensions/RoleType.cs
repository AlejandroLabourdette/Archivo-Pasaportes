using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Extensions
{
    public static class RoleType
    {
        public enum RoleTypes { Admin, Manager, User }

        public static Dictionary<RoleTypes, string> RoleTypeString = new Dictionary<RoleTypes, string>()
        {
        {RoleTypes.Admin, "Admin"},
        {RoleTypes.Manager, "Manager"},
        {RoleTypes.User, "User"}
        };

        public static Dictionary<string, RoleTypes> StringRoleType = new Dictionary<string, RoleTypes>()
        {
        {"Admin", RoleTypes.Admin},
        {"Manager", RoleTypes.Manager},
        {"User", RoleTypes.User}
        };


    }
}
