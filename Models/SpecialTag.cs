using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStoreMVC.Models
{
    public class SpecialTag
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name field cannot be empty")]
        [MinLength(2, ErrorMessage = "Minimum length is 2 characters")]
        [MaxLength(30, ErrorMessage = "Maximum length is 30 characters")]
        public string Name { get; set; }
    }
}
