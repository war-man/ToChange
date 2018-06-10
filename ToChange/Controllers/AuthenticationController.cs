using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToChange.DB_Connections;
using ToChange.Models;

namespace ToChange.Controllers
{
    public class AuthenticationController : Controller
    {
        /// <summary>
        /// Login Page - Get Method
        /// </summary>
        /// <returns></returns>
        public ActionResult LogIn()
        {
            ShowOrHideErrorMessage(); //Show Error Message if there is an error
            return View();
        }

        /// <summary>
        /// Show Or Hide the Div of the Status Message - if null hide
        /// </summary>
        private void ShowOrHideErrorMessage()
        {
            if (websiteSettings.isFirstTime)
            {
                Session["statusMessage"] = null;
                websiteSettings.isFirstTime = false;
            }
            else
            {
                websiteSettings.isFirstTime = true;
            }
        }

        /// <summary>
        /// Login - Post Method
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LogIn(Customer customer)
        {


            var _customer = DAL.Customers.Where(a => a.Username.Equals(customer.Username) && a.Password.Equals(customer.Password)).FirstOrDefault();
            if (_customer != null)
            {

                var user = DAL.Users.Where(a => a.Username.Equals(customer.Username)).FirstOrDefault();
                if (user != null)
                {

                    websiteSettings.hasLoggedIn = true;
                    websiteSettings.CurrentUser = _customer;
                    websiteSettings.UserInSystem = user;

                    if (user.IsAdmin)
                    {
                        return RedirectToAction("Home", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Home", "User");
                    }
                }
                else
                {
                    Session["statusMessage"] = "Login failed. Something went wrong";
                }
            }
            else
            {
                Session["statusMessage"] = "Incorrect Username Or Password";
            }

            return View(customer);
        }

        /// <summary>
        /// Add User to Users Table (Give him a role)
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
        /// Register - Get Method
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            ShowOrHideErrorMessage();

            return View();
        }

        /// <summary>
        /// Logout - Return all the Variables to Default
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            websiteSettings.ResetAll();

            Session.Clear();

            return RedirectToAction("Home", "Home");
        }

        /// <summary>
        /// Register - Post Method , check if the user doesn't exist and the Username has not taken and register 
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Register(Customer customer)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    var checkIfCustomerWithIDExist = DAL.Customers.Where(a => a.ID.Equals(customer.ID)).FirstOrDefault();
                    var checkIfCustomerWithUsernameExist = DAL.Customers.Where(a => a.Username.Equals(customer.Username)).FirstOrDefault();

                    if (checkIfCustomerWithIDExist == null && checkIfCustomerWithUsernameExist == null)
                    {
                        DAL.Customers.Add(customer);
                        DAL.entity.SaveChanges();
                        AddUserToUserTable(customer.Username);
                        Session["statusMessage"] = customer.Username + " has Registered Successfully.";
                        return RedirectToAction("Home", "Home");
                    }
                    else
                    {
                        Session["statusMessage"] = "This User is Already Registered!";
                    }
                }
                else
                {
                    Session["statusMessage"] = "All The Fields Are Required!";
                }
                return View(customer);
            }
            catch (Exception ex)
            {
                Session["statusMessage"] = "This Code Has Throw an Exception\nException :\n" + ex.Message;
                return View(customer);
            }
        }

    }
}