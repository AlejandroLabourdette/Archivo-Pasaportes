using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Data;
using ArchivoDePasaportes.Dto;
using ArchivoDePasaportes.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchivoDePasaportes.Controllers
{
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

            switch (sortOrder)
            {
                case "date_desc":
                    tickets = tickets.OrderByDescending(t => t.DepartureDate);
                    break;
                case "origin":
                    tickets = tickets.OrderBy(t => t.OriginCountry.Name);
                    break;
                case "origin_desc":
                    tickets = tickets.OrderByDescending(t => t.OriginCountry.Name);
                    break;
                case "destiny":
                    tickets = tickets.OrderBy(t => t.DestinyCountry.Name);
                    break;
                case "destiny_desc":
                    tickets = tickets.OrderByDescending(t => t.DestinyCountry.Name);
                    break;

                default:
                    tickets = tickets.OrderBy(t => t.DepartureDate);
                    break;
            }

            var pageSize = 5;
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
            var passports = new List<PassportToTravelDto>();
            passports.Add(new PassportToTravelDto() { OcupationId = 1, PassportNo = "1232154654" });
            var viewModel = new FlightFormViewModel()
            {
                Countries = _context.Countries.ToList(),
                Occupations = _context.Occupations.ToList(),
                PassportsToTravelDto = passports
            };
            return View("FlightForm", viewModel);
        }
    }
}