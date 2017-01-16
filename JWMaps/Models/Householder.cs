using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JWMaps.Models
{
    public class Householder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Neighbourhood { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public int PublisherId { get; set; }
    }
}