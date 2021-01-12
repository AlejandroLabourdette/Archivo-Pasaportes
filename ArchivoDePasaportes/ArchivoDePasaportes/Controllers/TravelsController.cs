using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchivoDePasaportes.Areas.Identity.Data;
using ArchivoDePasaportes.Data;
using ArchivoDePasaportes.Dto;
using ArchivoDePasaportes.Extensions;
using ArchivoDePasaportes.Models;
using ArchivoDePasaportes.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArchivoDePasaportes.Controllers
{
    //[Authorize]
    public class TravelsController : Controller
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TravelsController (ApplicationDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }


        public IActionResult Main()
        {
            return View();
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

            var pageSize = Utils.PageSize;
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
                    .ThenInclude(t=>t.DestinyCountry)
                .Include(o => o.Passport)
                    .ThenInclude(p => p.Owner)
                .ToList();

            bool userIsAdmin = Utils.IsCurrentUserAdmin(_context, _userManager, _httpContextAccessor);

            OfficialTravelViewModel officialTravelViewModel = new OfficialTravelViewModel()
            {
                OfficialTravelsList = officialTravelsList,
                UserIsAdmin = userIsAdmin
            };

            return View("ListOfficialTravels", officialTravelViewModel);
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
                officialTravels = officialTravels.Where(o => o.ReturnDate == null ? false : o.ReturnDate.Value.Day == arrivalDay);
            int arrivalMonth;
            if (!String.IsNullOrEmpty(searchArrivalMonth) && int.TryParse(searchArrivalMonth, out arrivalMonth))
                officialTravels = officialTravels.Where(o => o.ReturnDate == null ? false : o.ReturnDate.Value.Month == arrivalMonth);
            int arrivalYear;
            if (!String.IsNullOrEmpty(searchArrivalYear) && int.TryParse(searchArrivalYear, out arrivalYear))
                officialTravels = officialTravels.Where(o => o.ReturnDate == null ? false : o.ReturnDate.Value.Year == arrivalYear);
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
            IOrderedQueryable<OfficialTravel> orderedTravels = sortOrder switch
            {
                "passNo_desc" => o.OrderByDescending(o => o.Passport.PassportNo),
                "ci" => o.OrderBy(o => o.Passport.Owner.CI),
                "ci_desc" => o.OrderByDescending(o => o.Passport.Owner.CI),
                "departure" => o.OrderBy(o => o.Ticket.DepartureDate),
                "departure_desc" => o.OrderByDescending(o => o.Ticket.DepartureDate),
                "arrival" => o.OrderBy(o => o.ReturnDate),
                "arrival_desc" => o.OrderByDescending(o => o.ReturnDate),
                "destiny" => o.OrderBy(o => o.Ticket.DestinyCountry.Name),
                "destiny_desc" => o.OrderByDescending(o => o.Ticket.DestinyCountry.Name),
                _ => o.OrderBy(o => o.Passport.PassportNo),
            };
            return orderedTravels;
        }


        public IActionResult ListPermanent(string sortOrder, bool keepOrder, int pageIndex,
            string searchDepartureDay, string searchDepartureMonth, string searchDepartureYear,
            string searchDestiny, string searchCI, string searchPassportNo)
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
            ViewBag.SearchDestiny = searchDestiny;
            ViewBag.SearchCI = searchCI;
            ViewBag.SearchPassportNo = searchPassportNo;

            var permanentTravels = FilterPermanentTravels(
                searchDepartureDay, searchDepartureMonth, searchDepartureYear,
                searchDestiny, searchCI, searchPassportNo);

            var permanentTravelsSorted = SortPermanentTravels(permanentTravels, sortOrder);

            var pageSize = Utils.PageSize;
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
                    .ThenInclude(t => t.DestinyCountry)
                .Include(p => p.Passport)
                    .ThenInclude(p => p.Owner)
                .ToList();
            bool userIsAdmin = Utils.IsCurrentUserAdmin(_context, _userManager, _httpContextAccessor);

            PermanentTravelViewModel permanentTravelViewModel = new PermanentTravelViewModel()
            {
                UserIsAdmin = userIsAdmin,
                PermanentTravelsList = permanentTravelsList
            };

            return View("ListPermanentTravels", permanentTravelViewModel);
        }
        private IQueryable<PermanentTravel> FilterPermanentTravels(
           string searchDepartureDay, string searchDepartureMonth, string searchDepartureYear,
           string searchDestiny,
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
            IOrderedQueryable<PermanentTravel> orderedTravels = sortOrder switch
            {
                "passNo_desc" => p.OrderByDescending(o => o.Passport.PassportNo),
                "ci" => p.OrderBy(o => o.Passport.Owner.CI),
                "ci_desc" => p.OrderByDescending(o => o.Passport.Owner.CI),
                "departure" => p.OrderBy(o => o.Ticket.DepartureDate),
                "departure_desc" => p.OrderByDescending(o => o.Ticket.DepartureDate),
                "destiny" => p.OrderBy(o => o.Ticket.DestinyCountry.Name),
                "destiny_desc" => p.OrderByDescending(o => o.Ticket.DestinyCountry.Name),
                _ => p.OrderBy(o => o.Passport.PassportNo),
            };
            return orderedTravels;
        }
    
        [Authorize(Roles = "Admin")]
        public IActionResult EditFlight(long id, string listType, long ticketId)
        {
            long _ticketId = ticketId;
            if(_ticketId == 0)
            {
                if (listType == "Official")
                    _ticketId = _context.OfficialTravels.SingleOrDefault(ot => ot.Id == id)?.TicketId ?? 0;
                else if (listType == "Permanent")
                    _ticketId = _context.PermanentTravels.SingleOrDefault(pt => pt.Id == id)?.TicketId ?? 0;
            }

            if (_ticketId == 0)
                return RedirectToAction("Main");

            var viewModel = new FlightFormViewModel();
            viewModel.Ticket = _context.Tickets.SingleOrDefault(t => t.Id == _ticketId);
            viewModel.OldTicketId = viewModel.Ticket.Id;
            viewModel.Countries = _context.Countries.ToList();
            viewModel.Occupations = _context.Occupations.ToList();
            viewModel.PermanentTravelsDto = new List<PassInfoPermanentTravelDto>();
            viewModel.OfficialTravelsDto = new List<PassInfoOfficialTravelDto>();

            var permanentTravels = (from pt in _context.PermanentTravels where pt.TicketId == _ticketId select pt).ToList();
            foreach (var permanentTravel in permanentTravels)
            {
                var permanentTravelDto = new PassInfoPermanentTravelDto();
                TransferData.Transfer(permanentTravel, permanentTravelDto, _context);
                viewModel.PermanentTravelsDto.Add(permanentTravelDto);
            }
            var officialTravels = (from ot in _context.OfficialTravels where ot.TicketId == _ticketId select ot).ToList();
            foreach (var officialTravel in officialTravels)
            {
                var officialTravelDto = new PassInfoOfficialTravelDto();
                TransferData.Transfer(officialTravel, officialTravelDto, _context);
                viewModel.OfficialTravelsDto.Add(officialTravelDto);
            }

            return View("FlightForm", viewModel);
        }

        //[Authorize(Roles = "Admin")]
        public IActionResult NewFlight()
        {
            var viewModel = new FlightFormViewModel()
            {
                Countries = _context.Countries.ToList(),
                Occupations = _context.Occupations.ToList()
            };
      
            return View("FlightForm", viewModel);
        }

        [HttpPost]
        public IActionResult SaveFlight(FlightFormViewModel viewModel)
        {
            DeleteNullPassportsFromLists(viewModel);
            
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
                    viewModel.OcupationIncorrect = true;
                    viewModel.Occupations = _context.Occupations.ToList();
                    viewModel.Countries = _context.Countries.ToList();
                    return View("FlightForm", viewModel);
                }

                bool returnDateCorrect = travelData.ReturnDate == null ? true : travelData.ReturnDate > viewModel.Ticket.DepartureDate;
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
                p.DestinyCountryId == viewModel.Ticket.DestinyCountryId &&
                p.DepartureDate == viewModel.Ticket.DepartureDate) != null;

            if (ticketInDb != null)
            {
                bool isModifiedTicketInfo =
                    ticketInDb.DepartureDate != viewModel.Ticket.DepartureDate ||
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
                        TicketId = ticketId,
                    };
                    TransferData.Transfer(officialtravelInfo, officialTravel, _context);
                    _context.OfficialTravels.Add(officialTravel);
                }
            if (viewModel.PermanentTravelsDto != null)
                foreach (var permanentTravelInfo in viewModel.PermanentTravelsDto)
                {
                    if (permanentTravelInfo.PassportNo == "null")
                        continue;

                    var permanentTravel = new PermanentTravel()
                    {
                        TicketId = ticketId
                    };
                    TransferData.Transfer(permanentTravelInfo, permanentTravel, _context);
                    _context.PermanentTravels.Add(permanentTravel);
                }
        }
        private void DeleteNullPassportsFromLists(FlightFormViewModel viewModel) 
        {
            Stack<int> toDelete = new Stack<int>();

            for (int i = 0; i < (viewModel.OfficialTravelsDto?.Count ?? 0); i++)
            {
                if (viewModel.OfficialTravelsDto[i].PassportNo == "null")
                    toDelete.Push(i);
            }
            while (toDelete.Count>0)
                viewModel.OfficialTravelsDto.RemoveAt(toDelete.Pop());

            for (int i = 0; i < (viewModel.PermanentTravelsDto?.Count ?? 0); i++)
            {
                if (viewModel.PermanentTravelsDto[i].PassportNo == "null")
                    toDelete.Push(i);
            }
            while (toDelete.Count > 0)
                viewModel.PermanentTravelsDto.RemoveAt(toDelete.Pop());
        }
    }
}   