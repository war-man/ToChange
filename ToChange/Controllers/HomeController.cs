using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToChange.DB_Connections;
using ToChange.Models;
using PagedList;
using PagedList.Mvc;
using ToChange.ViewModel;
using System.Collections.Specialized;
using System.Reflection;

namespace ToChange.Controllers
{
    public class HomeController : Controller
    {
        HomeVM homeVM = new HomeVM();

        /// <summary>
        /// Home Action - for Guests
        /// </summary>
        /// <returns></returns>
        public ActionResult Home()
        {
            homeVM.Products = DAL.Products.OrderBy(a => a.ID).ToPagedList(1, 7);
            return View(homeVM);
        }

        /// <summary>
        /// Search in products table for Guests
        /// </summary>
        /// <param name="txtSearch"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Search(string txtSearch)
        {
            homeVM.Products = DAL.Products.OrderBy(a => a.ID).ToPagedList(1, 7);

            if (!string.IsNullOrEmpty(txtSearch))
                homeVM.Products = DAL.Products.Where(a => a.ProductName.ToLower()
                .Contains(txtSearch.ToLower())).OrderBy(a => a.ProductName).ToPagedList(1, 7);

            ModelState.Clear();

            return View("Home" , homeVM);
        }

        /// <summary>
        /// Navigate to specific Page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Pagination(int? page)
        {
            homeVM.Products = DAL.Products.OrderBy(a => a.ID).ToPagedList(page ?? 1, 7);
            return View("Home", homeVM);
        }

        /// <summary>
        /// Load the Left Side Bar menu
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadSideBarMenu()
        {

            return PartialView("SideBarMenu");
        }

        /// <summary>
        /// Get the Correct Home Page According to the Current User(Logged In User)
        /// </summary>
        /// <returns></returns>
        public ActionResult NavigateToHomeAction()
        {
            string action = "Home";
            string controller = "Home";

            if (websiteSettings.hasLoggedIn)
            {
                if (websiteSettings.UserInSystem.IsAdmin)
                    controller = "Admin";
                else
                    controller = "User";
            }

            return RedirectToAction(action, controller);
        }

        /// <summary>
        /// Get History Page According to the User
        /// </summary>
        /// <returns></returns>
        public ActionResult NavigateToHistoryAction()
        {
            string action = "Home";
            string controller = "Home";

            if (websiteSettings.hasLoggedIn)
            {
                action = "Exchanges";
                if (websiteSettings.UserInSystem.IsAdmin)
                    controller = "Admin";
                else
                    controller = "User";
            }

            return RedirectToAction(action, controller);
        }
    }
}