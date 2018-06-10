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
    public class ExchangeController : Controller
    {

        /// <summary>
        /// Open Poke Popup Window
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PokeRequest(int id)
        {
            return PartialView(GetProductByID(id));
        }

        /// <summary>
        /// Send a Poke
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PokeRequest(Product product)
        {
            var firstCustomer = websiteSettings.CurrentUser;
            var secondCustomer = GetCustomerByProductID(product.ID);

            

            if (firstCustomer == null || secondCustomer == null || firstCustomer.ID.Equals(secondCustomer.ID))
                return RedirectToAction("NavigateToHomeAction", "Home");


            //Create a Handler Object for swap item 
            SwapHandler swapHandler = new SwapHandler();
            swapHandler.firstCustomerID = firstCustomer.ID;
            swapHandler.secondCustomerID = secondCustomer.ID;
            swapHandler.firstProductID = product.ID;
            swapHandler.secondProductID = 0;

            DAL.Swap.Add(swapHandler);
            DAL.entity.SaveChanges();
            
            //Add To Request Table
            Poke poke = new Poke();
            poke.firstCustomerID = firstCustomer.ID;
            poke.secondCustomerID = secondCustomer.ID;
            poke.ProductID = product.ID;
            poke.AddedOn = DateTime.Now;
            poke.Pending = true;
            poke.RespondAt = websiteSettings.DEFAULT_TIME;
            AddPokeToTable(poke);

            return RedirectToAction("NavigateToHomeAction", "Home");
        }

        /// <summary>
        /// Open Popup window to resend request
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ReSendPokeRequest(int id)
        {
            return PartialView(GetProductByID(id));
        }

        /// <summary>
        /// Accept the Request And ReSend Request for the customer
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReSendPokeRequest(Product product)
        {
            var firstCustomer = GetCustomerByProductID(product.ID);
            var secondCustomer = websiteSettings.CurrentUser;


            //for notification
            var request = websiteSettings.GetPokeList(firstCustomer.ID, secondCustomer.ID, websiteSettings.productIDforSwap);

            //for swap process
            var swapRequest = DAL.Swap.Where(a => a.firstCustomerID.Equals(firstCustomer.ID)
            && a.secondCustomerID.Equals(secondCustomer.ID) && a.firstProductID.Equals(websiteSettings.productIDforSwap)).FirstOrDefault();

            if(swapRequest != null)
            {
                if(swapRequest.secondProductID == 0)
                {
                    swapRequest.secondProductID = product.ID;
                    UpdateSwap(swapRequest);
                }
            }

            if(request != null)
            {
                request.RespondAt = DateTime.Now;
                request.ProductIDToSwapWith = product.ID;
                UpdatePoke(request);
            }
            return RedirectToAction("NavigateToHomeAction", "Home");
        }

        /// <summary>
        /// Accept Request
        /// </summary>
        /// <param name="senderID"></param>
        /// <param name="receiverID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public ActionResult AcceptRequest(string senderID, string receiverID , int productID)
        {
            var request = GetPokeList(senderID ,receiverID , productID);

            if(request != null)
            {
                request.Accepted = true;
                request.Pending = false;
                UpdatePoke(request);

                Exchange exchange = new Exchange();
                Product product = DAL.Products.Find(productID);
                Product secondProduct = DAL.Products.Find(request.ProductIDToSwapWith);
                

                exchange.FirstCustomerID = request.firstCustomerID;
                exchange.FirstDescription = product.Description;
                exchange.FirstProductID = productID;
                exchange.SecondCustomerID = receiverID;
                exchange.SecondDescription = secondProduct.Description;
                exchange.SecondProductID = request.ProductIDToSwapWith;

                DAL.Exchanges.Add(exchange);
                DAL.entity.SaveChanges();

                ExchangeProductsBetweenCustomers(request.firstCustomerID, request.secondCustomerID, request.ProductID, request.ProductIDToSwapWith);
            }
            return RedirectToAction("NavigateToHomeAction", "Home");
        }

        /// <summary>
        /// Get Products and Check if they are exist and Update
        /// </summary>
        /// <param name="firstCustomerID"></param>
        /// <param name="secondCustomrID"></param>
        /// <param name="firstProductID"></param>
        /// <param name="secondProductID"></param>
        private void ExchangeProductsBetweenCustomers(string firstCustomerID , string secondCustomrID , int firstProductID , int secondProductID)
        {
            Product firstProduct = GetProductByID(firstProductID);
            Product secondProduct = GetProductByID(secondProductID);

            if (firstProduct != null && secondProduct != null)
            {
                UpdateProductTable(firstProduct, secondCustomrID);
                UpdateProductTable(secondProduct, firstCustomerID);
            }
        }
      
        /// <summary>
        /// Update DataBase
        /// </summary>
        /// <param name="product"></param>
        /// <param name="id"></param>
        private void UpdateProductTable(Product product , string id)
        {

            string ConnectionString = ConfigurationManager.ConnectionStrings["ToChangeDBModelEntities"].ToString();
            string UpdateQuery = "Update Product set CategoryID='" + product.CategoryID +
                "',CustomerID='" + id + "',Description='" +
                 product.Description + "',ProductImage='" + product.ProductImage + "',ProductName='" + product.ProductName + 
                  "' where ID='" + product.ID + "'";
            SqlConnection con = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(UpdateQuery, con);
            if (con.State != System.Data.ConnectionState.Open)
                con.Open();
            cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// Reject Request - Delete from Notification
        /// </summary>
        /// <param name="senderID"></param>
        /// <param name="receiverID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        public ActionResult RejectRequest(string senderID, string receiverID, int productID)
        {
            var request = GetPokeList(senderID, receiverID, productID);
            if (request != null)
            {
                request.Accepted = false;
                request.Pending = false;
                UpdatePoke(request);
            }
            return RedirectToAction("NavigateToHomeAction", "Home");
        }

        /// <summary>
        /// Get OpenRequestInfo Page
        /// </summary>
        /// <param name="id"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        public ActionResult OpenRequestInfo(int id, string requestID)
        {

            websiteSettings.customerIDforSwap = requestID;
            websiteSettings.productIDforSwap = id;

            return View(GetUserVmModel(id , requestID , 1 , ""));
        }
        
        /// <summary>
        /// Return UserVM Model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="requestID"></param>
        /// <param name="page"></param>
        /// <param name="txtSearch"></param>
        /// <returns></returns>
        private UserVM GetUserVmModel(int id , string requestID , int page , string txtSearch)
        {
            var recieverProduct = GetProductByID(id);
            var senderCustomer = GetCustomerByID(requestID);
            var recieverCustomer = GetCustomerByID(recieverProduct.CustomerID);

            IPagedList<Product> senderProducts;

            if (string.IsNullOrEmpty(txtSearch))
                senderProducts = DAL.Products.Where(a => a.CustomerID.Equals(senderCustomer.ID)).OrderBy(a => a.ProductName)
                    .ToPagedList(page, 7);
            else
                senderProducts = DAL.Products.Where(a => a.CustomerID.Equals(senderCustomer.ID) && a.ProductName.ToLower()
                .Contains(txtSearch.ToLower())).OrderBy(a => a.ProductName).ToPagedList(page, 7);

            UserVM userVM = new UserVM();

            var product = DAL.Swap.Where(a => a.firstCustomerID.Equals(senderCustomer.ID)
            && a.secondCustomerID.Equals(recieverCustomer.ID) && a.firstProductID.Equals(recieverProduct.ID)).FirstOrDefault();
            Product senderProduct = null;

            if (product != null)
            {
               senderProduct = GetProductByID(product.secondProductID);
            }
            
            userVM.senderCustomer = senderCustomer;
            userVM.recieverProduct = recieverProduct;
            userVM.Products = senderProducts;
            userVM.recieverCustomer = recieverCustomer;
            userVM.senderProduct = senderProduct;

            return userVM;
        }

        /// <summary>
        /// Navigate To Specific Page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Pagination(int page)
        {
            return View("OpenRequestInfo", GetUserVmModel(websiteSettings.productIDforSwap , websiteSettings.customerIDforSwap , page , ""));
        }

        /// <summary>
        /// Search In List
        /// </summary>
        /// <param name="txtSearch"></param>
        /// <returns></returns>
        public ActionResult SearchProduct(string txtSearch)
        {
            return View("OpenRequestInfo", GetUserVmModel(websiteSettings.productIDforSwap, websiteSettings.customerIDforSwap, 1, txtSearch));
        }

        private Customer GetCustomerByID(string id)
        {
            return DAL.Customers.Find(id);
        }

        /// <summary>
        /// Get Customer ID By Product ID
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        private string GetCustomerIDByProductID(int productID)
        {
            return DAL.Products.Find(productID).CustomerID;
        }

        /// <summary>
        /// Get Product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Product GetProductByID(int id)
        {
            return DAL.Products.Find(id);
        }

        /// <summary>
        /// Get Customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Customer GetCustomerByProductID(int id)
        {
            var customerID = DAL.Products.Find(id).CustomerID;
            return DAL.Customers.Find(customerID);
        }

        /// <summary>
        /// Get Poke Object By ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Poke GetPokeList(string senderID, string receiverID, int productID)
        {
            return DAL.Pokes.Where(a => a.firstCustomerID.Equals(senderID) && a.secondCustomerID.Equals(receiverID) && a.ProductID.Equals(productID)).FirstOrDefault();
        }

        /// <summary>
        /// Add Poke To Table
        /// </summary>
        /// <param name="request"></param>
        private void AddPokeToTable(Poke request)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ToChangeDBModelEntities"].ToString();
            string CreateQuery = "Insert Into Poke(firstCustomerID,secondCustomerID,ProductID,AddedOn,Pending,Accepted,RespondAt,ProductIDToSwapWith)" 
                + " Values('" + request.firstCustomerID +
                "','" + request.secondCustomerID + "','" +
                 request.ProductID + "','" + request.AddedOn + "','" +
                 request.Pending + "','" + request.Accepted + "','" + request.RespondAt
                 + "','" + request.ProductIDToSwapWith + "');";

            SqlConnection con = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(CreateQuery, con);
            if (con.State != System.Data.ConnectionState.Open)
                con.Open();
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Update Poke Table
        /// </summary>
        /// <param name="request"></param>
        private void UpdatePoke(Poke request)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ToChangeDBModelEntities"].ToString();
            string UpdateQuery = "Update Poke set firstCustomerID='" + request.firstCustomerID +
                "',secondCustomerID='" + request.secondCustomerID + "',ProductID='" +
                 request.ProductID + "',AddedOn='" + request.AddedOn +  "',Pending='" +
                 request.Pending + "',Accepted='" + request.Accepted + "',RespondAt='" + request.RespondAt 
                 +  "',ProductIDToSwapWith='" + request.ProductIDToSwapWith + "' where ID='" + request.ID + "'";
            SqlConnection con = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(UpdateQuery , con);
            if (con.State != System.Data.ConnectionState.Open)
                con.Open();
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Update Swap Table
        /// </summary>
        /// <param name="swapRequest"></param>
        private void UpdateSwap(SwapHandler swapRequest)
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["ToChangeDBModelEntities"].ToString();
            string UpdateQuery = "Update Swap set firstCustomerID='" + swapRequest.firstCustomerID +
                "',secondCustomerID='" + swapRequest.secondCustomerID + "',firstProductID='" +
                 swapRequest.firstProductID + "',secondProductID='" + swapRequest.secondProductID + "' where ID='" + swapRequest.ID + "'";
            SqlConnection con = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(UpdateQuery, con);
            if (con.State != System.Data.ConnectionState.Open)
                con.Open();
            cmd.ExecuteNonQuery();
        }

        
    }
}