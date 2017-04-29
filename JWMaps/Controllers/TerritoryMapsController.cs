using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JWMaps.Models;
using JWMaps.ViewModel;
using System;
using GoogleMaps.LocationServices;

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
            return View(_context.TerritoryMaps.ToList());
        }

        // GET: TerritoryMaps/Details/5
        public ActionResult Details(int? id)
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

        // GET: TerritoryMaps/Create
        public ActionResult Create()
        {
            List<string> neighbourhoods = new List<string>();

            foreach(Householder householder in _context.Householders.ToList())
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

            var territoryMap = _context.TerritoryMaps.Single(t => t.Id == id);

            if(territoryMap == null)
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View("TerritoryMapView", territoryMap);
        }

        public ActionResult New(TerritoryMapViewModel territoryMapViewModel)
        {
            var householdersNotInMap = new List<Householder>();
            var householdersdb = _context.Householders
                                            .ToList()
                                            .Where(h => h.Neighbourhood.Equals(territoryMapViewModel.selectedNeighbourhood))
                                            .ToList();

            if (householdersdb.Count == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            List<TerritoryMap> mapsFromToday = _context.TerritoryMaps
                                                .ToList()
                                                .Where(t => t.CreationDate.Date == DateTime.Today)
                                                .ToList();

            List<TerritoryMap> mapsFromTodayAndFromGivenNeighbourhood = new List<TerritoryMap>();

            foreach(var map in mapsFromToday)
            {
                if(map.Householders.Count > 0)
                {
                    if (map.Householders.First().Neighbourhood.Equals(territoryMapViewModel.selectedNeighbourhood))
                        mapsFromTodayAndFromGivenNeighbourhood.Add(map);
                }
            }

            if (mapsFromTodayAndFromGivenNeighbourhood.Count == 0)
                householdersNotInMap.AddRange(householdersdb);


            foreach (var map in mapsFromTodayAndFromGivenNeighbourhood)
            {
                foreach (var householder in householdersdb)
                {
                    if (!map.Householders.Contains(householder))
                        householdersNotInMap.Add(householder);
                }
            }

            if (householdersNotInMap.Count == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var locationService = new GoogleLocationService();
            var newTerritoryMap = new TerritoryMap();
            newTerritoryMap.Householders = new List<Householder>();

            AddressData addrA = new AddressData();
            addrA.Address = householdersNotInMap.First().Address + ", " + householdersNotInMap.First().Neighbourhood;
            addrA.City = householdersNotInMap.First().City;

            for (int i = 1; i < householdersNotInMap.Count; i++)
            {
                if (i == (householdersNotInMap.Count - 1) || newTerritoryMap.Householders.Count >= territoryMapViewModel.MaxNumberOfHouseholders)
                {
                    _context.TerritoryMaps.Add(newTerritoryMap);
                    newTerritoryMap = new TerritoryMap();
                    newTerritoryMap.Householders = new List<Householder>();

                    if(i < (householdersNotInMap.Count - 1))
                    {
                        addrA = new AddressData();
                        addrA.Address = householdersNotInMap.First().Address + ", " + householdersNotInMap.First().Neighbourhood;
                        addrA.City = householdersNotInMap.First().City;
                    }
                }

                AddressData addrB = new AddressData();
                addrB.Address = householdersNotInMap[i].Address + ", " + householdersNotInMap[i].Neighbourhood;
                addrB.City = householdersNotInMap[i].City;

                var distance = locationService.GetDirections(addrA, addrB).Distance.Split(' ')[0].Replace('.', ',');

                if (Double.Parse(distance) <= territoryMapViewModel.MaxDistanceAmongHouseholders)
                {
                    newTerritoryMap.Householders.Add(householdersNotInMap[i]);
                    householdersNotInMap.RemoveAt(i);
                    newTerritoryMap.CreationDate= DateTime.Now;
                }
            }

            _context.SaveChanges();

            return List();
        }

        public ActionResult List()
        {
            var territoryMapsInDB = _context.TerritoryMaps.ToList().Where(t => t.CreationDate.Date == DateTime.Today).ToList();

            _context.TerritoryMaps.RemoveRange(_context.TerritoryMaps.ToList().Where(t => t.CreationDate.Date == DateTime.Today));
            _context.SaveChanges();

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

        // GET: TerritoryMaps/Delete/5
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
