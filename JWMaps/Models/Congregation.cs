using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace JWMaps.Models
{
    public class Congregation
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Congregação")]
        public string Name { get; set; }
    }
}