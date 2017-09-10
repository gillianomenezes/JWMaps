using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JWMaps.Models;

namespace JWMaps.ViewModel
{
    public class VisitViewModel
    {
        public Householder VisitedHouseHolder { get; set; }

        public TerritoryMap TerritoryMapUsed { get; set; }

        public List<Publisher> Publishers { get; set; }

        public string PublisherName { get; set; }

        public string VisitDescription { get; set; }
    }
}