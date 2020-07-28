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
    public class DroppedPassportsController : Controller
    {
        private ApplicationDbContext _context;
        public DroppedPassportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string sortOrder, bool keepOrder, string searchString, int pageIndex)
        {
            ViewBag.ActualSortOrder = sortOrder;
            ViewBag.PassNoSortParm = sortOrder == "passNo" && !keepOrder ? "passNo_desc" : "passNo";
            ViewBag.OwnerCISortParm = sortOrder == "ownerCI" && !keepOrder ? "ownerCI_desc" : "ownerCI";
            ViewBag.OwnerNameSortParm = sortOrder == "ownerName" && !keepOrder ? "ownerName_desc" : "ownerName";
            ViewBag.ExpeditionSortParm = sortOrder == "expedition" && !keepOrder ? "expedition_desc" : "expedition";
            ViewBag.ExpirationSortParm = sortOrder == "expiration" && !keepOrder ? "expiration_desc" : "expiration";
            ViewBag.SearchString = searchString;

            var droppedPassports = (from dp in _context.DroppedPassports select dp);

            if (!String.IsNullOrEmpty(searchString))
                droppedPassports = droppedPassports.Where(dp =>
                    dp.PassportNo.Contains(searchString) ||
                    dp.Owner.CI.Contains(searchString) ||
                    (dp.Owner.FirstName + " " + dp.Owner.LastName).Contains(searchString) ||
                    dp.PassportType.Name.Contains(searchString) ||
                    dp.Source.Name.Contains(searchString) ||
                    dp.DropCause.Name.Contains(searchString));

            switch (sortOrder)
            {
                case "passNo_desc":
                    droppedPassports = droppedPassports.OrderByDescending(dp => dp.PassportNo);
                    break;
                case "ownerCI":
                    droppedPassports = droppedPassports.OrderBy(dp => dp.Owner.CI);
                    break;
                case "ownerCI_desc":
                    droppedPassports = droppedPassports.OrderByDescending(dp => dp.Owner.CI);
                    break;
                case "ownerName":
                    droppedPassports = droppedPassports.OrderBy(dp => dp.Owner.LastName + ", " + dp.Owner.FirstName);
                    break;
                case "ownerName_desc":
                    droppedPassports = droppedPassports.OrderByDescending(dp => dp.Owner.LastName + ", " + dp.Owner.FirstName);
                    break;
                case "expedition":
                    droppedPassports = droppedPassports.OrderBy(dp => dp.ExpeditionDate);
                    break;
                case "expedition_desc":
                    droppedPassports = droppedPassports.OrderByDescending(dp => dp.ExpeditionDate);
                    break;
                case "expiration":
                    droppedPassports = droppedPassports.OrderBy(dp => dp.ExpirationDate);
                    break;
                case "expiration_desc":
                    droppedPassports = droppedPassports.OrderByDescending(dp => dp.ExpirationDate);
                    break;

                default:
                    droppedPassports = droppedPassports.OrderBy(dp => dp.PassportNo);
                    break;
            }

            var pageSize = 5;
            int maxPageIndex = droppedPassports.Count() % pageSize == 0 && droppedPassports.Count() > 0 ? droppedPassports.Count() / pageSize : droppedPassports.Count() / pageSize + 1;
            pageIndex = pageIndex < 1 ? 1 : pageIndex;
            pageIndex = pageIndex > maxPageIndex ? maxPageIndex : pageIndex;
            ViewBag.PageIndex = pageIndex;
            ViewBag.MaxPageIndex = maxPageIndex;

            var droppedPassports_list = droppedPassports
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Include(dp => dp.Owner)
                .Include(dp => dp.PassportType)
                .Include(dp => dp.Source)
                .Include(dp => dp.DropCause)
                .ToList();

            return View("ListDroppedPassports", droppedPassports_list);
        }

        public IActionResult Details(long id)
        {
            var dpInDb = _context.DroppedPassports.SingleOrDefault(dp => dp.Id == id);
            if (dpInDb == null)
                return NotFound();

            dpInDb.Owner = _context.People.Single(p => p.Id == dpInDb.OwnerId);
            dpInDb.PassportType = _context.PassportTypes.Single(pt => pt.Id == dpInDb.PassportTypeId);
            dpInDb.Source = _context.Sources.Single(s => s.Id == dpInDb.SourceId);
            dpInDb.DropCause = _context.DropCauses.Single(s => s.Id == dpInDb.DropCauseId);
            return View(dpInDb);
        }

        public IActionResult Edit(long id)
        {
            var dpInDb = _context.DroppedPassports.SingleOrDefault(dp => dp.Id == id);
            if (dpInDb == null)
                return NotFound();

            var dpDto = new DroppedPassportDto();
            TransferData.Transfer(dpInDb, dpDto, _context);

            var viewModel = new DroppedPassportFormViewModel()
            {
                OldPassportNo = dpInDb.PassportNo,
                DpDto = dpDto,
                DropCauses = _context.DropCauses.ToList(),
                PassportTypes = _context.PassportTypes.ToList(),
                Sources = _context.Sources.ToList()
            };

            return View("DroppedPassportForm", viewModel);
        }

        public IActionResult Save(DroppedPassportFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.DropCauses = _context.DropCauses.ToList();
                viewModel.Sources = _context.Sources.ToList();
                viewModel.PassportTypes = _context.PassportTypes.ToList();
                return View("DroppedPassportForm", viewModel);
            }

            var personInDb = _context.People.SingleOrDefault(p => p.CI == viewModel.DpDto.OwnerCI);
            if (personInDb == null)
            {
                viewModel.NotExistThisPersonInDb = true;
                viewModel.DropCauses = _context.DropCauses.ToList();
                viewModel.Sources = _context.Sources.ToList();
                viewModel.PassportTypes = _context.PassportTypes.ToList();
                return View("DroppedPassportForm", viewModel);
            }

            var dpInDb = _context.DroppedPassports.SingleOrDefault(dp => dp.PassportNo == viewModel.OldPassportNo);
            if (dpInDb == null)
                return NotFound();

            bool newPassportExists = _context.Passports.SingleOrDefault(dp => dp.PassportNo == viewModel.DpDto.PassportNo) != null;
            bool isModifiedPassNo = viewModel.OldPassportNo != viewModel.DpDto.PassportNo;
            if (isModifiedPassNo && newPassportExists)
            {
                viewModel.ExistOtherInDb = true;
                viewModel.DropCauses = _context.DropCauses.ToList();
                viewModel.Sources = _context.Sources.ToList();
                viewModel.PassportTypes = _context.PassportTypes.ToList();
                return View("DroppedPassportForm", viewModel);
            }

            TransferData.Transfer(viewModel.DpDto, dpInDb, _context);

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
