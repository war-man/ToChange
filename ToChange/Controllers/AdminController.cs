using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToChange.DB_Connections;
using ToChange.Models;
using ToChange.ViewModel;

namespace ToChange.Controllers
{
    public class AdminController : Controller
    {

        AdminVM adminVM = new AdminVM();

        /// <summary>
        /// Home Page - Admin Role
        /// </summary>
        /// <returns></returns>
        public ActionResult Home()
        {
            adminVM.Customers = DAL.Customers.Where(a => !a.ID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.FirstName).ToPagedList(1, 7);
            adminVM.Products = DAL.Products.OrderBy(a => a.ProductName).ToPagedList(1, 7);
            adminVM.Exchanges = DAL.Exchanges.OrderBy(a => a.ID).ToPagedList(1, 7);

            return View(adminVM);
        }

        public ActionResult Customers()
        {
            adminVM.Customers = DAL.Customers.Where(a => !a.ID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.FirstName).ToPagedList(1, 7);
            return View(adminVM);
        }

        public ActionResult SearchCustomerView(string txtSearch)
        {
            if (string.IsNullOrEmpty(txtSearch))
                adminVM.Customers = DAL.Customers.Where(a => !a.ID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.FirstName).ToPagedList(1, 7);
            else
                adminVM.Customers = DAL.Customers.Where(a => (!a.ID.Equals(websiteSettings.CurrentUser.ID))
                && (a.FirstName.ToLower().Contains(txtSearch.ToLower()) || a.LastName.ToLower().Contains(txtSearch.ToLower()))).OrderBy(a => a.FirstName).ToPagedList(1, 7);
            ModelState.Clear();

            return View("Customers", adminVM);
        }

        /// <summary>
        /// Search in Customers Table
        /// </summary>
        /// <param name="txtSearch"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchCustomer(string txtSearch)
        {
            if (string.IsNullOrEmpty(txtSearch))
                adminVM.Customers = DAL.Customers.Where(a => !a.ID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.FirstName).ToPagedList(1, 7);
            else
                adminVM.Customers = DAL.Customers.Where(a => (!a.ID.Equals(websiteSettings.CurrentUser.ID)) 
                && (a.FirstName.ToLower().Contains(txtSearch.ToLower()) || a.LastName.ToLower().Contains(txtSearch.ToLower()))).OrderBy(a => a.FirstName).ToPagedList(1, 7);
            ModelState.Clear();

            adminVM.Products = DAL.Products.OrderBy(a => a.ProductName).ToPagedList(1, 7);
            adminVM.Exchanges = DAL.Exchanges.OrderBy(a => a.ID).ToPagedList(1, 7);

            return View("Home", adminVM);
        }

        /// <summary>
        /// Search In Products Table
        /// </summary>
        /// <param name="txtSearch"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchProducts(string txtSearch)
        {
            if (string.IsNullOrEmpty(txtSearch))
                adminVM.Products = DAL.Products.OrderBy(a => a.ProductName).ToPagedList(1, 7);
            else
                adminVM.Products = DAL.Products.Where(a => a.ProductName.ToLower().Contains(txtSearch.ToLower())).OrderBy(a => a.ProductName).ToPagedList(1, 7);
            ModelState.Clear();

            adminVM.Customers = DAL.Customers.Where(a => !a.ID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.FirstName).ToPagedList(1, 7);
            adminVM.Exchanges = DAL.Exchanges.OrderBy(a => a.ID).ToPagedList(1, 7);

            return View("Home", adminVM);
        }

        /// <summary>
        /// Search in exchange table
        /// </summary>
        /// <param name="txtSearch"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SearchExchanges(string txtSearch)
        {
            if (string.IsNullOrEmpty(txtSearch))
                adminVM.Exchanges = DAL.Exchanges.OrderBy(a => a.ID).ToPagedList(1, 7);
            else
                adminVM.Exchanges = DAL.Exchanges.Where(a => (websiteSettings.GetProductName(a.FirstProductID).ToLower()
                .Contains(txtSearch.ToLower()) || websiteSettings.GetProductName(a.SecondProductID).ToLower().Contains(txtSearch.ToLower())))
                .OrderBy(a => a.ID).ToPagedList(1, 7);
            ModelState.Clear();

            adminVM.Customers = DAL.Customers.Where(a => !a.ID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.FirstName).ToPagedList(1, 7);
            adminVM.Products = DAL.Products.OrderBy(a => a.ProductName).ToPagedList(1, 7);

            return View("Home", adminVM);
        }

        /// <summary>
        /// Navigate to Specific Page in Specific Table
        /// </summary>
        /// <param name="page"></param>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public ActionResult Pagination(int? page, int tableID)
        {
            if (tableID == websiteSettings.PRODUCTS_TABLE_ID)
            {
                adminVM.Products = DAL.Products.OrderBy(a => a.ProductName).ToPagedList(page ?? 1, 7);
                adminVM.Customers = DAL.Customers.Where(a => !a.ID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.FirstName).ToPagedList(1, 7);
                adminVM.Exchanges = DAL.Exchanges.OrderBy(a => a.ID).ToPagedList(1, 7);

            }

            if (tableID == websiteSettings.CUSTOMERS_TABLE_ID)
            {
                adminVM.Customers = DAL.Customers.Where(a => !a.ID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.FirstName).ToPagedList(page ?? 1, 7);
                adminVM.Products = DAL.Products.OrderBy(a => a.ProductName).ToPagedList(1, 7);
                adminVM.Exchanges = DAL.Exchanges.OrderBy(a => a.ID).ToPagedList(1, 7);
            }

            if (tableID == websiteSettings.EXCHANGES_TABLE_ID)
            {
                adminVM.Customers = DAL.Customers.Where(a => !a.ID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.FirstName).ToPagedList(1, 7);
                adminVM.Products = DAL.Products.OrderBy(a => a.ProductName).ToPagedList(1, 7);
                adminVM.Exchanges = DAL.Exchanges.OrderBy(a => a.ID).ToPagedList(page ?? 1, 7);
            }


            return View("Home", adminVM);
        }

        /// <summary>
        /// Exchange Page
        /// </summary>
        /// <returns></returns>
        public ActionResult Exchanges()
        {
            return View(DAL.Exchanges.OrderBy(a => a.ID).ToPagedList(1, 7));
        }

        /// <summary>
        /// Get Customer By ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Customer GetCustomerByID(string id)
        {
            return DAL.Customers.Find(id);
        }

        /// <summary>
        /// Get Customer By Username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public Customer GetCustomerByUsername(string username)
        {
            return DAL.Customers.Where(a => a.Username.Equals(username)).FirstOrDefault();
        }

        /// <summary>
        /// Add Customer - Get Method
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddCustomer()
        {
            return PartialView();
        }

        /// <summary>
        /// Add Customer -  Post Method 
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                //make sure that the ID and Username is Valid (Not token)
                if (GetCustomerByID(customer.ID) == null && GetCustomerByUsername(customer.Username) == null)
                {
                    DAL.Customers.Add(customer);
                    DAL.entity.SaveChanges();
                    AddUserToUserTable(customer.Username);
                    return Json(new { status = true, msg = customer.FirstName + " has been added Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, msg = "Username Or ID is Already taken" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = false, msg = "You must fill all the required fields" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Add User to Users Table - Role Table
        /// </summary>
        /// <param name="Username"></param>
        private void AddUserToUserTable(string Username)
        {
            User newUser = new User();
            newUser.IsAdmin = false;
            newUser.Username = Username;

            websiteSettings.UserInSystem = newUser;
            DAL.Users.Add(newUser);
            DAL.entity.SaveChanges();
        }
        
        /// <summary>
        /// Edit Customer Details - Get Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditCustomer(string id)
        {
            return PartialView(GetCustomerByID(id));
        }

        /// <summary>
        /// Check if all the required fields are filled
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        private bool AllRequiredFiledsAreFilled(Customer customer)
        {
            if (!string.IsNullOrEmpty(customer.City) && !string.IsNullOrEmpty(customer.Email) &&
                !string.IsNullOrEmpty(customer.FirstName) && !string.IsNullOrEmpty(customer.LastName) &&
                !string.IsNullOrEmpty(customer.ID) && !string.IsNullOrEmpty(customer.Username) &&
                !string.IsNullOrEmpty(customer.Password) && !string.IsNullOrEmpty(customer.Phone))
                return true;
            return false;
        }

        /// <summary>
        /// Updating The Customer Details Using Stored Procedures.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        private bool UpdateCustomerDetails(Customer customer)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ToChangeDBModelEntities"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("UpdateCustomerDetails", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID" , customer.ID);
            cmd.Parameters.AddWithValue("@FirstName" , customer.FirstName);
            cmd.Parameters.AddWithValue("@LastName" , customer.LastName);
            cmd.Parameters.AddWithValue("@Email" , customer.Email);
            cmd.Parameters.AddWithValue("@Phone" , customer.Phone);
            cmd.Parameters.AddWithValue("@Username" , customer.Username);
            cmd.Parameters.AddWithValue("@Password" , customer.Password);
            cmd.Parameters.AddWithValue("@City" , customer.City);
            if (con.State != System.Data.ConnectionState.Open)
                con.Open();
            int UpdatingResult = cmd.ExecuteNonQuery();
            con.Close();

            if (UpdatingResult >= 1)
                return true;

            return false;
        }

        /// <summary>
        /// Edit Customer Details - Post Method
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditCustomer(Customer customer)
        {
            customer.Username = DAL.Customers.Find(customer.ID).Username;

            if (AllRequiredFiledsAreFilled(customer))
            {
                //User exist
                if (GetCustomerByID(customer.ID) != null && GetCustomerByUsername(customer.Username) != null)
                {
                    bool UpdatingResult = UpdateCustomerDetails(customer);
                    if(UpdatingResult)
                        return Json(new { status = true, msg = customer.FirstName + " has been modified Successfully" }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { status = false, msg = customer.FirstName + " Cannot Be Updated." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = false, msg = "User Not Found" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { status = false, msg = "You must fill all the required fields" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// View Customer details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ViewCustomer(string id)
        {
            return PartialView(GetCustomerByID(id));
        }


        /// <summary>
        /// delete Customer - Get Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DeleteCustomer(string id)
        {
            return PartialView(GetCustomerByID(id));
        }

        /// <summary>
        /// Delete Customer - Post Method
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteCustomer(Customer customer)
        {
            var customerToDelete = DAL.Customers.Find(customer.ID);

            if (customerToDelete != null)
            {
                DAL.Customers.Remove(customerToDelete);
                DAL.entity.SaveChanges();
                return Json(new { status = true, msg = customerToDelete.FirstName + " has deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }else
            {
                return Json(new { status = false, msg = "Something Went Wrong" }, JsonRequestBehavior.AllowGet);
            }
        }



    }
}