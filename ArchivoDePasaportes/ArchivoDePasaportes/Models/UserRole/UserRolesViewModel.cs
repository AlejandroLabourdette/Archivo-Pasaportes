using ArchivoDePasaportes.Areas.Identity.Data;
using ArchivoDePasaportes.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Models
{
    public class UserRolesViewModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public List<UserRolesItemView> UserRoleList;

        public UserRolesViewModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            UserRoleList = new List<UserRolesItemView>();

            Load().Wait();
        }

        public async Task Load()
        {
            var _users = _context.Users.ToList();

            foreach (var user in _users)
            {
                UserRolesItemView userRoles = new UserRolesItemView
                {
                    User = user,
                    Roles = new Dictionary<string, bool>()
                };

                bool adminRole = await _userManager.IsInRoleAsync(user, "Admin");
                userRoles.Roles.Add("Admin", adminRole);
                bool managerRole = await _userManager.IsInRoleAsync(user, "Manager");
                userRoles.Roles.Add("Manager", managerRole);
                bool userRole = await _userManager.IsInRoleAsync(user, "User");
                userRoles.Roles.Add("User", userRole);

                UserRoleList.Add(userRoles);
            }
        }
    }
}
