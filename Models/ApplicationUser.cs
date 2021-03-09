using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreStoreMVC.Models
{

    public class ApplicationUser:IdentityUser
    {
       [MaxLength(30)]
       [Display(Name = "Sales person")]
       public string Name { get; set; }

       [NotMapped]
       public bool IsSuperAdmin { get; set; }
    }
}

