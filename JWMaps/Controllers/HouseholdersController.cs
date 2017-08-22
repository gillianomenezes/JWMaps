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
                householder.DateAdded = DateTime.Now;
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
            }
            
            _context.SaveChanges();
            
            return RedirectToAction("Index","Householders");
        }
        
        public ActionResult New()
        {
            var householderViewModel = new HouseholderViewModel
            {
                Householder = new Householder()
            };

            return View("HouseholdersForm", householderViewModel);
        }

        public ActionResult Edit(int id)
        {
            var householderViewModel = new HouseholderViewModel
            {
                Householder = _context.Householders.Single(h => h.Id == id)
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
    }
}