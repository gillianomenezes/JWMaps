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
            var householders = new List<Householder>
            {
                new Householder
                {
                    Name = "Juan gonzalez",
                    Address = "Avenida Boa viagem, 455"
                },

                new Householder
                {
                    Name = "María Lopez",
                    Address = "Avenida 17 de agosto, 1056"
                }
            };

            var houseHolderViewModel = new HouseholderViewModel
            {
                Householders = householders
            };

            return View("ListHouseholders", houseHolderViewModel);
        }
    }
}