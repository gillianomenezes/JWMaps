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

        public virtual ICollection<Householder> Householders { get; set; }
    }
}