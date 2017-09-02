using JWMaps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JWMaps.Controllers
{
    public class CongregationsController : Controller
    {
        private ApplicationDbContext _context;

        public CongregationsController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View("ListCongregations", _context.Congregations.ToList());
        }

        public ActionResult New()
        {
            Congregation congregation = new Congregation();
            return View("CongregationsForm", congregation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Congregation congregation)
        {   
            if(!ModelState.IsValid)            
                return View("CongregationsForm", congregation);
            
            if(congregation.Id == 0)
            {
                _context.Congregations.Add(congregation);
            }
            else
            {
                var congregationdb = _context.Congregations.Single(c => c.Id == congregation.Id);

                congregationdb.Name = congregation.Name;
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Congregations");
        }

        public ActionResult Edit(int id)
        {
            var congregation = _context.Congregations.SingleOrDefault(c => c.Id == id);

            if (congregation == null)
                return HttpNotFound();

            return View("CongregationsForm", congregation);
        }

        public ActionResult Delete(int id)
        {
            var congregationdb = _context.Congregations.SingleOrDefault(c => c.Id == id);

            if (congregationdb == null)
                return HttpNotFound();

            _context.Congregations.Remove(congregationdb);

            _context.SaveChanges();

            return RedirectToAction("Index", "Congregations");
        }
    }
}