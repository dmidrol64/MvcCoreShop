using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStoreMVC.Models.ViewModel
{
    // Урок 10
    public class ShoppingCartViewModel
    {
        public List<Product> Products { get; set; }
        public Appointment Appointment { get; set; }
    }
}
