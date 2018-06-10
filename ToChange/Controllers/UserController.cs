using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToChange.DB_Connections;
using ToChange.Models;
using ToChange.ViewModel;

namespace ToChange.Controllers
{
    public class UserController : Controller
    {

        UserVM userVM = new UserVM();

        /// <summary>
        /// Set Variables to the Tables and return the model
        /// </summary>
        /// <returns></returns>
        public ActionResult Home()
        {

            userVM.Products = DAL.Products.Where(a => !a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.ProductName).ToPagedList(1, 7);
            userVM.MyProducts = DAL.Products.Where(a => a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.ProductName).ToPagedList(1, 7);
            userVM.Exchanges = DAL.Exchanges.Where(a => a.FirstCustomerID.Equals(websiteSettings.CurrentUser.ID) || a.SecondCustomerID.Equals(websiteSettings.CurrentUser.ID))
                .OrderBy(a => a.ID).ToPagedList(1, 7);

            return View(userVM);

        }

        /// <summary>
        /// Exchange Table
        /// </summary>
        /// <returns></returns>
        public ActionResult Exchanges()
        {
            return View(DAL.Exchanges.Where(a => a.FirstCustomerID.Equals(websiteSettings.CurrentUser.ID) || a.SecondCustomerID.Equals(websiteSettings.CurrentUser.ID))
                    .OrderBy(a => a.ID).ToPagedList(1, 7));
        }

        /// <summary>
        /// Search in MyProducts Table
        /// </summary>
        /// <param name="txtSearch"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchMyProducts(string txtSearch)
        {
            if (string.IsNullOrEmpty(txtSearch))
                userVM.MyProducts = DAL.Products.Where(a => a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.ProductName).ToPagedList(1, 7);
            else
                userVM.MyProducts = DAL.Products.Where(a => a.CustomerID.Equals(websiteSettings.CurrentUser.ID) &&
                    a.ProductName.ToLower().Contains(txtSearch.ToLower())).OrderBy(a => a.ProductName).ToPagedList(1, 7);

            ModelState.Clear();

            userVM.Products = DAL.Products.Where(a => !a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.ProductName).ToPagedList(1, 7);
            userVM.Exchanges = DAL.Exchanges.Where(a => a.FirstCustomerID.Equals(websiteSettings.CurrentUser.ID) || a.SecondCustomerID.Equals(websiteSettings.CurrentUser.ID))
                .OrderBy(a => a.ID).ToPagedList(1, 7);

            return View("Home", userVM);
        }

        /// <summary>
        /// Search in Products Table
        /// </summary>
        /// <param name="txtSearch"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchProducts(string txtSearch)
        {
            if (string.IsNullOrEmpty(txtSearch))
                userVM.Products = DAL.Products.Where(a => !a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.ProductName).ToPagedList(1, 7);
            else
                userVM.Products = DAL.Products.Where(a => !a.CustomerID.Equals(websiteSettings.CurrentUser.ID) &&
                    a.ProductName.ToLower().Contains(txtSearch.ToLower())).OrderBy(a => a.ProductName).ToPagedList(1, 7);

            ModelState.Clear();

            userVM.MyProducts = DAL.Products.Where(a => a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.ProductName).ToPagedList(1, 7);
            userVM.Exchanges = DAL.Exchanges.Where(a => a.FirstCustomerID.Equals(websiteSettings.CurrentUser.ID) || a.SecondCustomerID.Equals(websiteSettings.CurrentUser.ID))
                .OrderBy(a => a.ID).ToPagedList(1, 7);

            return View("Home", userVM);
        }

        /// <summary>
        /// Search in Exchanges Table
        /// </summary>
        /// <param name="txtSearch"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchExchanges(string txtSearch)
        {
            if (string.IsNullOrEmpty(txtSearch))
                userVM.Exchanges = DAL.Exchanges.Where(a => a.FirstCustomerID.Equals(websiteSettings.CurrentUser.ID) || a.SecondCustomerID.Equals(websiteSettings.CurrentUser.ID))
                    .OrderBy(a => a.ID).ToPagedList(1, 7);
            else
                userVM.Exchanges = DAL.Exchanges.Where(a => (a.FirstCustomerID.Equals(websiteSettings.CurrentUser.ID) || a.SecondCustomerID.Equals(websiteSettings.CurrentUser.ID))
                && (websiteSettings.GetProductName(a.FirstProductID).ToLower().Contains(txtSearch.ToLower()) || websiteSettings.GetProductName(a.SecondProductID).ToLower().Contains(txtSearch.ToLower())))
                .OrderBy(a => a.ID).ToPagedList(1, 7);

            ModelState.Clear();

            userVM.Products = DAL.Products.Where(a => !a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.ProductName).ToPagedList(1, 7);
            userVM.MyProducts = DAL.Products.Where(a => a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.ProductName).ToPagedList(1, 7);

            return View("Home", userVM);
        }


        [HttpPost]
        public ActionResult Exchanges(string txtSearch)
        {
            if (string.IsNullOrEmpty(txtSearch))
                userVM.Exchanges = DAL.Exchanges.Where(a => a.FirstCustomerID.Equals(websiteSettings.CurrentUser.ID) || a.SecondCustomerID.Equals(websiteSettings.CurrentUser.ID))
                    .OrderBy(a => a.ID).ToPagedList(1, 7);
            else
                userVM.Exchanges = DAL.Exchanges.Where(a => (a.FirstCustomerID.Equals(websiteSettings.CurrentUser.ID) || a.SecondCustomerID.Equals(websiteSettings.CurrentUser.ID))
                && (websiteSettings.GetProductName(a.FirstProductID).ToLower().Contains(txtSearch.ToLower()) || websiteSettings.GetProductName(a.SecondProductID).ToLower().Contains(txtSearch.ToLower())))
                .OrderBy(a => a.ID).ToPagedList(1, 7);
            return View(userVM);
        }

        /// <summary>
        /// Get Specific Page Data
        /// </summary>
        /// <param name="page">which page you want to attach</param>
        /// <param name="tableID">which table you want to change</param>
        /// <returns></returns>
        public ActionResult Pagination(int? page, int tableID)
        {
            if (tableID == websiteSettings.PRODUCTS_TABLE_ID)
            {
                userVM.Products = DAL.Products.Where(a => !a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.ProductName).ToPagedList(page ?? 1, 7);
                userVM.MyProducts = DAL.Products.Where(a => a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.ProductName).ToPagedList(1, 7);
                userVM.Exchanges = DAL.Exchanges.Where(a => a.FirstCustomerID.Equals(websiteSettings.CurrentUser.ID) || a.SecondCustomerID.Equals(websiteSettings.CurrentUser.ID))
                    .OrderBy(a => a.ID).ToPagedList(1, 7);
            }

            if (tableID == websiteSettings.MYPRODUCTS_TABLE_ID)
            {
                userVM.Products = DAL.Products.Where(a => !a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.ProductName).ToPagedList(1, 7);
                userVM.MyProducts = DAL.Products.Where(a => a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.ProductName).ToPagedList(page ?? 1, 7);
                userVM.Exchanges = DAL.Exchanges.Where(a => a.FirstCustomerID.Equals(websiteSettings.CurrentUser.ID) || a.SecondCustomerID.Equals(websiteSettings.CurrentUser.ID))
                    .OrderBy(a => a.ID).ToPagedList(1, 7);
            }

            if (tableID == websiteSettings.EXCHANGES_TABLE_ID)
            {
                userVM.Products = DAL.Products.Where(a => !a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.ProductName).ToPagedList(1, 7);
                userVM.MyProducts = DAL.Products.Where(a => a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.ProductName).ToPagedList(1, 7);
                userVM.Exchanges = DAL.Exchanges.Where(a => a.FirstCustomerID.Equals(websiteSettings.CurrentUser.ID) || a.SecondCustomerID.Equals(websiteSettings.CurrentUser.ID))
                    .OrderBy(a => a.ID).ToPagedList(page ?? 1, 7);
            }


            return View("Home", userVM);
        }

        /// <summary>
        /// Get Notification to Push In Notification View
        /// </summary>
        /// <returns></returns>
        public JsonResult GetNotification()
        {
            var notificationRegisterTime = Session["LastUpdated"] != null
                ? Convert.ToDateTime(Session["LastUpdated"]) : DateTime.Now;

            Notification_Adapter.NotificationComponent NC = new Notification_Adapter.NotificationComponent();
            var list = NC.GetAllPokes(notificationRegisterTime, websiteSettings.CurrentUser.ID);
            
            List<List<string>> notificationList = new List<List<string>>();

            foreach (var item in list)
            {
                List<string> currentList = new List<string>();
                var currentCustomer = DAL.Customers.Find(item.firstCustomerID);
                var currentProduct = DAL.Products.Find(item.ProductID);
                var currentTime = item.AddedOn.ToString();
                int itemIDforSwap = item.ProductIDToSwapWith;


                currentList.Add(currentCustomer == null ? "" : currentCustomer.FirstName + " " + currentCustomer.LastName);
                currentList.Add(item.ProductIDToSwapWith == 0 ? "want to swap an item with you" : "accepted your request and ask for an item");
                currentList.Add(item.ProductIDToSwapWith == 0 ? currentProduct.ProductName : websiteSettings.GetProductName(item.ProductIDToSwapWith));
                currentList.Add(item.ProductIDToSwapWith == 0 ? currentTime : item.RespondAt.ToString());
                currentList.Add("/Exchange/OpenRequestInfo?id=" + currentProduct.ID + "&&requestID=" + currentCustomer.ID);

                notificationList.Add(currentList);
            }

            Session["LastUpdate"] = DateTime.Now;
            return new JsonResult { Data = notificationList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


    }
}