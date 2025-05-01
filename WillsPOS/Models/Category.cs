using System.ComponentModel.DataAnnotations;

namespace WillsPOS.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; }

        // Connection to MenuItem
        public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();        
    }
}
