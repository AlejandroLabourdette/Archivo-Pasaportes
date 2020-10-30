using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Areas.Identity.Data;
using ArchivoDePasaportes.Data;
using ArchivoDePasaportes.Dto;
using ArchivoDePasaportes.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArchivoDePasaportes.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserRolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserRolesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public IActionResult Index()
        {
            var users = from u in _context.Users select u;
            List<UserRolesDto> userRolesList = new List<UserRolesDto>();

            foreach (var user in users)
            {
                UserRolesDto userRoles = new UserRolesDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    LastName = user.LastName,
                    SecondLastName = user.SecondLastName,
                    Email = user.Email,
                    Occupation = user.Occupation,
                    IsManager = _userManager.IsInRoleAsync(user, "Manager").Result,
                    IsUser = _userManager.IsInRoleAsync(user, "User").Result
                };
                
                bool IsAdmin = _userManager.IsInRoleAsync(user, "Admin").Result;
                if (!IsAdmin)
                    userRolesList.Add(userRoles);
            }


            var userRolesViewModel = new UserRoleViewModel()
            {
                UserRolesList = userRolesList
            };

            return View(userRolesViewModel);
        }
    }
}
