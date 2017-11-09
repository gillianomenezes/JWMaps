using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JWMaps.ViewModel;
using JWMaps.Models;

namespace JWMaps.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;

        public HomeController()
        {
            _context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            if(User.IsInRole(RoleName.CanCreateTerritoryMap))
            {
                return RedirectToAction("TerritoryMaps", "Index");
            }

            DashboardViewModel dashboardViewModel = new DashboardViewModel
            {
                Householders = _context.Householders.ToList()
            };

            return View(dashboardViewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}