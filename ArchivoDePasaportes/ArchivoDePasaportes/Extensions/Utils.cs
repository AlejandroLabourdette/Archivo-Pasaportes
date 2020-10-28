using ArchivoDePasaportes.Areas.Identity.Data;
using ArchivoDePasaportes.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Extensions
{
    public class Utils
    {
        public static int PageSize = 10;
        public static bool IsCurrentUserAdmin(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            ApplicationDbContext _context = context;
            UserManager<ApplicationUser>  _userManager = userManager;
            IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ApplicationUser currentUser = _userManager.FindByIdAsync(userId).Result;
            bool IsAdmin = _userManager.IsInRoleAsync(currentUser, "Admin").Result;

            return (IsAdmin);
        }
    }
}
