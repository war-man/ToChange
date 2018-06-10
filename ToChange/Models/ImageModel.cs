using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToChange.Models
{
    public class ImageModel
    {
        public HttpPostedFileWrapper ImageFile
        {
            set; get;
        }
    }
}