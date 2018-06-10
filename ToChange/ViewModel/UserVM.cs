using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToChange.Models;

namespace ToChange.ViewModel
{
    public class UserVM
    {
        public IPagedList<Product> Products { set; get; }
        public IPagedList<Product> MyProducts { set; get; }
        public IPagedList<Exchange> Exchanges { set; get; }

        public Product recieverProduct { set; get; }
        public Customer senderCustomer { set; get; }
        public Customer recieverCustomer { set; get; }
        public Product senderProduct { set; get; }
    }
}