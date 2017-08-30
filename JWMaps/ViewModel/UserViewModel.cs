using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JWMaps.Models;

namespace JWMaps.ViewModel
{
    public class UserViewModel
    {
        public RegisterViewModel RegisterViewModel { get; set; }

        public IEnumerable<Congregation> Congregations { get; set; }
    }
}