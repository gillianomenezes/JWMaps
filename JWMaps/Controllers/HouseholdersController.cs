using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JWMaps.Models;
using JWMaps.ViewModel;
using GoogleMaps.LocationServices;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace JWMaps.Controllers
{
    public class HouseholdersController : Controller
    {
        private ApplicationDbContext _context;

        public HouseholdersController()
        {
            _context = new ApplicationDbContext();           
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: Householder
        public ActionResult Index()
        {
            List <HouseholderViewModel> householderViewModels = new List<HouseholderViewModel>();
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;

            var householders = _context.Householders.Where(h => h.CongregationId == user.CongregationId);

            foreach (var householder in householders)
            {
                var householderViewModel = new HouseholderViewModel
                {
                    Householder = householder
                };

                householderViewModels.Add(householderViewModel);
            }

            return View("ListHouseholders", householderViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Householder householder)
        {
            if(!ModelState.IsValid)
            {
                var householderViewModel = new HouseholderViewModel
                {
                    Householder = householder
                };

                return View("HouseholdersForm", householderViewModel);
            }

            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;

            var locationService = new GoogleLocationService();
            var point = locationService.GetLatLongFromAddress(householder.Address + ", " + householder.Neighbourhood + "-" + householder.City);
            
            householder.Latitude = point.Latitude;
            householder.Longitude = point.Longitude;            

            if (householder.Id == 0)
            {
                householder.CreationDate = DateTime.Now;
                _context.Householders.Add(householder);
            }
            else
            {
                var householderdb = _context.Householders.Single(h => h.Id == householder.Id);

                householderdb.Name = householder.Name;
                householderdb.Neighbourhood = householder.Neighbourhood;
                householderdb.Phone = householder.Phone;
                householder.CongregationId = user.CongregationId;
                householderdb.Address = householder.Address;
                householderdb.City = householder.City;
                householderdb.Latitude = householder.Latitude;
                householderdb.Longitude = householder.Longitude;
                householderdb.PublisherId = householder.PublisherId;
                householderdb.Observations = householder.Observations;

                RemoveHouseholderFromExistingTerritoryMap(householder.Id);
            }
            
            _context.SaveChanges();
            
            return RedirectToAction("Index","Householders");
        }

        private void RemoveHouseholderFromExistingTerritoryMap(int househoulderId)
        {
            var user = GetUser();

            var territoryMaps = _context.TerritoryMaps.Include(t => t.Householders).Where(t => t.UserId.Equals(user.Id)).ToList();
            Householder householderVisited = new Householder();
            TerritoryMap territoryMapUsed = new TerritoryMap();

            foreach (var territoryMap in territoryMaps)
            {
                var householderFound = territoryMap.Householders.Find(h => h.Id == househoulderId);
                if (householderFound != null)
                {
                    territoryMap.Householders.Remove(householderFound);

                    if (territoryMap.Householders.Count == 0)
                        _context.TerritoryMaps.Remove(_context.TerritoryMaps.Single(t => t.Id == territoryMap.Id));

                    _context.SaveChanges();
                    break;
                }
            }
        }

        private ApplicationUser GetUser()
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;

            return user;
        }

        public ActionResult New()
        {
            var householderViewModel = new HouseholderViewModel
            {
                Householder = new Householder(),
                Publishers = _context.Publishers
            };

            return View("HouseholdersForm", householderViewModel);
        }

        public ActionResult Edit(int id)
        {
            var householderViewModel = new HouseholderViewModel
            {
                Householder = _context.Householders.Include(h => h.Visits).Single(h => h.Id == id),
                Publishers = _context.Publishers.ToList()
            };

            return View("HouseholdersForm", householderViewModel);
        }

        public ActionResult Delete(int id)
        {
            var householderInDb = _context.Householders.Single(h => h.Id == id);
            _context.Householders.Remove(householderInDb);

            _context.SaveChanges();

            return View();
        }

        public ActionResult ListVisits(int id)
        {
            var visits = _context.Householders.Include(h => h.Visits).Single(h => h.Id == id).Visits.ToList();

            return View(visits);
        }
    }
}