using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JWMaps.Models;

namespace JWMaps.ViewModel
{
    public class TerritoryStatistics
    {
        public IEnumerable<Householder> householders { get; set; }
    }
}