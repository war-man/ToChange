using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ToChange.Models
{
    public class Poke
    {
        [Key]
        [Required]
        public int ID { set; get; }

        [Required]
        public string firstCustomerID { set; get; }

        [Required]
        public string secondCustomerID { set; get; }

        [Required]
        public int ProductID { set; get; }

        [Required]
        public DateTime AddedOn { set; get; }
        public DateTime RespondAt {set;get;}

        public bool Pending  {set;get;}
        public bool Accepted { set; get; }

        public int ProductIDToSwapWith { set; get; }
         


    }
}