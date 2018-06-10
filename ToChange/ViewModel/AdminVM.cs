using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToChange.Models;

namespace ToChange.ViewModel
{
    public class AdminVM
    {
        public IPagedList<Product> Products { set; get; }

        public IPagedList<Customer> Customers { set; get; }

        public IPagedList<Exchange> Exchanges { set; get; }
    }
}