using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JWMaps.Models;
using JWMaps.ViewModel;
using GoogleMaps.LocationServices;

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
            var householders = _context.Householders;

            foreach (var householder in householders)
            {
                var householderViewModel = new HouseholderViewModel
                {
                    Householder = householder,
                    Publishers = _context.Publishers
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
                    Householder = householder,
                    Publishers = _context.Publishers
                };

                return View("HouseholdersForm", householderViewModel);
            }

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
                householderdb.PublisherId = householder.PublisherId;
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
                Householder = new Householder(),
                Publishers = _context.Publishers
            };

            return View("HouseholdersForm", householderViewModel);
        }

        public ActionResult Edit(int id)
        {
            var householderViewModel = new HouseholderViewModel
            {
                Householder = _context.Householders.Single(h => h.Id == id),
                Publishers = _context.Publishers
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