using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Coza.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Display(Name="Name of category")]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
