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
using System.Web;

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
                List<ListAllTerritoryMapsViewModel> usersMaps = new List<ListAllTerritoryMapsViewModel>();

                foreach (var user in _context.Users.ToList())
                {
                    var maps = _context.TerritoryMaps.Where(t => t.UserId == user.Id).ToList();

                    if (maps.Count > 0)
                    {
                        ListAllTerritoryMapsViewModel viewModel = new ListAllTerritoryMapsViewModel
                        {
                            TerritoryMaps = maps,
                            User = user
                        };

                        usersMaps.Add(viewModel);
                    }
                }
                

                return View("ListAllTerritoryMaps", usersMaps);
            }

            ApplicationUser myUser = GetUser();

            var territoryMaps = _context.TerritoryMaps.Where(t => t.UserId.Equals(myUser.Id));

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
        //public ActionResult Create(TerritoryTypeViewModel territoryTypeViewModel)
        {
            return View(new TerritoryMapViewModel { Neighbourhoods = new List<string>() });
        }

        [HttpPost]
        public ActionResult GetNeighbourhoodByMapType(string mapType)
        {
            string categoryName = GetCategoryName(mapType);

            if (String.IsNullOrEmpty(categoryName))
                return Json(new { succes = false, JsonRequestBehavior.AllowGet });

            List<string> neighbourhoods = new List<string>();
            ApplicationUser user = GetUser();

            var householders = _context.Householders.Include(h => h.Publisher)
                                                    .Where(h => h.CongregationId == user.CongregationId
                                                            && h.TerritoryMap == null
                                                            && h.Publisher == null
                                                            && h.Category.ToString().Equals(categoryName)).ToList();
            var territoryMaps = _context.TerritoryMaps.Where(t => t.CongregationId == user.CongregationId);

            foreach (Householder householder in householders)
            {
                var territoriesInDb = territoryMaps.Where(t => t.Neighbourhood.Equals(householder.Neighbourhood));
                //var householdersInTerritory = 0;
                //var householdersInNeighbourhood = householders.Where(h => h.Neighbourhood.Equals(householder.Neighbourhood)).Count();



                //foreach (var territory in territoriesInDb)
                //    householdersInTerritory += territory.Householders.Count;

                //if (householdersInTerritory < householdersInNeighbourhood && householder.Category.ToString().Equals(categoryName))
                    neighbourhoods.Add(householder.Neighbourhood);
            }

            var territoryMapViewModel = new TerritoryMapViewModel
            {
                Neighbourhoods = neighbourhoods.Distinct().OrderBy(n => n)
            };

            return Json(new { success = true, Neighbourhoods = neighbourhoods.Distinct().OrderBy(n => n) }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult linkPublisherToHouseholder()
        //{
        //    var publishers = _context.Publishers.Where(p => p.CongregationId == GetUser().CongregationId);
        //    var householders = _context.Householders.Where(h => h.CongregationId == GetUser().CongregationId);

        //    foreach(var publisher in publishers)
        //    {

        //    }
        //}

        private string GetCategoryName(string mapType)
        {
            if (mapType.Equals("Comercial"))
                return "Business";
            else if ((mapType.Equals("Residência")))
                return "House";
            else
                return null;
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
            var householders = _context.TerritoryMaps.Include(t => t.Householders.Select(h => h.Visits))
                                                     .SingleOrDefault(t => t.Id == id).Householders.ToList();

            return View(householders);
        }

        public ActionResult SelectTerritoryType(int id)
        {
            var territoryTypeViewModel = new TerritoryTypeViewModel();
            return PartialView("_TerritoryTypeChoice", territoryTypeViewModel);
        }

        public ActionResult New(TerritoryMapViewModel territoryMapViewModel)
        {
            try
            {
                var locationService = new GoogleLocationService();

                ApplicationUser user = GetUser();
                var householdersToVisit = GetHouseholdersToVisit(territoryMapViewModel, user);

                //Vou mudar isso para refletir as mudanças no algoritmo de montagem dos mapas
                territoryMapViewModel.MaxNumberOfHouseholders = 5;
                territoryMapViewModel.MaxDistanceAmongHouseholders = 5;


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

                    for (int i = 0; newTerritoryMap.Householders.Count() < territoryMapViewModel.MaxNumberOfHouseholders && i < householdersToVisit.Count(); i++)
                    {                        
                        var distance = locationService.GetDirections(firstHouseholderToVisit.GetAddress(), householdersToVisit[i].GetAddress()).Distance.Split(' ')[0];
                        
                        if (Double.Parse(distance) <= territoryMapViewModel.MaxDistanceAmongHouseholders)
                        {                            
                            newTerritoryMap.Householders.Add(householdersToVisit[i]);
                        }
                    }

                    _context.TerritoryMaps.Add(newTerritoryMap);
                    _context.SaveChanges();
                }
                else
                {
                    return View("NoMap");
                }

                return RedirectToAction("Index");
            }
            catch(Exception)
            {
                System.Threading.Thread.Sleep(1000);
                return New(territoryMapViewModel);
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
                                                                                          h.Publisher == null)
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
            var user = GetUser();
            var Visit = new VisitViewModel()
            {
                VisitedHouseHolder = _context.Householders.Single(h => h.Id == id),
                Publishers = _context.Publishers.Where(p => p.CongregationId == user.CongregationId).ToList()
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
