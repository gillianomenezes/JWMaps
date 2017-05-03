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

            //var territoryMap = _context.TerritoryMaps.Single(t => t.Id == id);

            //if(territoryMap == null)
            //    return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            //return View("TerritoryMapView", territoryMap);

            var householders = _context.Householders.Where(h => h.TerritoryMapId == id).ToList();

            return View("TerritoryMapView", householders);
        }

        public ActionResult New(TerritoryMapViewModel territoryMapViewModel)
        {
            //var householdersNotInMap = new List<Householder>();
            var locationService = new GoogleLocationService();
            var peopleNotInMap = _context.Householders
                                            .ToList()
                                            .Where(h => h.Neighbourhood.Equals(territoryMapViewModel.selectedNeighbourhood))
                                            .Where(h => h.TerritoryMapId == 0 || h.TerritoryMapId == null)
                                            .ToList();

            if (peopleNotInMap.Count == 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            while (peopleNotInMap.Count > 0)
            {
                TerritoryMap newMap = new TerritoryMap();

                _context.TerritoryMaps.Add(newMap);
                _context.SaveChanges();

                peopleNotInMap.First().TerritoryMapId = newMap.Id;
                int peopleAdded = 1;

                AddressData addrA = new AddressData();
                addrA.Address = peopleNotInMap.First().Address + ", " + peopleNotInMap.First().Neighbourhood;
                addrA.City = peopleNotInMap.First().City;

                for (int i = 0; i < peopleNotInMap.Count; i++)
                {
                    if (peopleAdded >= territoryMapViewModel.MaxNumberOfHouseholders || (i == peopleNotInMap.Count - 1))
                    {
                        peopleAdded = 0;
                        break;
                    }

                    if (peopleNotInMap[i] == peopleNotInMap.First())
                        continue;

                    AddressData addrB = new AddressData();
                    addrB.Address = peopleNotInMap[i].Address + ", " + peopleNotInMap[i].Neighbourhood;
                    addrB.City = peopleNotInMap[i].City;

                    var distance = locationService.GetDirections(addrA, addrB).Distance.Split(' ')[0].Replace('.', ',');

                    if (Double.Parse(distance) <= territoryMapViewModel.MaxDistanceAmongHouseholders)
                    {
                        peopleNotInMap[i].TerritoryMapId = newMap.Id;
                        peopleAdded++;
                        peopleNotInMap.Remove(peopleNotInMap[i]);
                    }
                }

                peopleNotInMap.Remove(peopleNotInMap.First());
            }

            _context.SaveChanges();

            return List();
        }

        public ActionResult List()
        {
            var territoryMapsInDB = _context.TerritoryMaps.ToList().Where(t => t.CreationDate.Date == DateTime.Today).ToList();
            
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
