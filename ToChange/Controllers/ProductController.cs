using PagedList;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToChange.DB_Connections;
using ToChange.Models;

namespace ToChange.Controllers
{
    public class ProductController : Controller
    {
        /// <summary>
        /// Get all the products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<Product> GetAll()
        {
            return DAL.Products.ToList();
        }

        /// <summary>
        /// Get specifec product by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public Product GetProduct(int id)
        {
            return DAL.Products.Where(x => x.ID.Equals(id)).FirstOrDefault();
        }

        /// <summary>
        /// MyProducts Table
        /// </summary>
        /// <param name="txtSearch"></param>
        /// <returns></returns>
        public ActionResult MyProducts(string txtSearch)
        {

            IPagedList<Product> model = DAL.Products.ToList().ToPagedList(1, 7);

            if (websiteSettings.hasLoggedIn && !websiteSettings.UserInSystem.IsAdmin)
            {
                    if (string.IsNullOrEmpty(txtSearch))
                        return View(DAL.Products.Where(a => a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).ToList().ToPagedList(1, 7));

                    model = DAL.Products.Where(a => a.ProductName.ToLower().Contains(txtSearch.ToLower()) && a.CustomerID.Equals(websiteSettings.CurrentUser.ID))
                        .ToList().ToPagedList(1, 7);
            }

            return View(model);
        }

        /// <summary>
        /// Open Add Product View , PartialView Because we open it in a popup
        /// </summary>
        /// <returns></returns>
        public ActionResult AddProduct()
        {
            return PartialView();
        }

        /// <summary>
        /// Check for the required fields
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool AllRequiredFiledsAreFilled(Product product)
        {
            if (!string.IsNullOrEmpty(product.Description) &&
                !string.IsNullOrEmpty(product.ProductImage) && !string.IsNullOrEmpty(product.CustomerID)
                && !string.IsNullOrEmpty(product.ProductImage))
                return true;

            return false;
        }

        /// <summary>
        /// Open Product Details Window
        /// </summary>
        /// <param name="id">id of the product</param>
        /// <returns></returns>
        public ActionResult ProductDetails(int id)
        {
            return PartialView(GetProduct(id));
        }


        /// <summary>
        /// Products Table
        /// </summary>
        /// <param name="txtSearch"></param>
        /// <returns></returns>
        public ActionResult Products(string txtSearch)
        {
            IPagedList<Product> model = DAL.Products.ToList().ToPagedList(1 ,7);

            if(websiteSettings.hasLoggedIn)
            {
                if(!websiteSettings.UserInSystem.IsAdmin)
                {
                    if (string.IsNullOrEmpty(txtSearch))
                        return View(DAL.Products.ToList().ToPagedList(1, 7));

                    model = DAL.Products.Where(a => a.ProductName.ToLower().Contains(txtSearch.ToLower()) && !a.CustomerID.Equals(websiteSettings.CurrentUser.ID))
                        .ToList().ToPagedList(1, 7);
                }
            }
            
            return View(model);
        }

        /// <summary>
        /// Post Method - Add Product
        /// </summary>
        /// <param name="newProduct"></param>
        /// <returns>Json Object withe the result</returns>
        [HttpPost]
        public ActionResult AddProduct(Product newProduct)
        {

            newProduct.CustomerID = websiteSettings.CurrentUser.ID; //Add Customer ID

            if (AllRequiredFiledsAreFilled(newProduct))
            {
                var product = GetProduct(newProduct.ID);

                if (product == null)
                {
                    DAL.Products.Add(newProduct);
                    DAL.entity.SaveChanges();
                    return Json(new { status = true, msg = newProduct.ProductName + " has been added successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        status = true,
                        msg = newProduct.ProductName + " is Already Exist in the System"
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(new { status = true, msg = "All Fields Are Required" }, JsonRequestBehavior.AllowGet);
            }
        }

       
        /// <summary>
        /// Edit Product - Get Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditProduct(int id)
        {
            if (id > 0)
            {
                return PartialView(GetProduct(id));
            }

            return RedirectToAction("Home", "Home");
        }

        /// <summary>
        /// Update Product Details Using Stored Procedure
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private bool UpdateProductDetails(Product product)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ToChangeDBModelEntities"].ToString();
            SqlConnection con = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("UpdateProductDetails", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ID", product.ID);
            cmd.Parameters.AddWithValue("@CategoryID", product.CategoryID);
            cmd.Parameters.AddWithValue("@CustomerID", product.CustomerID);
            cmd.Parameters.AddWithValue("@Description", product.Description);
            cmd.Parameters.AddWithValue("@ProductImage", product.ProductImage);
            cmd.Parameters.AddWithValue("@ProductName", product.ProductName);
            if (con.State != System.Data.ConnectionState.Open)
                con.Open();
            int UpdatingResult = cmd.ExecuteNonQuery();
            con.Close();

            if (UpdatingResult >= 1)
                return true;

            return false;
        }


        /// <summary>
        /// Edit Product - Post Method
        /// </summary>
        /// <param name="product"></param>
        /// <returns>Json Object with the result</returns>
        [HttpPost]
        public ActionResult EditProduct(Product product)
        {
            var updatedProduct = GetProduct(product.ID);
            if (updatedProduct != null)
            {
                product.CustomerID = updatedProduct.CustomerID;
                if (AllRequiredFiledsAreFilled(product))
                {
                    if (product.ID > 0)
                    {
                        if (updatedProduct != null)
                        {
                            bool UpdateResult = UpdateProductDetails(product);
                            if(UpdateResult)
                                return Json(new { status = true, msg = product.ProductName + " has updated Successfully" }, JsonRequestBehavior.AllowGet);
                            else
                                return Json(new { status = false, msg = product.ProductName + " cannot be updated" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { status = true, msg = "This Product Cannot Be Edited" }, JsonRequestBehavior.AllowGet);

                        }
                    }
                }
                else
                {
                    return Json(new { status = true, msg = "All Fields Are Required" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = true, msg = "Something Went Wrong" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Delete Product - Get Method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteProduct(int id)
        {
            if (id > 0)
            {
                return PartialView(GetProduct(id));
            }

            return RedirectToAction("Home", "Home");
        }

        /// <summary>
        /// Delete Product - Post Method
        /// </summary>
        /// <param name="product"></param>
        /// <returns>Json Object with the result</returns>
        [HttpPost]
        public ActionResult DeleteProduct(Product product)
        {

            //because we disabled the textboxes so we has a null values
            var productToDelete = DAL.Products.Find(product.ID); // find the product that you want to delete

            if(productToDelete != null)
            { 
                DAL.Products.Remove(productToDelete);
                DAL.entity.SaveChanges();
                return Json(new { status = true, msg = productToDelete.ProductName + " has deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { status = false, msg = "Something Went Wrong" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Upload Image To Images Folder into the project
        /// </summary>
        /// <param name="model"></param>
        /// <returns>json message with the path of the image or error message</returns>
        [HttpPost]
        public JsonResult UploadImage(ImageModel model)
        {
            var file = model.ImageFile;
            if (file != null)
            {
                string fileExtension = Path.GetExtension(file.FileName.ToLower());
                if (isImageExtension(fileExtension))
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = "/Images/" + fileName;
                    file.SaveAs(Server.MapPath(path));
                    return new JsonResult { Data = new { status = true, path = path }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }else
                {
                    return new JsonResult { Data = new { status = false, path = "Please Select Image Only!\n" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
            }

            return new JsonResult { Data = new { status = false, path = "Cannot Upload Image" }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        /// <summary>
        /// Check for the extension of the file , if it's image or not
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        private bool isImageExtension(string fileExtension)
        {
            if (fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif" || fileExtension == ".jpg")
                return true;
            return false;
        }

        /// <summary>
        /// Navigate to Specific Page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public ActionResult Pagination(int? page , int tableID)
        {
            var returnedModel = DAL.Products.ToList().ToPagedList(page ?? 1, 7);

            if (tableID == websiteSettings.PRODUCTS_TABLE_ID)
            {
                returnedModel = DAL.Products.Where(a => !a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.ProductName).ToPagedList(page ?? 1, 7);
                return View("Products", returnedModel);
            }

            if (tableID == websiteSettings.MYPRODUCTS_TABLE_ID)
            {
                returnedModel = DAL.Products.Where(a => a.CustomerID.Equals(websiteSettings.CurrentUser.ID)).OrderBy(a => a.ProductName).ToPagedList(page ?? 1, 7);
                return View("MyProducts", returnedModel);
            }

            return RedirectToAction("Home" , "Home");
        }

    }
}