using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JWMaps.Models;
using JWMaps.ViewModel;
using System;
using GoogleMaps.LocationServices;
using System.Net.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace JWMaps.Controllers
{
    public class TerritoryMapsController : Controller
    {
        private ApplicationDbContext _context;

        public TerritoryMapsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: TerritoryMaps
        public ActionResult Index()
        {
            //if (User.IsInRole(RoleName.CanAdministrate))
            //    return View("ListAllTerritoryMaps", _context.TerritoryMaps.ToList());

            ApplicationUser user = GetUser();

            var territoryMaps = _context.TerritoryMaps.Where(t => t.UserId.Equals(user.Id));

            return View(territoryMaps.ToList());
        }

        private ApplicationUser GetUser()
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;

            return user;
        }

        // GET: TerritoryMaps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TerritoryMap territoryMap = _context.TerritoryMaps.Find(id);
            if (territoryMap == null)
                return HttpNotFound();

            return View(territoryMap);
        }

        // GET: TerritoryMaps/Create
        public ActionResult Create()
        {
            List<string> neighbourhoods = new List<string>();
            ApplicationUser user = GetUser();

            foreach (Householder householder in _context.Householders.Where(h => h.CongregationId == user.CongregationId).ToList())
                neighbourhoods.Add(householder.Neighbourhood);

            var territoryMapViewModel = new TerritoryMapViewModel
            {
                Neighbourhoods = neighbourhoods
            };

            return View(territoryMapViewModel);
        }

        // POST: TerritoryMaps/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] TerritoryMap territoryMap)
        {
            if (ModelState.IsValid)
            {
                _context.TerritoryMaps.Add(territoryMap);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(territoryMap);
        }

        public ActionResult Show(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            //var territoryMap = _context.TerritoryMaps.Single(t => t.Id == id);

            //if(territoryMap == null)
            //    return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            //return View("TerritoryMapView", territoryMap);

            //var householders = _context.Householders.Where(h => h.TerritoryMapId == id).ToList();

            //return View("TerritoryMapView", householders);
            return View();
        }

        public ActionResult New(TerritoryMapViewModel territoryMapViewModel)
        {
            var locationService = new GoogleLocationService();

            ApplicationUser user = GetUser();
            var householdersToVisit = GetHouseholdersToVisit(territoryMapViewModel.selectedNeighbourhood, user);

            var newTerritoryMap = new TerritoryMap()
            {
                CongregationId = user.CongregationId,
                Householders = new List<Householder>(),
                UserId = user.Id
            };
            
            if(householdersToVisit.Count > 0)
            {
                var firstHouseholderToVisit = householdersToVisit.First();
                newTerritoryMap.Householders.Add(firstHouseholderToVisit);
                householdersToVisit.Remove(firstHouseholderToVisit);                
                
                for (int i = 0; i < territoryMapViewModel.MaxNumberOfHouseholders && i < householdersToVisit.Count(); i++)
                {
                    var distance = locationService.GetDirections(firstHouseholderToVisit.GetAddress(), householdersToVisit[i].GetAddress()).Distance.Split(' ')[0].Replace('.', ',');

                    if (Double.Parse(distance) <= territoryMapViewModel.MaxDistanceAmongHouseholders)
                    {
                        newTerritoryMap.Householders.Add(householdersToVisit[i]);
                        householdersToVisit.Remove(householdersToVisit[i]);
                    }
                }

                _context.TerritoryMaps.Add(newTerritoryMap);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        private List<Householder> GetHouseholdersToVisit(string neighbourhood, ApplicationUser user)
        {
            var myCongregationTerritoriesMap = _context.TerritoryMaps.Where(t => t.CongregationId == user.CongregationId);
            List<Householder> houseHoldersInMap = new List<Householder>();

            foreach (var terrytoryMap in myCongregationTerritoriesMap)
                            houseHoldersInMap.AddRange(terrytoryMap.Householders);
            

            var householdersToVisit = _context.Householders.Where(h => h.Neighbourhood.Equals(neighbourhood) && h.CongregationId == user.CongregationId)
                                                           .Except(houseHoldersInMap)
                                                           .OrderBy(h => h.LastTimeVisited)
                                                           .ToList();

            return householdersToVisit;
        }
        
        public ActionResult List()
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;

            var territoryMapsInDB = _context.TerritoryMaps.Where(h => h.CongregationId == user.CongregationId)
                                                          .ToList()
                                                          .Where(t => t.CreationDate.Date == DateTime.Today)
                                                          .ToList();

            return View("ListTerritoryMapView", territoryMapsInDB);
        }

        // GET: TerritoryMaps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            TerritoryMap territoryMap = _context.TerritoryMaps.Find(id);
            if (territoryMap == null)
            {
                return HttpNotFound();
            }
            return View(territoryMap);
        }

        // POST: TerritoryMaps/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] TerritoryMap territoryMap)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(territoryMap).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(territoryMap);
        }

        //GET: TerritoryMaps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TerritoryMap territoryMap = _context.TerritoryMaps.Find(id);
            if (territoryMap == null)
            {
                return HttpNotFound();
            }
            return View(territoryMap);
        }

        // POST: TerritoryMaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TerritoryMap territoryMap = _context.TerritoryMaps.Find(id);
            _context.TerritoryMaps.Remove(territoryMap);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public HttpResponseMessage Delete(int id)
        {
            TerritoryMap territoryMap = _context.TerritoryMaps.Find(id);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
