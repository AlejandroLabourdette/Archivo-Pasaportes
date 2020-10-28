using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Areas.Identity.Data;
using ArchivoDePasaportes.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace ArchivoDePasaportes.Controllers.Api
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        public UserRolesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpDelete]
        [Route("{Id}")]
        public IActionResult DeletePerson(string id)
        {
            var userInDb = _context.Users.Single(u => u.Id == id);
            if (userInDb == null)
                return NotFound();

            _context.Remove(userInDb);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPut]
        [Route("{Id}")]
        public IActionResult ChangeManagerRole(string id)
        {
            string managerRole = "Manager";
            string userRole = "User";

            var userInDb = _context.Users.Single(u => u.Id == id);
            bool isManager = _userManager.IsInRoleAsync(userInDb, managerRole).Result;
            bool isUser = _userManager.IsInRoleAsync(userInDb, userRole).Result;

            if (isManager)
            {
                _userManager.RemoveFromRoleAsync(userInDb, managerRole);
            }
            else
            {
                _userManager.AddToRoleAsync(userInDb, managerRole);
                if (isUser)
                    _userManager.RemoveFromRoleAsync(userInDb, userRole);
            }

            return Ok();
        }
    }
}
