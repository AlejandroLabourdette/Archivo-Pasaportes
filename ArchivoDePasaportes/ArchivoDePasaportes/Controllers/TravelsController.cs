using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Data;
using ArchivoDePasaportes.Dto;
using ArchivoDePasaportes.Models;
using ArchivoDePasaportes.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchivoDePasaportes.Controllers
{
    public class TravelsController : Controller
    {
        public ApplicationDbContext _context { get; set; }
        public TravelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ListOfficial(string sortOrder, bool keepOrder, int pageIndex,
            string searchDepartureDay, string searchDepartureMonth, string searchDepartureYear, 
            string searchArrivalDay, string searchArrivalMonth, string searchArrivalYear, 
            string searchOrigin, string searchDestiny,
            string searchCI, string searchPassportNo)
        {
            //Sort
            ViewBag.ActualSortOrder = sortOrder;
            ViewBag.DepartureSortParm = sortOrder == "departure" && !keepOrder ? "departure_desc" : "departure";
            ViewBag.ArrivalSortParm = sortOrder == "arrival" && !keepOrder ? "arrival_desc" : "arrival";
            ViewBag.OriginSortParm = sortOrder == "origin" && !keepOrder ? "origin_desc" : "origin";
            ViewBag.DestinySortParm = sortOrder == "destiny" && !keepOrder ? "destiny_desc" : "destiny";
            ViewBag.CISortParm = sortOrder == "ci" && !keepOrder ? "ci_desc" : "ci";
            ViewBag.PassNoSortParm = sortOrder == "passNo" && !keepOrder ? "passNo_desc" : "passNo";

            //Filter
            ViewBag.SearchDepartureDay = searchDepartureDay;
            ViewBag.SearchDepartureMonth = searchDepartureMonth;
            ViewBag.SearchDepartureYear = searchDepartureYear;
            ViewBag.SearchArrivalDay = searchArrivalDay;
            ViewBag.SearchArrivalMonth = searchArrivalMonth;
            ViewBag.SearchArrivalYear = searchArrivalYear;
            ViewBag.SearchOrigin = searchOrigin;
            ViewBag.SearchDestiny = searchDestiny;
            ViewBag.SearchCI = searchCI;
            ViewBag.SearchPassportNo = searchPassportNo;

            var officialTravels = FilterOfficialTravels(
                searchDepartureDay, searchDepartureMonth, searchDepartureYear,
                searchArrivalDay, searchArrivalMonth, searchArrivalYear,
                searchOrigin, searchDestiny, searchCI, searchPassportNo);

            var officialTravelsSorted = SortOfficialTravels(officialTravels, sortOrder);

            var pageSize = 5;
            int maxPageIndex = officialTravelsSorted.Count() % pageSize == 0 && officialTravelsSorted.Count() > 0 ? officialTravelsSorted.Count() / pageSize : officialTravelsSorted.Count() / pageSize + 1;
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageIndex = pageIndex > maxPageIndex ? maxPageIndex : pageIndex;
            ViewBag.PageIndex = pageIndex;
            ViewBag.MaxPageIndex = maxPageIndex;

            var officialTravelsList = officialTravelsSorted
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(o => o.Occupation)
                .Include(o => o.Ticket)
                    .ThenInclude(t=>t.OriginCountry)
                .Include(o => o.Ticket)
                    .ThenInclude(t=>t.DestinyCountry)
                .Include(o => o.Passport)
                    .ThenInclude(p => p.Owner)
                .ToList();

            return View("ListOfficialTravels", officialTravelsList);
        }
        private IQueryable<OfficialTravel> FilterOfficialTravels(
            string searchDepartureDay, string searchDepartureMonth, string searchDepartureYear,
            string searchArrivalDay, string searchArrivalMonth, string searchArrivalYear,
            string searchOrigin, string searchDestiny,
            string searchCI, string searchPassportNo)
        {
            var officialTravels = from o in _context.OfficialTravels select o;

            int departureDay;
            if (!String.IsNullOrEmpty(searchDepartureDay) && int.TryParse(searchDepartureDay, out departureDay))
                officialTravels = officialTravels.Where(o => o.Ticket.DepartureDate.Day == departureDay);
            int departureMonth;
            if (!String.IsNullOrEmpty(searchDepartureMonth) && int.TryParse(searchDepartureMonth, out departureMonth))
                officialTravels = officialTravels.Where(o => o.Ticket.DepartureDate.Month == departureMonth);
            int departureYear;
            if (!String.IsNullOrEmpty(searchDepartureYear) && int.TryParse(searchDepartureYear, out departureYear))
                officialTravels = officialTravels.Where(o => o.Ticket.DepartureDate.Year == departureYear);
            int arrivalDay;
            if (!String.IsNullOrEmpty(searchArrivalDay) && int.TryParse(searchArrivalDay, out arrivalDay))
                officialTravels = officialTravels.Where(o => o.ReturnDate.Day == arrivalDay);
            int arrivalMonth;
            if (!String.IsNullOrEmpty(searchArrivalMonth) && int.TryParse(searchArrivalMonth, out arrivalMonth))
                officialTravels = officialTravels.Where(o => o.ReturnDate.Month == arrivalMonth);
            int arrivalYear;
            if (!String.IsNullOrEmpty(searchArrivalYear) && int.TryParse(searchArrivalYear, out arrivalYear))
                officialTravels = officialTravels.Where(o => o.ReturnDate.Year == arrivalYear);
            if (!String.IsNullOrEmpty(searchOrigin))
                officialTravels = officialTravels.Where(o => o.Ticket.OriginCountry.Name == searchOrigin);
            if (!String.IsNullOrEmpty(searchDestiny))
                officialTravels = officialTravels.Where(o => o.Ticket.DestinyCountry.Name == searchDestiny);
            if (!String.IsNullOrEmpty(searchCI))
                officialTravels = officialTravels.Where(o => o.Passport.Owner.CI == searchCI);
            if (!String.IsNullOrEmpty(searchPassportNo))
                officialTravels = officialTravels.Where(o => o.Passport.PassportNo == searchPassportNo);

            return officialTravels;
        }
        private IOrderedQueryable<OfficialTravel> SortOfficialTravels(IQueryable<OfficialTravel> o, string sortOrder)
        {
            IOrderedQueryable<OfficialTravel> orderedTravels;
            switch (sortOrder)
            {
                case "passNo_desc":
                    orderedTravels = o.OrderByDescending(o => o.Passport.PassportNo);
                    break;
                case "ci":
                    orderedTravels = o.OrderBy(o => o.Passport.Owner.CI);
                    break;
                case "ci_desc":
                    orderedTravels = o.OrderByDescending(o => o.Passport.Owner.CI);
                    break;
                case "departure":
                    orderedTravels = o.OrderBy(o => o.Ticket.DepartureDate);
                    break;
                case "departure_desc":
                    orderedTravels = o.OrderByDescending(o => o.Ticket.DepartureDate);
                    break;
                case "arrival":
                    orderedTravels = o.OrderBy(o => o.ReturnDate);
                    break;
                case "arrival_desc":
                    orderedTravels = o.OrderByDescending(o => o.ReturnDate);
                    break;
                case "origin":
                    orderedTravels = o.OrderBy(o => o.Ticket.OriginCountry.Name);
                    break;
                case "origin_desc":
                    orderedTravels = o.OrderByDescending(o => o.Ticket.OriginCountry.Name);
                    break;
                case "destiny":
                    orderedTravels = o.OrderBy(o => o.Ticket.DestinyCountry.Name);
                    break;
                case "destiny_desc":
                    orderedTravels = o.OrderByDescending(o => o.Ticket.DestinyCountry.Name);
                    break;
                default:
                    orderedTravels = o.OrderBy(o => o.Passport.PassportNo);
                    break;
            }
            return orderedTravels;
        }


        public IActionResult ListPermanent(string sortOrder, bool keepOrder, int pageIndex,
            string searchDepartureDay, string searchDepartureMonth, string searchDepartureYear,
            string searchOrigin, string searchDestiny,
            string searchCI, string searchPassportNo)
        {
            //Sort
            ViewBag.ActualSortOrder = sortOrder;
            ViewBag.DepartureSortParm = sortOrder == "departure" && !keepOrder ? "departure_desc" : "departure";
            ViewBag.OriginSortParm = sortOrder == "origin" && !keepOrder ? "origin_desc" : "origin";
            ViewBag.DestinySortParm = sortOrder == "destiny" && !keepOrder ? "destiny_desc" : "destiny";
            ViewBag.CISortParm = sortOrder == "ci" && !keepOrder ? "ci_desc" : "ci";
            ViewBag.PassNoSortParm = sortOrder == "passNo" && !keepOrder ? "passNo_desc" : "passNo";

            //Filter
            ViewBag.SearchDepartureDay = searchDepartureDay;
            ViewBag.SearchDepartureMonth = searchDepartureMonth;
            ViewBag.SearchDepartureYear = searchDepartureYear;
            ViewBag.SearchOrigin = searchOrigin;
            ViewBag.SearchDestiny = searchDestiny;
            ViewBag.SearchCI = searchCI;
            ViewBag.SearchPassportNo = searchPassportNo;

            var permanentTravels = FilterPermanentTravels(
                searchDepartureDay, searchDepartureMonth, searchDepartureYear,
                searchOrigin, searchDestiny, searchCI, searchPassportNo);

            var permanentTravelsSorted = SortPermanentTravels(permanentTravels, sortOrder);

            var pageSize = 5;
            int maxPageIndex = permanentTravelsSorted.Count() % pageSize == 0 && permanentTravelsSorted.Count() > 0 ? permanentTravelsSorted.Count() / pageSize : permanentTravelsSorted.Count() / pageSize + 1;
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageIndex = pageIndex > maxPageIndex ? maxPageIndex : pageIndex;
            ViewBag.PageIndex = pageIndex;
            ViewBag.MaxPageIndex = maxPageIndex;

            var permanentTravelsList = permanentTravelsSorted
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(p => p.Occupation)
                .Include(p => p.Ticket)
                    .ThenInclude(t => t.OriginCountry)
                .Include(p => p.Ticket)
                    .ThenInclude(t => t.DestinyCountry)
                .Include(p => p.Passport)
                    .ThenInclude(p => p.Owner)
                .ToList();

            return View("ListPermanentTravels", permanentTravelsList);
        }
        private IQueryable<PermanentTravel> FilterPermanentTravels(
           string searchDepartureDay, string searchDepartureMonth, string searchDepartureYear,
           string searchOrigin, string searchDestiny,
           string searchCI, string searchPassportNo)
        {
            var permanentTravels = from p in _context.PermanentTravels select p;

            int departureDay;
            if (!String.IsNullOrEmpty(searchDepartureDay) && int.TryParse(searchDepartureDay, out departureDay))
                permanentTravels = permanentTravels.Where(p => p.Ticket.DepartureDate.Day == departureDay);
            int departureMonth;
            if (!String.IsNullOrEmpty(searchDepartureMonth) && int.TryParse(searchDepartureMonth, out departureMonth))
                permanentTravels = permanentTravels.Where(p => p.Ticket.DepartureDate.Month == departureMonth);
            int departureYear;
            if (!String.IsNullOrEmpty(searchDepartureYear) && int.TryParse(searchDepartureYear, out departureYear))
                permanentTravels = permanentTravels.Where(p => p.Ticket.DepartureDate.Year == departureYear);
            if (!String.IsNullOrEmpty(searchOrigin))
                permanentTravels = permanentTravels.Where(p => p.Ticket.OriginCountry.Name == searchOrigin);
            if (!String.IsNullOrEmpty(searchDestiny))
                permanentTravels = permanentTravels.Where(p => p.Ticket.DestinyCountry.Name == searchDestiny);
            if (!String.IsNullOrEmpty(searchCI))
                permanentTravels = permanentTravels.Where(p => p.Passport.Owner.CI == searchCI);
            if (!String.IsNullOrEmpty(searchPassportNo))
                permanentTravels = permanentTravels.Where(p => p.Passport.PassportNo == searchPassportNo);

            return permanentTravels;
        }
        private IOrderedQueryable<PermanentTravel> SortPermanentTravels(IQueryable<PermanentTravel> p, string sortOrder)
        {
            IOrderedQueryable<PermanentTravel> orderedTravels;
            switch (sortOrder)
            {
                case "passNo_desc":
                    orderedTravels = p.OrderByDescending(o => o.Passport.PassportNo);
                    break;
                case "ci":
                    orderedTravels = p.OrderBy(o => o.Passport.Owner.CI);
                    break;
                case "ci_desc":
                    orderedTravels = p.OrderByDescending(o => o.Passport.Owner.CI);
                    break;
                case "departure":
                    orderedTravels = p.OrderBy(o => o.Ticket.DepartureDate);
                    break;
                case "departure_desc":
                    orderedTravels = p.OrderByDescending(o => o.Ticket.DepartureDate);
                    break;
                case "origin":
                    orderedTravels = p.OrderBy(o => o.Ticket.OriginCountry.Name);
                    break;
                case "origin_desc":
                    orderedTravels = p.OrderByDescending(o => o.Ticket.OriginCountry.Name);
                    break;
                case "destiny":
                    orderedTravels = p.OrderBy(o => o.Ticket.DestinyCountry.Name);
                    break;
                case "destiny_desc":
                    orderedTravels = p.OrderByDescending(o => o.Ticket.DestinyCountry.Name);
                    break;
                default:
                    orderedTravels = p.OrderBy(o => o.Passport.PassportNo);
                    break;
            }
            return orderedTravels;
        }
    
        
        //public IActionResult EditFlight(long id)
        //{
        //   
        //}


        public IActionResult NewFlight()
        {
            var viewModel = new FlightFormViewModel()
            {
                Countries = _context.Countries.ToList(),
                Occupations = _context.Occupations.ToList(),
                OfficialTravelsDto = new List<PassInfoOfficialTravelDto>()
                {
                    new PassInfoOfficialTravelDto()
                    {
                        OcupationId = 1,
                        PassportNo = "A151",
                        ReturnDate= new DateTime()
                    }
                }
            };

            
            return View("FlightForm", viewModel);
        }

        [HttpPost]
        public IActionResult SaveFlight(FlightFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.Occupations = _context.Occupations.ToList();
                viewModel.Countries = _context.Countries.ToList();
                return View("FlightForm", viewModel);
            }

            //Comprobar q todos los pasaportes existan
            //Comprobar q los Id de ocupacion existan
            //Comprobar q no se dupliquen los pasaportes
            for (int i = 0; i < (viewModel.OfficialTravelsDto?.Count ?? 0); i++)
            {
                var travelData = viewModel.OfficialTravelsDto[i];

                if (travelData.PassportNo == "null")
                    continue;

                bool existPassport = _context.Passports.SingleOrDefault(p => p.PassportNo == travelData.PassportNo) != null;
                if (!existPassport)
                {
                    viewModel.MissAnyPassport = true;
                    viewModel.Occupations = _context.Occupations.ToList();
                    viewModel.Countries = _context.Countries.ToList();
                    return View("FlightForm", viewModel);
                }

                bool existOccupation = _context.Occupations.SingleOrDefault(o => o.Id == travelData.OcupationId) != null;
                if (!existOccupation)
                {
                    return BadRequest();
                }

                bool returnDateCorrect = travelData.ReturnDate > viewModel.Ticket.DepartureDate;
                if (!returnDateCorrect)
                {
                    viewModel.ReturnDateIncorrect = true;
                    viewModel.Occupations = _context.Occupations.ToList();
                    viewModel.Countries = _context.Countries.ToList();
                    return View("FlightForm", viewModel);
                }

                for (int j = i+1; j < viewModel.OfficialTravelsDto.Count; j++)
                {
                    var toCompareData = viewModel.OfficialTravelsDto[j];
                    if (travelData.PassportNo == toCompareData.PassportNo)
                    {
                        viewModel.RepetedPassport = true;
                        viewModel.Occupations = _context.Occupations.ToList();
                        viewModel.Countries = _context.Countries.ToList();
                        return View("FlightForm", viewModel);
                    }
                }
            }
            for (int i = 0; i < (viewModel.PermanentTravelsDto?.Count ?? 0); i++)
            {
                var travelData = viewModel.PermanentTravelsDto[i];

                if (travelData.PassportNo == "null")
                    continue;

                bool existPassport = _context.Passports.SingleOrDefault(p => p.PassportNo == travelData.PassportNo) != null;
                if (!existPassport)
                {
                    viewModel.MissAnyPassport = true;
                    viewModel.Occupations = _context.Occupations.ToList();
                    viewModel.Countries = _context.Countries.ToList();
                    return View("FlightForm", viewModel);
                }

                bool existOccupation = _context.Occupations.SingleOrDefault(o => o.Id == travelData.OcupationId) != null;
                if (!existOccupation)
                {
                    return BadRequest();
                }

                for (int j = i + 1; j < viewModel.PermanentTravelsDto.Count; j++)
                {
                    var toCompareData = viewModel.PermanentTravelsDto[j];
                    if (travelData.PassportNo == toCompareData.PassportNo)
                    {
                        viewModel.RepetedPassport = true;
                        viewModel.Occupations = _context.Occupations.ToList();
                        viewModel.Countries = _context.Countries.ToList();
                        return View("FlightForm", viewModel);
                    }
                }
            }

            var ticketInDb = _context.Tickets.SingleOrDefault(t => t.Id == viewModel.OldTicketId);
            bool newTicketExists = _context.Tickets.SingleOrDefault(p =>
                p.OriginCountryId == viewModel.Ticket.OriginCountryId &&
                p.DestinyCountryId == viewModel.Ticket.DestinyCountryId &&
                p.DepartureDate == viewModel.Ticket.DepartureDate) != null;

            if (ticketInDb != null)
            {
                bool isModifiedTicketInfo =
                    ticketInDb.DepartureDate != viewModel.Ticket.DepartureDate ||
                    ticketInDb.OriginCountryId != viewModel.Ticket.OriginCountryId ||
                    ticketInDb.DestinyCountryId != viewModel.Ticket.DestinyCountryId;
                if (isModifiedTicketInfo && newTicketExists)
                {
                    viewModel.ExistOtherTicketInDb = true;
                    viewModel.Occupations = _context.Occupations.ToList();
                    viewModel.Countries = _context.Countries.ToList();
                    return View("FlightForm", viewModel);
                }
                //Delete all travels with old ticket
                var officialTravels = from ot in _context.OfficialTravels where ot.TicketId == ticketInDb.Id select ot;
                var permanentTravels = from pt in _context.PermanentTravels where pt.TicketId == ticketInDb.Id select pt;
                _context.OfficialTravels.RemoveRange(officialTravels.ToList());
                _context.PermanentTravels.RemoveRange(permanentTravels.ToList());
                _context.SaveChanges();

                ticketInDb.OriginCountryId = viewModel.Ticket.OriginCountryId;
                ticketInDb.DestinyCountryId = viewModel.Ticket.DestinyCountryId;
                ticketInDb.DepartureDate = viewModel.Ticket.DepartureDate;
                AddTravelsToDb(viewModel, ticketInDb.Id);
            }
            else if (newTicketExists)
            {
                viewModel.ExistOtherTicketInDb = true;
                viewModel.Occupations = _context.Occupations.ToList();
                viewModel.Countries = _context.Countries.ToList();
                return View("FlightForm", viewModel);
            }
            else
            {
                _context.Tickets.Add(viewModel.Ticket);
                _context.SaveChanges();
                AddTravelsToDb(viewModel, viewModel.Ticket.Id);
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        private void AddTravelsToDb(FlightFormViewModel viewModel, long ticketId)
        {
            if (viewModel.OfficialTravelsDto != null)
                foreach (var officialtravelInfo in viewModel.OfficialTravelsDto)
                {
                    if (officialtravelInfo.PassportNo == "null")
                        continue;

                    var officialTravel = new OfficialTravel()
                    {
                        PassportId = _context.Passports.Single(p => p.PassportNo == officialtravelInfo.PassportNo).Id,
                        OccupationId = officialtravelInfo.OcupationId,
                        TicketId = ticketId,
                        ReturnDate = officialtravelInfo.ReturnDate
                    };
                    _context.OfficialTravels.Add(officialTravel);
                }
            if (viewModel.PermanentTravelsDto != null)
                foreach (var permanentTravelInfo in viewModel.PermanentTravelsDto)
                {
                    if (permanentTravelInfo.PassportNo == "null")
                        continue;

                    var permanentTravel = new PermanentTravel()
                    {
                        PassportId = _context.Passports.Single(p => p.PassportNo == permanentTravelInfo.PassportNo).Id,
                        OccupationId = permanentTravelInfo.OcupationId,
                        TicketId = ticketId
                    };
                    _context.PermanentTravels.Add(permanentTravel);
                }
        }
    }
}   