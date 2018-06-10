using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ToChange.Models
{
    public class History
    {
        [Key]
        [Column(Order = 0)]
        [Required]
        public string CustomerID { set; get; }

        [Key]
        [Column(Order = 1)]
        [Required]
        public int ExchangeID { set; get; }
    }
}