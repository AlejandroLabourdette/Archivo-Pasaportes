using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Areas.Identity.Data;
using ArchivoDePasaportes.Data;
using ArchivoDePasaportes.Dto;
using ArchivoDePasaportes.Extensions;
using ArchivoDePasaportes.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchivoDePasaportes.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TicketsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index(string sortOrder, bool keepOrder, string searchDay, string searchMonth, string searchYear, string searchOrigin, string searchDestiny, int pageIndex)
        {
            //Sort
            ViewBag.ActualSortOrder = sortOrder;
            ViewBag.DateSortParm = sortOrder == "date" && !keepOrder ? "date_desc" : "date";
            ViewBag.OriginSortParm = sortOrder == "origin" && !keepOrder ? "origin_desc" : "origin";
            ViewBag.DestinySortParm = sortOrder == "destiny" && !keepOrder ? "destiny_desc" : "destiny";

            //Filter
            ViewBag.SearchDay = searchDay;
            ViewBag.SearchMonth = searchMonth;
            ViewBag.SearchYear = searchYear;
            ViewBag.SearchOrigin = searchOrigin;
            ViewBag.SearchDestiny = searchDestiny;

            var tickets = from t in _context.Tickets select t;
            if (!String.IsNullOrEmpty(searchDestiny))
                tickets = tickets.Where(t => t.DestinyCountry.Name == searchDestiny);
            int day;
            if (!String.IsNullOrEmpty(searchDay) && int.TryParse(searchDay, out day))
                tickets = tickets.Where(t => t.DepartureDate.Day == day);
            int month;
            if (!String.IsNullOrEmpty(searchMonth) && int.TryParse(searchMonth, out month))
                tickets = tickets.Where(t => t.DepartureDate.Month == month);
            int year;
            if (!String.IsNullOrEmpty(searchYear) && int.TryParse(searchYear, out year))
                tickets = tickets.Where(t => t.DepartureDate.Year == year);

            tickets = sortOrder switch
            {
                "date_desc" => tickets.OrderByDescending(t => t.DepartureDate),
                "destiny" => tickets.OrderBy(t => t.DestinyCountry.Name),
                "destiny_desc" => tickets.OrderByDescending(t => t.DestinyCountry.Name),
                _ => tickets.OrderBy(t => t.DepartureDate),
            };



            var pageSize = Utils.PageSize;
            int maxPageIndex = tickets.Count() % pageSize == 0 && tickets.Count() > 0 ? tickets.Count() / pageSize : tickets.Count() / pageSize + 1;
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageIndex = pageIndex > maxPageIndex ? maxPageIndex : pageIndex;
            ViewBag.PageIndex = pageIndex;
            ViewBag.MaxPageIndex = maxPageIndex;

            var ticketsList = tickets
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(t=>t.DestinyCountry)
                .ToList();


            FlightViewModel flightViewModel = new FlightViewModel()
            {
                TicketList = ticketsList,
                UserIsAdmin = Utils.IsCurrentUserAdmin(_context, _userManager, _httpContextAccessor)
            };

            return View("ListTickets", flightViewModel);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult CreateTravel()
        {
            var viewModel = new FlightFormViewModel()
            {
                Countries = _context.Countries.ToList(),
                Occupations = _context.Occupations.ToList()
            };
            return View("FlightForm", viewModel);
        }
    }
}