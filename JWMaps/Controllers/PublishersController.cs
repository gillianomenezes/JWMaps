using JWMaps.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JWMaps.Controllers
{
    public class PublishersController : Controller
    {
        private ApplicationDbContext _context;

        public PublishersController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            return View("ListPublishers", _context.Publishers.ToList());
        }

        public ActionResult New()
        {
            Publisher publisher = new Publisher();
            return View("PublishersForm", publisher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Publisher publisher)
        {
            if (!ModelState.IsValid)
                return View("PublishersForm", publisher);

            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var userManager = new UserManager<ApplicationUser>(store);
            ApplicationUser user = userManager.FindByNameAsync(User.Identity.Name).Result;
            publisher.CongregationId = user.CongregationId;

            if (publisher.Id == 0)
            {
                _context.Publishers.Add(publisher);
            }
            else
            {
                var publisherdb = _context.Publishers.Single(c => c.Id == publisher.Id);

                publisherdb.Name = publisher.Name;
                publisherdb.CongregationId = publisher.CongregationId;

            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Publishers");
        }

        public ActionResult Edit(int id)
        {
            var publisher = _context.Publishers.SingleOrDefault(c => c.Id == id);

            if (publisher == null)
                return HttpNotFound();

            return View("PublishersForm", publisher);
        }

        public ActionResult Delete(int id)
        {
            var publisherdb = _context.Publishers.SingleOrDefault(c => c.Id == id);

            if (publisherdb == null)
                return HttpNotFound();

            var publisherStudends = _context.Householders.Where(h => h.PublisherId == id);

            foreach (var student in publisherStudends)
                student.PublisherId = null;

            _context.Publishers.Remove(publisherdb);

            _context.SaveChanges();

            return RedirectToAction("Index", "Publishers");
        }
    }
}