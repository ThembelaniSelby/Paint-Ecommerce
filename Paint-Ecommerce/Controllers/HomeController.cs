
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest.ClientRuntime;
using Microsoft.Rest.ClientRuntime.Azure;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using System.Web.Mvc;
using Paint_Ecommerce.Models;
using PagedList;
using QRCoder;
using System.Drawing;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Image = iTextSharp.text.Image;
using System.Net.Mail;

namespace Paint_Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        Db db = new Db();
        public ActionResult Index()
        {
            //User u = db.Users.Find(1);
            //u.Status = "VERIFIED";
            //db.SaveChanges();
            return View();
        }
        public ActionResult Products(int? page, int? catId)
        {

            //Declare List Of ProductVM
            List<ProductVM> listOfProductVM;
            //Set Page Number
            var pageNumber = page ?? 1;
            using (Db db = new Db())
            {
                //Intialize List
                listOfProductVM = db.Products.ToArray()
                                .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                                .Select(x => new ProductVM(x))
                                .ToList();
                //Populate Categories select list
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                //set selected category
                ViewBag.SelectedCat = catId.ToString();
            }

            //set pagination
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 10);
            ViewBag.OnePageOfProducts = onePageOfProducts;
            //return view with list

            return View();
        }


        public ActionResult Category(int catid)
        {

            var pro = db.Products.Where(x => x.CategoryId == catid).ToList();

            return View(pro);
        }





        public ActionResult Categories()
        {
            var cat = db.Categories.ToList();
            return View(cat);
        }

       


        public ActionResult productinfo(int id)
        {
            Product pr = db.Products.Find(id);


            var rev = db.ProductReviews.Where(x => x.ProductId == id).ToList();

            var ngrev = db.ProductReviews.Where(x => x.ProductId == id && x.Status == "Negative").ToList();
            var porev = db.ProductReviews.Where(x => x.ProductId == id && x.Status == "Positive").ToList();



            decimal totrev = rev.Count();

            decimal negrev = ngrev.Count();

            decimal posrev = porev.Count();





            if (rev.Count() != 0)
            {
                string totpos = decimal.Round(posrev / totrev * 100, 0, MidpointRounding.AwayFromZero).ToString();
                ViewBag.Rating = totpos.ToString() + "%";
                ViewBag.Tot = totrev.ToString();
                return View(pr);

            }

            else
            {
                ViewBag.Rating = 0 + "%";
                ViewBag.Tot = totrev.ToString();
                return View(pr);

            }

        }



        public ActionResult ProReviews(int id)
        {
            Product pr = db.Products.Find(id);


            List<ProductReview> li = new List<ProductReview>();


            var rev = db.ProductReviews.Where(x => x.ProductId == id).ToList();

            var ngrev = db.ProductReviews.Where(x => x.ProductId == id && x.Status == "Negative").ToList();
            var porev = db.ProductReviews.Where(x => x.ProductId == id && x.Status == "Positive").ToList();

            foreach (var item in rev)
            {
                decimal rescore = decimal.Parse(item.Score.ToString()) * 100;

                decimal cc = decimal.Round(rescore, 0, MidpointRounding.AwayFromZero);

                li.Add(new ProductReview()
                {
                    Comment = item.Comment,
                    Email = item.Email,
                    Id = item.Id,
                    Name = item.Name,
                    Phone = item.Phone,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Score = double.Parse(cc.ToString()),
                    Status = item.Status,
                });
            }


            decimal totrev = rev.Count();

            decimal negrev = ngrev.Count();

            decimal posrev = porev.Count();

            if (rev.Count() != 0)
            {
                string totpos = decimal.Round(posrev / totrev * 100, 0, MidpointRounding.AwayFromZero).ToString();
                ViewBag.Rating = totpos.ToString() + "%";
                ViewBag.Tot = totrev.ToString();
                return View(li);

            }

            else
            {
                ViewBag.Rating = 0 + "%";
                ViewBag.Tot = totrev.ToString();
                return View(li);

            }



        }


        public ActionResult Prods(int id)
        {
            Product pr = db.Products.Find(id);
            var prods = db.Products.Where(x => x.CategoryId == pr.CategoryId).ToList().Take(4);
            return View(prods);
        }

        public ActionResult Showcase()
        {
            var prod = db.Products.ToList().Take(1);
            return View(prod);
        }

        public ActionResult FirstSide()
        {
            var prod = db.Products.ToList().Take(6);
            return View(prod);
        }
        public ActionResult SecondSide()
        {
            var prod = db.Products.ToList().Take(1);
            return View(prod);
        }

        public ActionResult Search(string q)
        {

            var pro = db.Products.Where(x => x.Name.Contains(q));

            return View(pro);
        }

        public ActionResult QuickView(int id)
        {
            Product pr = db.Products.Find(id);
            return View(pr);
        }



        public ActionResult Cart()
        {
            // Init the cart list
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Check if cart is empty
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "cart is empty.";

                return View();
            }
            // Calculate total and save to ViewBag

            double total = 0;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            int productQuantity = 0;
            int status = 0;
            using (Db db = new Db())

                foreach (var item in cart)
                {

                    //Get Current User Balance
                    var p = db.Products.FirstOrDefault(x => x.Id == item.ProductId);
                    productQuantity += p.Quantity;

                    if (productQuantity < item.Quantity)
                    {
                        status = 1;
                    }
                    //Get Current User Balance
                }
            ViewBag.Stat = status;

            ViewBag.Quant = productQuantity;


            ViewBag.GrandTotal = total;
            ViewBag.Dilivloyal = total - 100;
            ViewBag.Categories = new SelectList(db.PickPoints.ToList(), "Id", "PointAddress");
            // Return view with list
            return View(cart);
        }

        public ActionResult AddtoCart(int id,string color,int qty)
        {

            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();



            Db cc = new Db();


            // Init CartVM
            CartVM model = new CartVM();


            using (Db db = new Db())
            {
                // Get the product
                Product product = db.Products.Find(id);
                int pr = product.Price;

                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                // If not, add new



                if ((productInCart == null) && (product.Quantity >= 1))
                {


                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Color =color,
                        Price = pr,
                        Image = product.ImageName,
                        ProductQuant = product.Quantity
                    });

                }


                else if ((productInCart != null) && (product.Quantity - 1 >= productInCart.Quantity))
                {
                    // If it is, increment
                    productInCart.Quantity++;
                }

                {
                    Redirect("/");
                }

                string dat = DateTime.UtcNow.DayOfWeek.ToString().ToUpper();


            }

            // Get total qty and price and add to model
            int price = 0;

            foreach (var item in cart)
            {
                qty += item.Quantity;

                price += item.Quantity * item.Price;
            }


            model.Quantity = qty;

            model.Price = price;


            // Save cart back to session
            Session["cart"] = cart;

            // Return partial view with model
            return Redirect("/");
        }


        public ActionResult IncrementProduct(int productId)
        {
            // Init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            using (Db db = new Db())
            {
                Product product = db.Products.Find(productId);

                // Get cartVM from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);
                if (product.Quantity > model.Quantity)
                {
                    model.Quantity++;
                    // Store needed data
                    var result = new { qty = model.Quantity, price = model.Price };
                    // Return json with data
                    return RedirectToAction("cart");
                }

                else
                {
                    return RedirectToAction("cart");

                }
                // Increment qty

            }

        }

        // GET: /Cart/DecrementProduct
        public ActionResult DecrementProduct(int productId)
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            using (Db db = new Db())
            {
                // Get model from list
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Decrement qty
                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                // Store needed data
                var result = new { qty = model.Quantity, price = model.Price };
                // Return json
                return RedirectToAction("cart");
            }
        }

        // GET: /Cart/RemoveProduct
        public ActionResult RemoveProduct(int productId)
        {
            // Init cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            using (Db db = new Db())
            {
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);
                // Remove model from list
                cart.Remove(model);
            }

            return RedirectToAction("cart");

        }


        public ActionResult Reorder(int id)
        {
            var details = db.OrderDetails.Where(x => x.OrderId == id).ToList();
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            foreach (var item in details)
            {
                Product pr = db.Products.Find(item.ProductId);

                cart.Add(new CartVM()
                {
                    Color = item.Color,
                    Image = pr.ImageName,
                    Price = pr.Price,
                    ProductId = pr.Id,
                    Quantity = item.Quantity,
                    ProductName = pr.Name,
                    ProductQuant = item.Quantity,
                });
            }
            Session["cart"] = cart;



            return Redirect("/home/cart");
        }






        // POST: /Cart/PlaceOrder
        [Authorize]
        public ActionResult PlaceOrder(string shipping, Nullable<int> point)
        {
            // Get cart list
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // Get username
            string username = User.Identity.Name;
            int orderId = 0;

            using (Db db = new Db())
            {
                Order orderDTO = new Order();

                var add = db.Addresses.Where(x => x.Username == User.Identity.Name).FirstOrDefault();
                if(add == null)
                {
                    return Redirect("/Account/user-profile");
                }


               


                // Get user id
                var q = db.Users.FirstOrDefault(x => x.EmailAddress == username);
                int userId = q.Id;
                int tot = 0;

                foreach (var item in cart)
                {

                    tot = cart.Sum(x => x.Total);
                }

                orderDTO.TotalPrice = tot;
                orderDTO.UserId = userId;
                orderDTO.CreatedAt = DateTime.UtcNow.AddHours(2);

                orderDTO.Status = "NEW";
                orderDTO.DeliveryStatus = "NEW";
                string deliveryaddress = "";
                if(shipping == "address")
                {
                    deliveryaddress = add.Addres + " " + add.City + " " + add.Province + " " + add.Zip;
                }

                if(shipping == "TOBECOLLECTED")
                {
                    deliveryaddress = "TOBECOLEECTED";
                }

                if(shipping == "pickpoint" && point!= null)
                {
                    PickPoint pi = db.PickPoints.Find(point);

                    deliveryaddress = pi.PointAddress;
                }

                if(shipping =="pickpoint" && point == null)
                {
                    return RedirectToAction("Cart");
                }
                orderDTO.Destination = deliveryaddress;

                db.Orders.Add(orderDTO);
                db.SaveChanges();
                orderId = orderDTO.OrderId;


                OrderDetails orderDetailsDTO = new OrderDetails();
                // Add to OrderDetailsDTO
                foreach (var item in cart)
                {
                    orderDetailsDTO.OrderId = orderId;
                    orderDetailsDTO.ProductId = item.ProductId;
                    orderDetailsDTO.Color = item.Color;
                    orderDetailsDTO.Quantity = item.Quantity;
                    db.OrderDetails.Add(orderDetailsDTO);
                    db.SaveChanges();
                }
            }

            // Reset session
            Session["cart"] = null;

            return RedirectToAction("PaymentOptions");
        }

        public ActionResult Cartcount()
        {
            List<CartVM> cart = Session["cart"] as List<CartVM>;
            if (cart != null)
            {
                string c = cart.ToList().Count().ToString();

                return Content(c);
            }
            else
            {
                return Content("0");
            }

        }

        [Authorize]
        public ActionResult PaymentOptions()
        {
            //Intialize List Of OrdersForUserVM
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();
            ViewBag.Bal = "";

            using (Db db = new Db())
            {
                //get user id
                User user = db.Users.Where(x => x.EmailAddress == User.Identity.Name).FirstOrDefault();
                int userId = user.Id;
                //Intialize List Of OrderVM
                List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId && x.Status == "NEW").ToArray().Select(x => new OrderVM(x)).OrderByDescending(p => p.CreatedAt).Take(1).ToList();

                string dest = orders.FirstOrDefault().Destination;




                //loop through List Of OrderVM
                foreach (var order in orders)
                {
                    //Intialize Product Dictionary
                    Dictionary<string, int> productsAndQty = new Dictionary<string, int>();
                    //declare total
                    int total = 0;

                    //Intialize List Of OrderDetailsDTO
                    List<OrderDetails> orderDetailsDTO = db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();
                    //loop through List Of OrderDetailsDTO
                    foreach (var orderDetails in orderDetailsDTO)
                    {

                        //Get The Product
                        Product product = db.Products.Where(x => x.Id == orderDetails.ProductId).FirstOrDefault();
                        //Get The Product Price
                        int price = product.Price;
                        //Get The Product Name
                        string productName = product.Name;
                        //Add to Product dictionary
                        productsAndQty.Add(productName, orderDetails.Quantity);
                        //Get Total
                        total += orderDetails.Quantity * price;
                        int tok = orderDetails.Quantity * price;
                        int tokt = order.TotalPrice;


                    }
                    //Add to ordersforuserVM List
                    ordersForUser.Add(new OrdersForUserVM()
                    {

                        OrderNumber = order.OrderId,

                        Total = order.TotalPrice,


                        ProductsAndQty = productsAndQty,
                        CreatedAt = order.CreatedAt,
                        Destination = order.Destination,
                        DeliveryFee = order.DeliveryFee,

                    });
                }

            }

            //Return View With List Of OrdersForUserVM

            return View(ordersForUser);
        }


        /// <summary>
        /// ///////////////////////////////////////////////////
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// </summary>
        /// <returns></returns>
        /// 







        [Authorize]
        public ActionResult Payfast(string SearchString)
        {
            Db dc = new Db();
            //Intialize List Of OrdersForUserVM
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();
            int tt = 0;
            using (Db db = new Db())
            {
                //get user id
                User user = db.Users.Where(x => x.EmailAddress == User.Identity.Name).FirstOrDefault();
                int userId = user.Id;
                //Intialize List Of OrderVM
                List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId && x.Status == "NEW").ToArray().Select(x => new OrderVM(x)).OrderByDescending(p => p.CreatedAt).Take(1).ToList();

                tt = orders.FirstOrDefault().TotalPrice;
                //loop through List Of OrderVM
                foreach (var order in orders)
                {
                    //Intialize Product Dictionary
                    Dictionary<string, int> productsAndQty = new Dictionary<string, int>();
                    //Intialize List Of OrderDetailsDTO
                    List<OrderDetails> orderDetailsDTO = db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();
                    //loop through List Of OrderDetailsDTO
                    foreach (var orderDetails in orderDetailsDTO)
                    {

                        //Get The Product
                        Product product = db.Products.Where(x => x.Id == orderDetails.ProductId).FirstOrDefault();
                        //Get The Product Price
                        int price = product.Price;
                        //Get The Product Name
                        string productName = product.Name;
                        //Add to Product dictionary
                        productsAndQty.Add(productName, orderDetails.Quantity);
                        //Get Total
                       

                    }

                    //Add to ordersforuserVM List
                    ordersForUser.Add(new OrdersForUserVM()
                    {


                        OrderNumber = order.OrderId,
                        Total = orders.FirstOrDefault().TotalPrice,
                        ProductsAndQty = productsAndQty,
                        CreatedAt = order.CreatedAt,
                        Status = order.Status

                    });
                }

            }

            return Redirect("https://www.payfast.co.za/eng/process?cmd=_paynow&receiver=18541607&item_name=ORDER-FEE&item_description=ORDER-PAYMENT&return_url=https://2021grp33.azurewebsites.net/HOME/DeductQuantity&cancel_url=https://2021grp33.azurewebsites.net/hOME/DeductQuantity&amount=" + tt + "&email_address=" + dc.Users.Where(x => x.EmailAddress == User.Identity.Name).FirstOrDefault().EmailAddress);
        }
        int idsms = 0;

        public ActionResult MyBookings()
        {
            return View(db.Bookings.Where(x => x.Email == User.Identity.Name).ToList());
        }


        [Authorize]
        public ActionResult BookService()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BookService(Booking model)
        {
            int PRICE = 0;
            if (model.Servicetype == "Damp-Proofing")
            {
                PRICE = 200;
            }
            if (model.Servicetype == "Roof Proofing")
            {
                PRICE = 300;
            }
            if (model.Servicetype == "Interior-Painting")
            {
                PRICE = 400;
            }
            if (model.Servicetype == "Exterior-Painting")
            {
                PRICE = 400;
            }



            Booking bk = new Booking()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = User.Identity.Name,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                SUrbace = model.SUrbace,
                ZipCode = model.ZipCode,
                Servicetype = model.Servicetype,
                ServicingDate = model.ServicingDate,
                InspectionDate = model.InspectionDate,
                ServiceStatus = "NEW",
                InspectinStatus ="NEW",
                Satatus = "NEW",
                Total =  PRICE ,
                StatusNum =1

            };
            db.Bookings.Add(bk);
            db.SaveChanges();







            return Redirect("/account/user-profile");
        }

        public ActionResult bookingPayment(int id)
        {
            Booking bk = db.Bookings.Find(id);


            return Redirect("https://www.payfast.co.za/eng/process?cmd=_paynow&receiver=18541607&item_name=ORDER-FEE&item_description=ORDER-PAYMENT&return_url=https://2021grp33.azurewebsites.net/home/UpdateBooking?id="+bk.Id+ "&cancel_url=https://2021grp33.azurewebsites.net/home/UpdateBooking?id=" + bk.Id + "&amount=" + bk.Total + "&email_address=" + db.Users.Where(x => x.EmailAddress == User.Identity.Name).FirstOrDefault().EmailAddress);

        }

        public ActionResult UpdateBooking(int id)
        {
            GetQuery qr = new GetQuery();

            Booking bk = db.Bookings.Find(1);


            bk.Satatus = "PAID";
            bk.StatusNum = 6;
            bk.Qr = qr.Main();


            db.SaveChanges();


            return Redirect("/account/user-profile");
        }


        [Authorize]
        public ActionResult DeductQuantity()
        {
            //Intialize List Of OrdersForUserVM
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();

            using (Db db = new Db())
            {
                //get user id
                User user = db.Users.Where(x => x.EmailAddress == User.Identity.Name).FirstOrDefault();
                int userId = user.Id;
                //Intialize List Of OrderVM
                List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId && x.Status == "NEW").ToArray().Select(x => new OrderVM(x)).OrderByDescending(p => p.CreatedAt).Take(1).ToList();

                //loop through List Of OrderVMx.
                foreach (var order in orders)
                {
                    //Intialize Product Dictionary
                    Dictionary<string, int> productsAndQty = new Dictionary<string, int>();
                    //declare total
                    int total = 0;
                    //Intialize List Of OrderDetailsDTO
                    List<OrderDetails> orderDetailsDTO = db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();
                    //loop through List Of OrderDetailsDTO
                    foreach (var orderDetails in orderDetailsDTO)
                    {

                        //Get The Product
                        Product product = db.Products.Where(x => x.Id == orderDetails.ProductId).FirstOrDefault();
                        //Get The Product Price
                        int price = product.Price;
                        //Get The Product Name
                        string productName = product.Name;
                        //Add to Product dictionary
                        productsAndQty.Add(productName, orderDetails.Quantity);
                        //Get Total
                        total += orderDetails.Quantity * price;

                    }



                    /////////////////////////////////////
                    ////////////////////////////////////
                    ///
                    foreach (var item in orderDetailsDTO)
                    {
                        //Get Current User Balance
                        var p = db.Products.FirstOrDefault(x => x.Id == item.ProductId);
                        int productQuantity = p.Quantity;
                        //Get Current User Balance


                        var productId = item.ProductId; // Set a user ID that you would like to retrieve

                        var dbContext = new Db(); // Your entity framework DbContext

                        // Retrieve a user from the database
                        var product = dbContext.Set<Product>().Find(productId);

                        // Update a property on your user
                        product.Quantity = productQuantity - item.Quantity;

                        // Save the new value to the database
                        dbContext.SaveChanges();


                     



                    }
                    foreach (var item in orderDetailsDTO)
                    {

                        //Get Current User Balance
                        var p = db.Orders.FirstOrDefault(x => x.OrderId == item.OrderId);
                        string productQuantity = p.Status;
                        //Get Current User Balance




                        var orderId = item.OrderId; // Set a user ID that you would like to retrieve

                        var dbContext = new Db(); // Your entity framework DbContext

                        // Retrieve a user from the database
                        var orderr = dbContext.Set<Order>().Find(orderId);

                        if (3 > 2)
                        {
                            idsms = orderId;
                        }

                        // Update a property on your user
                        orderr.Status = "PAID";
                        orderr.DeliveryStatus = "PROCESSING";


                        GetQuery query = new GetQuery();

                        orderr.OrderCode = query.Main();

                        // Save the new value to the database
                        dbContext.SaveChanges();
                    }


                    //Add to ordersforuserVM List
                    ordersForUser.Add(new OrdersForUserVM()
                    {
                        OrderNumber = order.OrderId,
                        Total = total,
                        ProductsAndQty = productsAndQty,
                        CreatedAt = order.CreatedAt

                    });

                }

            }







            //Return View With List Of OrdersForUserVM

            return RedirectToAction("ActionQrCode");
        }



        [Authorize]

        public ActionResult ActionQrCode()
        {

            int wid = 0;
            //Intialize List Of OrdersForUserVM
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();

            using (Db db = new Db())
            {
                //get user id
                User user = db.Users.Where(x => x.EmailAddress == User.Identity.Name).FirstOrDefault();
                int userId = user.Id;
                //Intialize List Of OrderVM
                List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId && x.Status == "PAID").ToArray().Select(x => new OrderVM(x)).OrderByDescending(p => p.CreatedAt).Take(1).ToList();

                //loop through List Of OrderVMx.
                foreach (var order in orders)
                {
                    //Intialize Product Dictionary
                    Dictionary<string, int> productsAndQty = new Dictionary<string, int>();
                    //declare total
                    int total = 0;
                    //Intialize List Of OrderDetailsDTO
                    List<OrderDetails> orderDetailsDTO = db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();
                    //loop through List Of OrderDetailsDTO
                    foreach (var orderDetails in orderDetailsDTO)
                    {

                        //Get The Product
                        Product product = db.Products.Where(x => x.Id == orderDetails.ProductId).FirstOrDefault();
                        //Get The Product Price
                        int price = product.Price;
                        //Get The Product Name
                        string productName = product.Name;
                        //Add to Product dictionary
                        productsAndQty.Add(productName, orderDetails.Quantity);
                        //Get Total
                        total += orderDetails.Quantity * price;

                        var dbContext = new Db(); // Your entity framework DbContext

                        // Retrieve a user from the database



                    }



                    /////////////////////////////////////
                    ////////////////////////////////////
                    ///
                    foreach (var item in orderDetailsDTO)
                    {
                        //Get Current User Balance
                        var p = db.Products.FirstOrDefault(x => x.Id == item.ProductId);
                        int productQuantity = p.Quantity;
                        //Get Current User Balance


                        var productId = item.ProductId; // Set a user ID that you would like to retrieve

                        var dbContext = new Db(); // Your entity framework DbContext

                        // Retrieve a user from the database
                        var product = dbContext.Set<Product>().Find(productId);

                        // Update a property on your user

                        // Save the new value to the database
                        dbContext.SaveChanges();
                    }
                    foreach (var item in orderDetailsDTO)
                    {

                        //Get Current User Balance
                        var p = db.Orders.FirstOrDefault(x => x.OrderId == item.OrderId);
                        string productQuantity = p.Status;
                        //Get Current User Balance




                        var orderId = item.OrderId; // Set a user ID that you would like to retrieve

                        var dbContext = new Db(); // Your entity framework DbContext

                        // Retrieve a user from the database
                        var orderr = dbContext.Set<Order>().Find(orderId);

                        if (3 > 2)
                        {
                            wid = orderId;
                        }


                    }
                    int orderid = orders.FirstOrDefault().OrderId;
                    /// qr code generator
                    Order ordd = db.Orders.Find(orderid);

                    string Message = ordd.OrderCode;


                    QRCodeGenerator ObjQr = new QRCodeGenerator();

                    QRCodeData qrCodeData = ObjQr.CreateQrCode(Message, QRCodeGenerator.ECCLevel.Q);

                    Bitmap bitMap = new QRCode(qrCodeData).GetGraphic(20);

                    using (MemoryStream ms = new MemoryStream())

                    {

                        bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                        byte[] byteImage = ms.ToArray();

                        ViewBag.Url = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                        bitMap.Save(Server.MapPath("~/images/Verify/" + User.Identity.Name + wid + "qrcode.png"), System.Drawing.Imaging.ImageFormat.Png);
                    }


                    GetQuery invoname = new GetQuery();
                    string invon = invoname.Main().ToString();
                    System.IO.FileStream fs = new FileStream(Server.MapPath("~/Images/") + invon + ".pdf", FileMode.Create);

                    Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, fs);

                    pdfDoc.Open();


                    try
                    {



                        Order odd = db.Orders.Find(wid);

                        //Top Heading
                        Chunk chunk = new Chunk(DateTime.UtcNow.AddHours(2).ToString(), FontFactory.GetFont("Arial", 5, iTextSharp.text.Font.BOLDITALIC, BaseColor.BLACK));
                        pdfDoc.Add(chunk);

                        //Horizontal Line
                        Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                        pdfDoc.Add(line);


                        //Table
                        PdfPTable table = new PdfPTable(5);
                        table.WidthPercentage = 100;
                        //0=Left, 1=Centre, 2=Right
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 20f;
                        table.SpacingAfter = 30f;
                        ////////
                        ///


                        



                        //Cell no 1
                        PdfPCell cell = new PdfPCell();
                        cell.Border = 0;
                        Image image = Image.GetInstance(Server.MapPath("~/images/Verify/" + User.Identity.Name + wid + "qrcode.png"));
                        image.ScaleAbsolute(100, 100);
                        cell.AddElement(image);
                        table.AddCell(cell);




                        chunk = new Chunk("ORDER NUM: " +wid+ "\nDATE: \n"+odd.CreatedAt+"\nADDRESS: " +odd.Destination+"\nDELIVERY FEE: R "+odd.DeliveryFee+"\nBALANCE DUE :R 0\nTOTAL: R " +odd.TotalPrice, FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell();
                        cell.Border = 0;
                        var para3 = new Paragraph(chunk);
                        para3.Alignment = Element.ALIGN_LEFT;
                        para3.Alignment = -100;

                        cell.AddElement(para3);
                        table.AddCell(cell);




                        chunk = new Chunk("", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell();
                        cell.Border = 0;

                        var para4 = new Paragraph(chunk);
                        para4.Alignment = Element.ALIGN_LEFT;
                        para4.Alignment = -100;

                        cell.AddElement(para4);
                        table.AddCell(cell);



                        chunk = new Chunk("", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell();
                        cell.Border = 0;

                        var para5 = new Paragraph(chunk);
                        para5.Alignment = Element.ALIGN_LEFT;
                        para5.Alignment = -100;

                        cell.AddElement(para5);
                        table.AddCell(cell);



                        //Cell no 2
                        chunk = new Chunk("DELCO \nILLOVU \nTOWNSHIP \nAMANZIMTOTI \nSOUTH AFRICA \nC4004\n", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell();
                        cell.Border = 0;

                        var para1 = new Paragraph(chunk);
                        para1.Alignment = Element.ALIGN_RIGHT;


                        cell.AddElement(para1);
                        table.AddCell(cell);



                        //Add table to document
                        pdfDoc.Add(table);

                        //Horizontal Line
                        //line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                        //pdfDoc.Add(line);

                        //Table
                        table = new PdfPTable(5);
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 20f;
                        table.SpacingAfter = -0f;

                        //Cell
                        cell = new PdfPCell();
                        chunk = new Chunk("ORDER ITEMS", FontFactory.GetFont("Daytona Condensed Light", 14, iTextSharp.text.Font.BOLD, BaseColor.BLACK));
                        cell.Colspan = 5;
                        var para13 = new Paragraph(chunk);
                        para13.Alignment = Element.ALIGN_CENTER;


                        cell.AddElement(para13);
                        cell.BackgroundColor = BaseColor.WHITE;
                        table.AddCell(cell);

                        table.AddCell("PROD NUMBER");
                        table.AddCell("TITLE" + Environment.NewLine);
                        table.AddCell("PRICE" + Environment.NewLine);
                        table.AddCell("QUANTITY" + Environment.NewLine);
                        table.AddCell("TOTAL" + Environment.NewLine);
                        pdfDoc.Add(table);





                        var cart = db.OrderDetails.Where(x => x.OrderId == wid);

                        table = new PdfPTable(5);
                        table.WidthPercentage = 100;
                        table.HorizontalAlignment = 0;
                        table.SpacingBefore = 0f;
                        table.SpacingAfter = 30f;




                        foreach (var item in cart)
                        {
                            line = new Paragraph(new Chunk(item.ProductId.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            table.AddCell(line);
                            line = new Paragraph(new Chunk(item.Product.Name.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            table.AddCell(line);

                            line = new Paragraph(new Chunk("R:" + item.Product.Price.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            table.AddCell(line);
                            line = new Paragraph(new Chunk(item.Quantity.ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            table.AddCell(line);

                            line = new Paragraph(new Chunk((item.Quantity * item.Product.Price).ToString(), FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                            table.AddCell(line);


                        }


                        pdfDoc.Add(table);



                        Paragraph para = new Paragraph();
                        para.Add("Please keep This Slip Safe");
                        pdfDoc.Add(para);

                        //Horizontal Line
                        line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                        pdfDoc.Add(line);

                        pdfWriter.CloseStream = false;
                        pdfDoc.Close();
                        pdfDoc.CloseDocument();
                        fs.Close();


                        string sender = "21817974@dut4life.ac.za";
                        string password = "Dut990310";



                        string recipient = User.Identity.Name;
                        SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

                        client.Port = 587;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.UseDefaultCredentials = false;
                        System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(sender, password);
                        client.EnableSsl = true;
                        client.Credentials = credentials;
                        Attachment data = new Attachment(Server.MapPath("~/Images/" + invon + ".pdf"));


                        var mail = new MailMessage(sender.Trim(), recipient.Trim());

                        mail.Subject = "ORDER SLIP";
                        mail.Body = "Plese  find  Attachement";
                        mail.Attachments.Add(data);
                        client.Send(mail);

                        TempData["Success"] = "Slip Sent!!";



                        return Redirect("/Account/user-profile");
                    }
                    catch
                    {
                        return View();
                    }
                    
                }
            }

            return Redirect("/Account/user-profile");

        }


        public ActionResult DeliveryFee(int id, string dist)
        {


            double distt = double.Parse(dist.Replace(" ", "").Replace("km", "").Replace(".",","));


            double delfee = distt * 1;

            Order order = db.Orders.Find(id);

            order.TotalPrice = order.TotalPrice +  int.Parse(delfee.ToString().Replace(",",""))/10;
            order.DeliveryFee = int.Parse(delfee.ToString().Replace(",", "")) / 10;
            db.SaveChanges();

            return RedirectToAction("PaymentOptions");
        }



        
        public ActionResult Reviews()
        {
            var rev = db.Reviews.ToList();

            return View(rev);
        }
        
        [HttpGet]
        public ActionResult SubmitReview()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SubmitReview(Review model)
        {
            Review rev = new Review()
            {
                Comment = model.Comment,
                Email = model.Email,
                Name = model.Name,
                Phone = model.Phone,
                Service = model.Service,
            };
            db.Reviews.Add(rev);
            db.SaveChanges();

            return View();
        }


        [HttpPost]
        public ActionResult Prouctcomment(ProductReview model)
        {

            Product pr = db.Products.Find(model.ProductId);
            Program prr = new Program();



            prr.ProductId = model.ProductId;
            prr.Comment = model.Comment;

            ProductReview rv = new ProductReview()
            {
                Comment = model.Comment,
                Email = model.Email,
                Name = model.Name,
                Phone = model.Phone,
                ProductId = model.ProductId,
                ProductName = pr.Name,
                Score = prr.GetScore(),
                Status = prr.Main(),
            };

            db.ProductReviews.Add(rv);
            db.SaveChanges();

             return Redirect("/home/productinfo?id=" + model.ProductId);

        }


    }
               
}