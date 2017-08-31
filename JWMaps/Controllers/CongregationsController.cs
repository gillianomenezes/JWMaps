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
            return View("ListCongregations",_context.Congregations.ToList());
        }

        public ActionResult Create()
        {
            return View("CongregationsForm");
        }

        //public ActionResult New()
        //{
        //    Congregation congregation = new Congregation();

        //    return("")
        //}
    }
}