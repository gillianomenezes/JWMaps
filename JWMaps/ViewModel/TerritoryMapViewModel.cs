using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JWMaps.Models;

namespace JWMaps.ViewModel
{
    public class TerritoryMapViewModel
    {
        public TerritoryMap TerritoryMap { get; set; }
        public IEnumerable<string> Neighbourhoods { get; set; }
    }
}