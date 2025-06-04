using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [DisplayName("Category Name")]
        [Required(ErrorMessage="Category Name is required")]        
        [MaxLength(50)]
        public required string Name { get; set; }
        [DisplayName("Display Order")]
        [Required(ErrorMessage = "Display Order is required")]
        [Range(1,100, ErrorMessage ="Display Order must be between 1-100")]
        public int DisplayOrder { get; set; }
    }
}
