using System;
using System.ComponentModel.DataAnnotations;

namespace JWMaps.Models
{
    public class Householder
    {
        public int Id { get; set; }

        [Display(Name = "Nome")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Endereço")]
        [StringLength(500)]
        public string Address { get; set; }
        
        [Display(Name = "Bairro")]
        [StringLength(255)]
        public string Neighbourhood { get; set; }

        [Required]
        [Display(Name = "Cidade")]
        [StringLength(255)]
        public string City { get; set; }

        [Display(Name = "Telefone")]
        [StringLength(13)]
        public string Phone { get; set; }
                
        public bool IsStudying { get; set; }
        
        [Display(Name = "Que tipo de endereço?")]
        public Category Category { get; set; }
        
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTime DateAdded { get; set; }

        [Required]
        public int CongregationId { get; set; }
    }

    public enum Category { House, Business };
}