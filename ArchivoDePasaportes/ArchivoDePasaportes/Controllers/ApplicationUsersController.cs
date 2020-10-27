using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Data;
using Microsoft.AspNetCore.Mvc;

namespace ArchivoDePasaportes.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ApplicationUsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var users = from u in _context.Users select u;
            var usersList = users.ToList();

            return View(usersList);
        }
    }
}
