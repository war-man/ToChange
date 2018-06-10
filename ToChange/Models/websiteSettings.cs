using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToChange.DB_Connections;
using ToChange.ViewModel;

namespace ToChange.Models
{
    public class websiteSettings
    {
        public static bool hasLoggedIn = false;
        public static bool isFirstTime = true;
        public static User UserInSystem = null;
        public static Customer CurrentUser = null;
        public static HomeVM homeVm = null;
        public static UserVM userVm = null;
        public static AdminVM adminVm = null;
        public static Customer firstCustomer = null;
        public static Customer secondCustomer = null;


        public static int productIDforSwap = -1;
        public static string customerIDforSwap = "";
        public static DateTime DEFAULT_TIME = new DateTime(2000, 1, 1, 0, 0, 0);

        public const int NONE = -1;
        public const int ADD_NEW_PRODUCT = 1;
        public const int EDIT_PRODUCT = 2;
        public const int DELETE_PRODUCT = 3;
        public const int VIEW_SINGLE_IMAGE = 4;
        public const int EDIT_CUSTOMER = 5;
        public const int VIEW_CUSTOMER = 6;
        public const int DELETE_CUSTOMER = 7;
        public const int ADD_NEW_CUSTOMER = 8;
        public const int SEND_POKE_REQUEST = 9;
        public const int RE_SEND_POKE_REQUEST = 10;


        public const int PRODUCTS_TABLE_ID = 1;
        public const int MYPRODUCTS_TABLE_ID = 2;
        public const int EXCHANGES_TABLE_ID = 3;
        public const int CUSTOMERS_TABLE_ID = 4;

        public static void ResetAll()
        {
            hasLoggedIn = false;
            isFirstTime = true;
            UserInSystem = null;
            CurrentUser = null;
            homeVm = null;
            userVm = null;
            adminVm = null;
        }

         public static string GetCategoryName(int categoryID)
        {
            return DAL.Categories.Find(categoryID).CategoryName;
        }

        public static string GetCustomerName(string customerID)
        {
            return DAL.Customers.Find(customerID).FirstName;
        }

        public static string GetProductName(int productID)
        {
            return DAL.Products.Find(productID).ProductName;
        }

        public static List<SelectListItem> GetDropDownList()
        {
            var SelectionList = new List<SelectListItem>();

            foreach(var item in DAL.Categories)
            {
                SelectionList.Add(new SelectListItem{

                    Text = item.CategoryName, Value = item.ID.ToString()
                });
            }

            return SelectionList;
        }

        public static int GetNotificationCount()
        {
            var poke = DAL.Pokes.Where(a => a.secondCustomerID.Equals(CurrentUser.ID)).ToList();
            return poke.Count;
        }

        public static bool IsResendedRequest(string senderID , string receiverID , int productID)
        {
            var product = DAL.Pokes.Where(a => a.ProductID.Equals(productID)).FirstOrDefault();

            if (product == null)
                return false;
            if (product.RespondAt.Equals(DEFAULT_TIME))
                return false;
            return true;
        }

        public static Poke GetPokeList(string senderID, string receiverID, int productID)
        {
            return DAL.Pokes.Where(a => a.firstCustomerID.Equals(senderID) && a.secondCustomerID.Equals(receiverID) && a.ProductID.Equals(productID)).FirstOrDefault();
        }

    }
}