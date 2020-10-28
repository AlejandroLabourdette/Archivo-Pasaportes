using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ArchivoDePasaportes.Models;
using ArchivoDePasaportes.Data;
using ArchivoDePasaportes.ViewModels;
using ArchivoDePasaportes.Dto;

namespace ArchivoDePasaportes.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var criticalReturnDate = (from p in _context.GivePassports where p.Active && DateTime.Now.AddDays(3) >= p.ExpectedReturn select p).ToList();
            var criticalReturnDateDto = new List<GivePassportDto>();
            TransferData.Transfer(criticalReturnDate, criticalReturnDateDto, _context);
            var viewModel = new HomeViewModel()
            {
                GivePassportsData = criticalReturnDateDto
            };
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
