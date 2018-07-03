using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace JWMaps.Models
{
    public class Visit : IComparable<Visit>
    {
        [Required]
        public int Id { get; set; }

        public string PublisherName { get; set; }

        public DateTime DateOfVisit { get; set; }

        public string Description { get; set; }

        public int CompareTo(Visit other)
        {
            if (other == null)
                return 1;
            else
                return this.DateOfVisit.CompareTo(other.DateOfVisit);
        }

        //public int CongregationId { get; set; }
    }
}