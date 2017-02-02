using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using JWMaps.Models;
using JWMaps.ViewModel;
using System;

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

            List<Householder> householders = _context.Householders
                                        .ToList()
                                        .FindAll(h => h.Neighbourhood.Equals(selectedNeighbourhood))
                                        .OrderBy(h => h.LasTimeIncludedInTerritoryMap).ToList();

            //here I have to calculate the distances and put on the list only those who are near

            return View("TerritoryMapView", householders);
        }

        private decimal calcDistance(decimal latA, decimal longA, decimal latB, decimal longB)
        {
            double theDistance = Math.Sin(Convert.ToDouble(DegreesToRadians(latA))) *
                    Math.Sin(Convert.ToDouble(DegreesToRadians(latB))) +
                    Math.Cos(Convert.ToDouble(DegreesToRadians(latA))) *
                    Math.Cos(Convert.ToDouble(DegreesToRadians(latB))) *
                    Math.Cos(Convert.ToDouble(DegreesToRadians(longA - longB)));

            return Convert.ToDecimal(DecRadiansToDegrees(Math.Acos(theDistance))) * 69.09M * 1.6093M;
        }

        private static decimal DegreesToRadians(decimal degrees)
        {
            decimal radians = Convert.ToDecimal(Math.PI / 180) * degrees;
            return (radians);
        }

        private double DecRadiansToDegrees(double angle)
        {
            return angle * 180.0 / Math.PI;
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
