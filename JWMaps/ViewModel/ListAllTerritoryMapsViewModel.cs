using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JWMaps.Models;

namespace JWMaps.ViewModel
{
    public class ListAllTerritoryMapsViewModel
    {
        public List<TerritoryMap> TerritoryMaps { get; set; }

        public ApplicationUser User { get; set; }
    }
}