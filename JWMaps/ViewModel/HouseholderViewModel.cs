using System.Collections.Generic;
using JWMaps.Models;

namespace JWMaps.ViewModel
{
    public class HouseholderViewModel
    {
        public IEnumerable<Publisher> Publishers { get; set; }
        public Householder Householder { get; set; }
    }
}