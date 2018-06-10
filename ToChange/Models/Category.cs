using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ToChange.Models
{
    public class Category
    {

        [Key]
        [Required]
        public int ID { set; get; }

        [Required]
        public string CategoryName { set; get; }
    }
}