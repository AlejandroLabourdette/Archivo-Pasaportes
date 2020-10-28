using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Data;
using ArchivoDePasaportes.Dto;
using ArchivoDePasaportes.Extensions;
using ArchivoDePasaportes.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchivoDePasaportes.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext _context;
        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
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

            if (!String.IsNullOrEmpty(searchOrigin))
                tickets = tickets.Where(t => t.OriginCountry.Name == searchOrigin);
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
                "origin" => tickets.OrderBy(t => t.OriginCountry.Name),
                "origin_desc" => tickets.OrderByDescending(t => t.OriginCountry.Name),
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

            var tickets_list = tickets
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(t=>t.OriginCountry)
                .Include(t=>t.DestinyCountry)
                .ToList();
            

            return View("ListTickets", tickets_list);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult New()
        {
            return View("New");
        }

        [HttpPost]
        public IActionResult Save(FlightFormViewModel viewModel)
        {
            return RedirectToAction("Index");
        }

        public IActionResult CreateTravel()
        {
            var passports = new List<PassInfoOfficialTravelDto>();
            passports.Add(new PassInfoOfficialTravelDto() { OcupationId = 1, PassportNo = "1232154654" });
            var viewModel = new FlightFormViewModel()
            {
                Countries = _context.Countries.ToList(),
                Occupations = _context.Occupations.ToList(),
                OfficialTravelsDto = passports
            };
            return View("FlightForm", viewModel);
        }
    }
}