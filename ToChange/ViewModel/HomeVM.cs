using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToChange.Models;

namespace ToChange.ViewModel
{
    public class HomeVM
    {
        public IPagedList<Product> Products { set; get; }
    }
}