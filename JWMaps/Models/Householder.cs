using GoogleMaps.LocationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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

        [Required]
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


        [Display(Name = "CEP")]
        public string ZipCode { get; set; }

        public Category Category { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        [MaxLength(500)]
        [Display(Name = "Observações")]
        public string Observations { get; set; }

        [Required]
        public int CongregationId { get; set; }

        public DateTime CreationDate { get; set; }

        public List<Visit> Visits { get; set; }

        public TerritoryMap TerritoryMap { get; set; }

        public Publisher Publisher { get; set; }

        public AddressData GetAddress()
        {
            AddressData householderAddres = new AddressData();
            householderAddres.Address = this.Address + ", " + this.Neighbourhood;
            householderAddres.City = this.City;

            return householderAddres;
        }

        public DateTime? LastTimeVisited()
        {
            if (Visits != null)
            {
                if (Visits.Count > 0)
                {
                    var lastVisit = Visits.First();
                    foreach(var visit in Visits)
                    {
                        if (visit.DateOfVisit > lastVisit.DateOfVisit)
                            lastVisit = visit;
                    }

                    return lastVisit.DateOfVisit;
                }
            }

            return null;
        }

    }

    public enum Category
    {
        [Display(Name = "Residência")]
        House,
        [Display(Name = "Comercial")]
        Business
    };
}