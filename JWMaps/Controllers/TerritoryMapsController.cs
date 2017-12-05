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
            if (User.IsInRole(RoleName.CanAdministrate))
            {
                ListAllTerritoryMapsViewModel viewModel = new ListAllTerritoryMapsViewModel
                {
                    TerritoryMaps = _context.TerritoryMaps.ToList(),
                    User = GetUser()
                };

                return View("ListAllTerritoryMaps",viewModel);
            }

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

        public ActionResult Show(int id)
        {
            var householders = _context.TerritoryMaps.Include(t => t.Householders).SingleOrDefault(t => t.Id == id).Householders.ToList();

            return View(householders);
        }

        public ActionResult New(TerritoryMapViewModel territoryMapViewModel)
        {
            try
            {
                var locationService = new GoogleLocationService();

                ApplicationUser user = GetUser();
                var householdersToVisit = GetHouseholdersToVisit(territoryMapViewModel, user);

                if (householdersToVisit == null)
                    return View("NotAvailableHouseholders");

                var newTerritoryMap = new TerritoryMap()
                {
                    CongregationId = user.CongregationId,
                    Neighbourhood = territoryMapViewModel.selectedNeighbourhood,
                    Householders = new List<Householder>(),
                    UserId = user.Id
                };

                if (householdersToVisit.Count > 0)
                {
                    var firstHouseholderToVisit = householdersToVisit.First();
                    newTerritoryMap.Householders.Add(firstHouseholderToVisit);
                    householdersToVisit.Remove(firstHouseholderToVisit);

                    for (int i = 0; i < (territoryMapViewModel.MaxNumberOfHouseholders - 1) && i < householdersToVisit.Count(); i++)
                    {
                        System.Threading.Thread.Sleep(1000);
                        var distance = locationService.GetDirections(firstHouseholderToVisit.GetAddress(), householdersToVisit[i].GetAddress()).Distance.Split(' ')[0].Replace('.', ',');

                        if (Double.Parse(distance) <= territoryMapViewModel.MaxDistanceAmongHouseholders)
                        {
                            newTerritoryMap.Householders.Add(householdersToVisit[i]);
                        }
                    }

                    _context.TerritoryMaps.Add(newTerritoryMap);
                    _context.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch(Exception)
            {
                return RedirectToAction("Index");
            }
        }

        private List<Householder> GetHouseholdersToVisit(TerritoryMapViewModel territory, ApplicationUser user)
        {
            var myCongregationTerritoriesMap = _context.TerritoryMaps.Include(t => t.Householders).Where(t => t.CongregationId == user.CongregationId);
            List<Householder> houseHoldersInMap = new List<Householder>();

            foreach (var terrytoryMap in myCongregationTerritoriesMap)
                houseHoldersInMap.AddRange(terrytoryMap.Householders);


            var householdersToVisit = _context.Householders.Include(h => h.Visits).Where(h => h.Neighbourhood.Equals(territory.selectedNeighbourhood) &&
                                                                                          h.CongregationId == user.CongregationId && 
                                                                                          h.Category == territory.Category && 
                                                                                          h.PublisherId == null)
                                                                                          .ToList();


            householdersToVisit = householdersToVisit.Except(houseHoldersInMap).ToList();
            householdersToVisit.Sort((x, y) => DateTime.Compare(x.LastTimeVisited(), y.LastTimeVisited()));

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

        public ActionResult Delete(int id)
        {
            var territoryMapInDb = _context.TerritoryMaps.Include(t => t.Householders).SingleOrDefault(t => t.Id == id);

            for(int i = 0; i < territoryMapInDb.Householders.Count; i++)            
                territoryMapInDb.Householders.RemoveAt(i);            
            
            _context.TerritoryMaps.Remove(territoryMapInDb);
            _context.SaveChanges();

            return View();
        }

        public ActionResult SaveVisit(VisitViewModel visit)
        {
            var user = GetUser();

            var territoryMaps = _context.TerritoryMaps.Include(t => t.Householders).Where(t => t.UserId.Equals(user.Id)).ToList();
            Householder householderVisited = new Householder();
            TerritoryMap territoryMapUsed = new TerritoryMap();

            foreach (var territoryMap in territoryMaps)
            {
                var householderFound = territoryMap.Householders.Find(h => h.Id == visit.VisitedHouseHolder.Id);
                if (householderFound != null)
                {
                    territoryMapUsed = territoryMap;
                    householderVisited = householderFound;
                    break;
                }
            }

            visit.TerritoryMapUsed = territoryMapUsed;
            visit.VisitedHouseHolder = householderVisited;

            var householderInDb = _context.Householders.Include(h => h.Visits).SingleOrDefault(h => h.Id == visit.VisitedHouseHolder.Id);
            var territoyMapInDb = _context.TerritoryMaps.Include(t => t.Householders).SingleOrDefault(t => t.Id == visit.TerritoryMapUsed.Id);

            territoyMapInDb.Householders.Remove(householderInDb);
            var Visit = new Visit()
            {
                PublisherName = visit.PublisherName,
                DateOfVisit = DateTime.Now,
                Description = visit.VisitDescription
            };

            householderInDb.Visits.Add(Visit);
            
            if (territoyMapInDb.Householders.Count > 0)
            {                
                _context.SaveChanges();
                return RedirectToAction("Show", visit.TerritoryMapUsed);
            }
            else
            {
                _context.TerritoryMaps.Remove(_context.TerritoryMaps.Single(t => t.Id == territoyMapInDb.Id));
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        public ActionResult SetVisit(int id)
        {
            var Visit = new VisitViewModel()
            {
                VisitedHouseHolder = _context.Householders.Single(h => h.Id == id),
                Publishers = _context.Publishers.ToList()
            };

            return View(Visit);
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
