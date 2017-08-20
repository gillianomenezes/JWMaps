using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JWMaps.Models
{
    public class TerritoryMap
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? SharedDate { get; set; }
        public IEnumerable<Householder> Householders { get; set; }

        public TerritoryMap()
        {
            this.CreationDate = DateTime.Now;
        }
    }
}