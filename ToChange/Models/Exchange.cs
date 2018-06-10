using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ToChange.Models
{
    public class Exchange
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [Required]
        public string FirstCustomerID { get; set; }

        [Required]
        public int FirstProductID { get; set; }

        [Required]
        public string FirstDescription { get; set; }

        [Required]
        public string SecondCustomerID { get; set; }

        [Required]
        public int SecondProductID { get; set; }

        [Required]
        public string SecondDescription { get; set; }
    }
}