using System.ComponentModel.DataAnnotations;

namespace CoreStoreMVC.Models
{
    public class ProductsType
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name field cannot be empty")]
        [MinLength(2, ErrorMessage = "Minimum length is 2 characters")]
        [MaxLength(30, ErrorMessage = "Maximum length is 30 characters")]
        [Display(Name = "Product Type")]
        public string Name { get; set; }
    }
}
