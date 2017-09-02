using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace JWMaps.Models
{
    public class Publisher
    {
        public int Id { get; set; }

        [Required]
        [Display(Name="Nome")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int CongregationId { get; set; }
    }
}