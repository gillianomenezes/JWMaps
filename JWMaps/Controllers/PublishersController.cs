using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JWMaps.Models;

namespace JWMaps.Controllers
{
    public class PublishersController : Controller
    {
        public ApplicationDbContext _context { get; set; }

        public PublishersController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Publishers
        public ActionResult Index()
        {
            var publishers = _context.Publishers.ToList();
            return View("ListPublishers", publishers);
        }

        public ActionResult Save(Publisher publisher)
        {
            if(!ModelState.IsValid)
            {
                return View("PublishersForm", publisher);
            }
            else
            {
                if(publisher.Id == 0)
                {
                    _context.Publishers.Add(publisher);
                }
                else
                {
                    var publisherdb = _context.Publishers.Single(p => p.Id == publisher.Id);
                    publisherdb.Name = publisher.Name;
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Publishers");
        }

        public ActionResult New()
        {
            var publisher = new Publisher();
            return View("PublishersForm", publisher);
        }
    }
}