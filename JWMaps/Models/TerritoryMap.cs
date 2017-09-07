using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace JWMaps.Models
{
    public class TerritoryMap
    {
        public int Id { get; set; }

        public DateTime CreationDate { get; set; }

        public List<Householder> Householders { get; set; }

        [Required]
        public int CongregationId { get; set; }

        [Required]
        public string UserId { get; set; }

        public string Neighbourhood { get; set; }

        public TerritoryMap()
        {
            this.CreationDate = DateTime.Now;
        }
    }
}