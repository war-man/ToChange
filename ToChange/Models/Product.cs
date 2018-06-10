using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ToChange.Models
{
    public class Product
    {
        [Key]
        [Required]
        public int ID { set; get; }

        [Display(Name = "Product")]
        [Required]
        public string ProductName { set; get; }

        [Display(Name = "Category")]
        [Required]
        public int CategoryID { set; get; }

        [Display(Name = "Customer ID")]
        [Required]
        public string CustomerID { set; get; }

        [Required]
        public string Description { set; get; }

        [Display(Name = "Image")]
        [Required]
        public string ProductImage { set; get; }
    }
}