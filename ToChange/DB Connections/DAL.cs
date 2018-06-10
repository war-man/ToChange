using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ToChange.Models;

namespace ToChange.DB_Connections
{
    public class DAL
    {

        public static ToChangeDBModelEntities entity = new ToChangeDBModelEntities();

        public static DbSet<Customer> Customers { get { return entity.Customers; } }

        public static DbSet<Exchange> Exchanges { get { return entity.Exchanges; } }

        public static DbSet<Product> Products { get { return entity.Products; } }

        public static DbSet<User> Users { get { return entity.Users; } }

        public static DbSet<Category> Categories { get { return entity.Categories; } }
        
        public static DbSet<Poke> Pokes { get { return entity.Pokes; } }

        public static DbSet<SwapHandler> Swap { get { return entity.Swap; } }
    }
}