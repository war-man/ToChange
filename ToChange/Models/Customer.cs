using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ToChange.Models
{
    public class Customer
    {
        [Key]
        [Required]
        public string ID { set; get; }

        [Display(Name = "First Name")]
        [Required]
        public string FirstName { set; get; }

        [Display(Name = "Last Name")]
        [Required]
        public string LastName { set; get; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { set; get; }

        [Required]
        public string Email { set; get; }

        [Required]
        public string City { set; get; }

        [Required]
        public string Phone { set; get; }
    }
}