using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JWMaps.Models;
using JWMaps.ViewModel;

namespace JWMaps.Controllers
{
    public class HouseholdersController : Controller
    {        
        // GET: Householder
        public ActionResult Index()
        {
            return View("ListHouseholders");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Householder householder)
        {
            return View();
        }

        public ActionResult New()
        {
            var householderViewModel = new HouseholderViewModel
            {
                Householder = new Householder(),
                Publishers = new List<Publisher>()
            };
            return View("HouseholdersForm", householderViewModel);
        }
    }
}