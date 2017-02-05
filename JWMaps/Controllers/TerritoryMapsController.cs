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


        public ActionResult NewTerritoryMap(TerritoryMapViewModel territoryMapVM)
        {
            var selectedNeighbourhood = territoryMapVM.selectedNeighbourhood;

            List<Householder> householders = new List<Householder>();
            List<Householder> householdersdb = _context.Householders
                                        .ToList()
                                        .FindAll(h => h.Neighbourhood.Equals(selectedNeighbourhood))
                                        .OrderBy(h => h.LasTimeIncludedInTerritoryMap).ToList();

            //here I have to calculate the distances and put on the list only those who are near
            var locationService = new GoogleLocationService();
            //var pointA = locationService.GetLatLongFromAddress(householders[0].Address + ", " + householders[0].Neighbourhood + "-" + householders[0].City);
            //var pointB = locationService.GetLatLongFromAddress(householders[1].Address + ", " + householders[1].Neighbourhood + "-" + householders[1].City);

            householders.Add(householdersdb.First());

            AddressData addrA = new AddressData();
            addrA.Address = householdersdb[0].Address + ", " + householdersdb[0].Neighbourhood;
            addrA.City = householdersdb[0].City;

            for (int i = 1; i < householdersdb.Count ; i++)
            {
                if (householders.Count >= territoryMapVM.MaxNumberOfHouseholders)
                    break;

                AddressData addrB = new AddressData();
                addrB.Address = householdersdb[i].Address + ", " + householdersdb[i].Neighbourhood;
                addrB.City = householdersdb[i].City;

                var distance = locationService.GetDirections(addrA, addrB).Distance.Split(' ')[0].Replace('.', ',');

                if (Double.Parse(distance) <= territoryMapVM.MaxDistanceAmongHouseholders)
                    householders.Add(householdersdb[i]);                
            }
            
            return View("TerritoryMapView", householders);
        }

        // GET: TerritoryMaps/Edit/5
        public ActionResult Edit(int? id)
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
