using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WillsPOS.Models
{
    public class MenuItem
    {
        [Key]
        public int MenuItemId { get; set; }

        [Required]
        [StringLength(100)]
        public string MenuName { get; set; }
        [Range(0, 9999)]
        public decimal Price { get; set; }

        // Navigation to category
        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }  

    }
}
