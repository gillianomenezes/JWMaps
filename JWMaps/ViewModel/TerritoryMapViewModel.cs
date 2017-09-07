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
        public IEnumerable<Householder> Householders { get; set; }
        public IEnumerable<string> Neighbourhoods { get; set; }
        public string selectedNeighbourhood { get; set; }
        public int MaxNumberOfHouseholders { get; set; }
        public int MaxDistanceAmongHouseholders { get; set; }
        public Category Category { get; set; }
    }
}