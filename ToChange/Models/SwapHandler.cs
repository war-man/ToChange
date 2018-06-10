using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToChange.Models
{
    public class SwapHandler
    {

        public int ID { set; get; }
        public string firstCustomerID { set; get; }
        public string secondCustomerID { set; get; }

        public int firstProductID { set; get; }
        public int secondProductID { set; get; }

    }
}