using iTextSharp.text;
using iTextSharp.text.pdf;
using PagedList;
using Paint_Ecommerce.Models;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Image = iTextSharp.text.Image;

namespace Paint_Ecommerce.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
      
        Db db = new Db();
        public ActionResult Refunds()
        {
            return View(db.Refunds.ToList());
        }
        






        public ActionResult Schedulerefundcollect(int id)
        {
            Refund reff = db.Refunds.Find(id);

            var dell = db.Deliveries.Where(x => x.OrderId == reff.OrderNum).FirstOrDefault();
            int deid = dell.Id;
            Delivery de = db.Deliveries.Find(deid);

            return View(de);
        }









        [HttpPost]
        public ActionResult Schedulerefundcollect(int id,Delivery tripDTO)
        {

            Refund reff = db.Refunds.Find(id);

            var dell = db.Deliveries.Where(x => x.OrderId == reff.OrderNum).FirstOrDefault();
            int deid = dell.Id;

            if (ModelState.IsValid)
            {


                var pro = db.Orders.FirstOrDefault(x => x.OrderId == dell.OrderId);

                int pec = pro.OrderId;
                string Dest = pro.Destination;

                tripDTO.Destination = Dest;
                tripDTO.PickUpAddress = "C4004 ILLOVU TOWNSHIP 4126 AMANZIMTOTI SOUTH AFRICA";
                tripDTO.OrderId = pec;
                tripDTO.PickUpTime = tripDTO.PickUpTime;
                tripDTO.VehicleId = dell.VehicleId;
                tripDTO.Date = DateTime.UtcNow.AddHours(2);
                tripDTO.DriverId = dell.DriverId;
                tripDTO.DriverConfirm = "NEW";

                db.Deliveries.Add(tripDTO);
                db.SaveChanges();


                Vehicle vh = db.Vehicles.Find(tripDTO.VehicleId);
                vh.Status = "ALLOCATED";
                db.SaveChanges();

                User d = db.Users.Find(tripDTO.DriverId);
                d.AvailabilityStatus = "ALLOCATED";
                db.SaveChanges();




                var dbContext = new Db();
                // Update Shipping Status

                // Retrieve a order from the database
                var orderr = dbContext.Set<Order>().Find(tripDTO.OrderId);

                // Update a property on your user
                orderr.DeliveryStatus = "SHIPPED";
                dbContext.SaveChanges();


                string _sender = "21817974@dut4life.ac.za";
                string _password = "Dut990310";

                string recipient = tripDTO.DriverName;
                SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential(_sender, _password);
                client.EnableSsl = true;
                client.Credentials = credentials;
                try
                {
                    var mail = new MailMessage(_sender.Trim(), recipient.Trim());
                    mail.Subject = "NEW DELIVERY ASIGNED TO YOU";
                    mail.Body = "<HTML><BODY><p>PLEASE NOTE: NEW Delivery</p><br><br>" + "<div align='center'>DELIVERY DETAILS<br />From:" + tripDTO.PickUpAddress + "<br />To:" + tripDTO.Destination + "<br />DATE and TIME: " + tripDTO.PickUpTime + "<br/> <h2>PLEASE APPROVE ALLOCATION IF YOU ARE AVAILABLE FOR THIS DELIVERY</h2><br/><a href='/admin/admin/Approoveallocation?id=" + tripDTO + "' class='btn btn-success'>APPROVE ALLOCATION</a><br/> <a href='/admin/admin/rejectallocation?id=" + tripDTO + "'>REJECT ALLOCATION</a> </ div > " + " <br></BODY></HTML>";
                    mail.IsBodyHtml = true;
                    client.Send(mail);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw ex;
                }

            }

            return RedirectToAction("Refunds");
        }








        [HttpGet]
        public ActionResult ApproveRefund(int id)
        {
            Refund reff = db.Refunds.Find(id);

            return View(reff);
        }








        [HttpPost]
        public ActionResult ApproveRefund(int id,Refund f , int Credit)
        {
            Refund reff = db.Refunds.Find(id);
            reff.Status = "APPROVED";
            db.SaveChanges();

            int usid = db.Users.Where(x => x.EmailAddress == reff.CustomerEmail).FirstOrDefault().Id;

            User us = db.Users.Find(usid);
            us.Credit = us.Credit + Credit;
            db.SaveChanges();





            string _senderr = "21817974@dut4life.ac.za";
            string _passwordr = "Dut990310";

            string recipientr = us.EmailAddress;
            SmtpClient clientr = new SmtpClient("smtp-mail.outlook.com");

            clientr.Port = 587;
            clientr.DeliveryMethod = SmtpDeliveryMethod.Network;
            clientr.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentialsr =
                new System.Net.NetworkCredential(_senderr, _passwordr);
            clientr.EnableSsl = true;
            clientr.Credentials = credentialsr;
            try
            {
                var mail = new MailMessage(_senderr.Trim(), recipientr.Trim());
                mail.Subject = "refund approved";
                mail.Body = "<HTML><BODY><p><div align='centre'>" + "PLEASE NOTE THAT YOUR REFUND HAS BEEN APPROVED AND YOUR ACCOUNT HAS BEEN CREDITED" + "</div></h2></BODY></HTML>";
                mail.IsBodyHtml = true;
                clientr.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

            return RedirectToAction("Refunds");
        }









        public ActionResult PickUpPoints()
        {
            var pic = db.PickPoints.ToList();
            return View(pic);
        }
        







        [HttpGet]
        public ActionResult AddPickPoint(PickPointVM model)
        {
            model.Drivers = new SelectList(from n in db.Users
                                           join i in db.UserRoles
                                           on n.Id equals i.UserId
                                           where i.RoleId == 3
                                           select n.EmailAddress);
            model.Vihicles = new SelectList(db.Vehicles.ToList(), "Id", "NumberPlate");
            return View(model);
        }








        [HttpPost]
        public ActionResult AddPickPoint(PickPointVM mm,PickPoint m)
        {
            Vehicle v = db.Vehicles.Find(mm.PickVehId);
            m = new PickPoint()
            {
                DriverEmail = mm.DriverEmail,
                PickVehId = mm.PickVehId,
                NumberPlate = v.NumberPlate,
                PickUpPhone = mm.PickUpPhone,
                PointAddress = mm.PointAddress,
            };
            db.PickPoints.Add(m);
            db.SaveChanges();

            return RedirectToAction("AddPickPoint");
        }








        public ActionResult Index()
        {
            return View();
        }








        public ActionResult CollectOrder(int id)
        {
            Order order = db.Orders.Find(id);

            return View(order);
        }








        public ActionResult ConfirmOrderCollect(string qr,int ordid)
        {
            var orders = db.Orders.Where(x => x.OrderCode == qr && x.OrderId == ordid && x.Status == "PAID").FirstOrDefault();
            if(orders != null)
            {
                Order ord = db.Orders.Find(ordid);
                ord.DeliveryStatus = "COLLECTED";
                db.SaveChanges();
                string _senderr = "21817974@dut4life.ac.za";
                string _passwordr = "Dut990310";

                string recipientr = ord.User.EmailAddress;
                SmtpClient clientr = new SmtpClient("smtp-mail.outlook.com");

                clientr.Port = 587;
                clientr.DeliveryMethod = SmtpDeliveryMethod.Network;
                clientr.UseDefaultCredentials = false;
                System.Net.NetworkCredential credentialsr =
                    new System.Net.NetworkCredential(_senderr, _passwordr);
                clientr.EnableSsl = true;
                clientr.Credentials = credentialsr;
                try
                {
                    var mail = new MailMessage(_senderr.Trim(), recipientr.Trim());
                    mail.Subject = "ORDER COLLECTED";
                    mail.Body = "<HTML><BODY><p><div align='centre'>" + "PLEASE NOTE THAT YOUR ORDER HAS BEEN COLLECTED" + "</div></h2></BODY></HTML>";
                    mail.IsBodyHtml = true;
                    clientr.Send(mail);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw ex;
                }

                return Redirect("/admin/admin/OrderCollected?id="+ordid);
            }
            else
            {
                return RedirectToAction("OrderCollectFailed");
            }


        }







        public ActionResult OrderCollected(int id)
        {
            Order ord = db.Orders.Find(id);
            return View(ord);
        }







        public ActionResult OrderCollectFailed()
        {
            return View();
        }






        public ActionResult Deliveries()
        {
            return View(db.Deliveries.ToList());
        }






        public ActionResult ScheduleDelivery(int ordId)
        {
            return View();
        }







        public ActionResult Vehicles()
        {
            return View(db.Vehicles.ToList());
        }








        public ActionResult AddVehicle()
        {
            return View();
        }







        [HttpPost]
        public ActionResult AddVehicle(Vehicle model)
        {

            if(!db.Vehicles.Any(x => x.NumberPlate.Equals(model.NumberPlate)))
            {
                Vehicle v = new Vehicle()
                {
                    NumberPlate = model.NumberPlate,
                    Status ="AVAILABLE"
                };
                db.Vehicles.Add(v);
                db.SaveChanges();
                TempData["Success"] = "Vehicle added";

                return RedirectToAction("AddVehicle");
            }

            else
            {
                TempData["Error"] = "Vehicle exist";

                return RedirectToAction("AddVehicle");

            }

        }






        [HttpPost]
        public ActionResult UpDateVehicle(Vehicle model,int id)
        {
            Vehicle v = db.Vehicles.Find(id);
            v.NumberPlate = model.NumberPlate;
            db.SaveChanges();

            TempData["Success"] = "Vehicle update";

            return RedirectToAction("AddVehicle");

        }








        [HttpGet]
        public ActionResult AddPickDelivery(PickVM model,int id)
        {
            ViewBag.OrderId = new SelectList(from n in db.Orders
                                             where n.OrderId == id
                                             select n.OrderId);

            ViewBag.Destination = new SelectList(from n in db.Orders
                                                 where n.OrderId == id
                                                 select n.Destination);

            ViewBag.PickUpAddress = new SelectList(from n in db.Orders
                                                   where n.OrderId == id
                                                   select n.Destination);


            model.PickPoints = new SelectList(db.PickPoints.ToList(), "Id", "PointAddress");

            return View(model);
        }








        [HttpPost]
        public ActionResult AddPickDelivery(Delivery t,PickVM model,int id)
        {

                 int pec = id;
                  int pointid = int.Parse(model.PickUpAddress);
                var pics = db.PickPoints.Where(x => x.Id == pointid).FirstOrDefault();

                Order ord = db.Orders.Find(pec);

                string Dest = model.PickUpAddress;

                int vid = pics.PickVehId;

                int did = db.Users.Where(x => x.EmailAddress == pics.DriverEmail).FirstOrDefault().Id;


                Delivery tripDTO = new Delivery();
                tripDTO.Destination = pics.PointAddress;
                tripDTO.PickUpAddress = "C4004 ILLOVU TOWNSHIP 4126 AMANZIMTOTI SOUTH AFRICA";
                tripDTO.OrderId = pec;
                tripDTO.PickUpTime = model.PickUpTime;
                tripDTO.VehicleId = vid;
                tripDTO.Date = DateTime.UtcNow.AddHours(2);
                 tripDTO.NumberPlate = pics.NumberPlate;
                tripDTO.DriverId = did;
                tripDTO.DriverName = pics.DriverEmail;
                tripDTO.DriverConfirm = "NEW";

                db.Deliveries.Add(tripDTO);
                db.SaveChanges();


                Vehicle vh = db.Vehicles.Find(tripDTO.VehicleId);
                vh.Status = "ALLOCATED";
                db.SaveChanges();

                User d = db.Users.Find(tripDTO.DriverId);
                d.AvailabilityStatus = "ALLOCATED";
                db.SaveChanges();




                var dbContext = new Db();
                // Update Shipping Status

                // Retrieve a order from the database
                var orderr = dbContext.Set<Order>().Find(1);

                // Update a property on your user
                orderr.DeliveryStatus = "SHIPPED";
                dbContext.SaveChanges();


                string _sender = "21817974@dut4life.ac.za";
                string _password = "Dut990310";

                string recipient = tripDTO.DriverName;
                SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential(_sender, _password);
                client.EnableSsl = true;
                client.Credentials = credentials;
                try
                {
                    var mail = new MailMessage(_sender.Trim(), recipient.Trim());
                    mail.Subject = "NEW DELIVERY ASIGNED TO YOU";
                    mail.Body = "<HTML><BODY><p>PLEASE NOTE: NEW Delivery</p><br><br>" + "<div align='center'>DELIVERY DETAILS<br />From:" + tripDTO.PickUpAddress + "<br />To:" + tripDTO.Destination + "<br />DATE and TIME: " + tripDTO.PickUpTime + "<br/> <h2>PLEASE APPROVE ALLOCATION IF YOU ARE AVAILABLE FOR THIS DELIVERY</h2><br/><a href='https://2021grp33.azurewebsites.net/admin/admin/Approoveallocation?id=" + tripDTO.Id + "' class='btn btn-success'>APPROVE ALLOCATION</a><br/> <a href='https://2021grp33.azurewebsites.net/admin/admin/rejectallocation?id=" + tripDTO.OrderId + "'>REJECT ALLOCATION</a> </ div > " + " <br></BODY></HTML>";
                    mail.IsBodyHtml = true;
                    client.Send(mail);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw ex;
                }

            

            ViewBag.DriverName = new SelectList(from n in db.Users
                                                join i in db.UserRoles
                                                on n.Id equals i.UserId
                                                where i.RoleId == 3
                                                select n.EmailAddress);

            ViewBag.NumberPlate = new SelectList(db.Vehicles, "NumberPlate", "NumberPlate");

            TempData["Success"] = "Delivery created & driver Notified";

            return RedirectToAction("Deliveries");
        }









        public ActionResult Approoveallocation(int id)
        {
            Delivery del = db.Deliveries.Find(id);
            del.DriverConfirm = "APPROVED";
            db.SaveChanges();

            return Redirect("/Driver/NewDeliveries");
        }








        public ActionResult rejectallocation(int id)
        {
            Delivery del = db.Deliveries.Find(id);
            del.DriverConfirm = "REJECTED";
            db.SaveChanges();

            return Redirect("/Driver/NewDeliveries");
        }







        [HttpGet]
        public ActionResult AddDelivery(int? id)
        {
            ViewBag.OrderId = new SelectList(from n in db.Orders
                                             where n.OrderId == id
                                             select n.OrderId);

            ViewBag.Destination = new SelectList(from n in db.Orders
                                                 where n.OrderId == id
                                                 select n.Destination);

            ViewBag.PickUpAddress = new SelectList(from n in db.Orders
                                                   where n.OrderId == id
                                                   select n.Destination);


            ViewBag.DriverName = new SelectList(from n in db.Users
                                                join i in db.UserRoles
                                                on n.Id equals i.UserId 
                                                where i.RoleId == 3 
                                                select n.EmailAddress);
            ViewBag.NumberPlate = new SelectList(db.Vehicles.Where(x => x.Status =="AVAILABLE"), "NumberPlate", "NumberPlate");

            return View();
        }







        [HttpPost]
        public ActionResult AddDelivery([Bind(Include = "Id,OrderId,NumberPlate,OrderDetailsId,Controller,DriverName,Destination,PickUpAddress")] Delivery tripDTO, int? id,DateTime PickUpTime)
        {
            if (ModelState.IsValid)
            {

                var pro = db.Orders.FirstOrDefault(x => x.OrderId == id);

                int pec = pro.OrderId;
                string Dest = pro.Destination;

                tripDTO.Destination = Dest;
                tripDTO.PickUpAddress = "C4004 ILLOVU TOWNSHIP 4126 AMANZIMTOTI SOUTH AFRICA";
                tripDTO.OrderId = pec;
                tripDTO.PickUpTime = PickUpTime;
                tripDTO.VehicleId = db.Vehicles.Where(x => x.NumberPlate == tripDTO.NumberPlate).FirstOrDefault().Id;
                tripDTO.Date = DateTime.UtcNow.AddHours(2);
                tripDTO.DriverId = db.Users.Where(x => x.EmailAddress == tripDTO.DriverName).FirstOrDefault().Id;

                db.Deliveries.Add(tripDTO);
                db.SaveChanges();

             



                string _sender = "21817974@dut4life.ac.za";
                string _password = "Dut990310";

                string recipient = tripDTO.DriverName;
                SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential(_sender, _password);
                client.EnableSsl = true;
                client.Credentials = credentials;
                try
                {
                    var mail = new MailMessage(_sender.Trim(), recipient.Trim());
                    mail.Subject = "NEW DELIVERY ASIGNED TO YOU";
                    mail.Body = "<HTML><BODY><p>PLEASE NOTE: NEW Delivery</p><br><br>" + "<div align='center'>DELIVERY DETAILS<br />From:" + tripDTO.PickUpAddress + "<br />To:" + tripDTO.Destination + "<br />DATE and TIME: " + tripDTO.PickUpTime + "<br/> <h2>PLEASE APPROVE ALLOCATION IF YOU ARE AVAILABLE FOR THIS DELIVERY</h2><br/><a href='https://2021grp33.azurewebsites.net/admin/admin/Approoveallocation?id=" + tripDTO.Id + "' class='btn btn-success'>APPROVE ALLOCATION</a><br/> <a href='https://2021grp33.azurewebsites.net/admin/admin/rejectallocation?id=" + tripDTO.Id + "'>REJECT ALLOCATION</a> </ div > " + " <br></BODY></HTML>";
                    mail.IsBodyHtml = true;
                    client.Send(mail);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw ex;
                }

            }
            

            ViewBag.DriverName = new SelectList(from n in db.Users
                                                join i in db.UserRoles
                                                on n.Id equals i.UserId
                                                where i.RoleId == 3
                                                select n.EmailAddress);

            ViewBag.NumberPlate = new SelectList(db.Vehicles, "NumberPlate", "NumberPlate");

            TempData["Success"] = "Delivery created & driver Notified";

            return RedirectToAction("Deliveries");

        }








        public ActionResult allocate(int id)
        {
                 BookingVM model;
                //Get The product
                Booking dto = db.Bookings.Find(id);

                //Make Sure Product exist
                model = new BookingVM(dto);
                //Make A select List
                model.Constractors = new SelectList(from n in db.Users
                                                    join i in db.UserRoles
                                                    on n.Id equals i.UserId
                                                    where i.RoleId == 3
                                                    select n.EmailAddress);
            model.InspectionDate = dto.InspectionDate;
            model.ServicingDate = dto.ServicingDate;
                //Get All Gallery Images
            return View(model);
        }







        [HttpPost]
        public ActionResult allocate(BookingVM model,int id)
        {
            Booking b = db.Bookings.Find(id);
            b.Constractor = model.Constractor;
            b.Satatus = "ALLOCATED";
            b.StatusNum = 3;
            db.SaveChanges();


         

            string _sender = "21817974@dut4life.ac.za";
            string _password = "Dut990310";
           

            string recipient = b.Constractor;
            SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential(_sender, _password);
            client.EnableSsl = true;
            client.Credentials = credentials;
            try
            {
                var mail = new MailMessage(_sender.Trim(), recipient.Trim());
                mail.Subject = "INSPECTION ALLOCATED";
                mail.Body = "<HTML><BODY><p><div align='centre'>Date:  "+b.InspectionDate +" ADDRESS:  "+ b.Address+ "SERVICE TYPE: " +b.Servicetype +"</h2></BODY></HTML>";
                mail.IsBodyHtml = true;
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }

            return RedirectToAction("Bookings");
        }








        public ActionResult Approvebooking(int id)
        {
            GetQuery query = new GetQuery();

            Booking b = db.Bookings.Find(id);
            b.Satatus = "APPROVED";
            b.StatusNum = 2;
            db.SaveChanges();


            int orderid = id;
            string Message =  query.Main();



            Booking ordd = db.Bookings.Find(orderid);

            ordd.Qr = Message;
            db.SaveChanges();



            QRCodeGenerator ObjQr = new QRCodeGenerator();

            QRCodeData qrCodeData = ObjQr.CreateQrCode(Message, QRCodeGenerator.ECCLevel.Q);

            Bitmap bitMap = new QRCode(qrCodeData).GetGraphic(20);



            using (MemoryStream ms = new MemoryStream())
            {

                bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                byte[] byteImage = ms.ToArray();

                ViewBag.Url = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                bitMap.Save(Server.MapPath("~/images/Verify/" + Message+".png"), System.Drawing.Imaging.ImageFormat.Png);
            }







            string sender = "21817974@dut4life.ac.za";
            string password = "Dut990310";



            string recipient = ordd.Email;
            SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(sender, password);
            client.EnableSsl = true;
            client.Credentials = credentials;
            Attachment data = new Attachment(Server.MapPath("~/Images/Verify/" + Message+".png" ));


            var mail = new MailMessage(sender.Trim(), recipient.Trim());

            mail.Subject = "BOOKING APPROVED";
            mail.Body = "Plese  find  Attachement";
            mail.Attachments.Add(data);
            client.Send(mail);

            TempData["Success"] = "Booking Code!!";



            return Redirect("/admin/admin/allocate?id=" + id);




        }








        public ActionResult Bookings()
        {
            var boo = db.Bookings.Where(x => x.StatusNum ==1).ToList();
            return View(boo);
        }








        public ActionResult ApprovedBookings()
        {
            var apbo = db.Bookings.Where(x => x.StatusNum != 1).ToList();

            return View(apbo);
        }








        public ActionResult Categories()
        {
            var cats = db.Categories.ToList();
            return View(cats);
        }









        [HttpGet]
        public ActionResult AddCategory()
        {
            return View();
        }









        [HttpPost]
        public ActionResult AddCategory(Category ca)
        {

            if (db.Categories.Any(x => x.Name.Replace("&", "-").Replace(" ", "-").ToLower().Equals(ca.Name.Replace("&", "-").Replace(" ", "-").ToLower())))
            {
                TempData["Error"] = "Category exist";
                return View();
            }
            {
                Category cat = new Category()
                {
                    Name = ca.Name,
                    NameFix = ca.Name.Replace("&", "-").Replace(" ", "-").ToLower(),
                };

                db.Categories.Add(cat);
                db.SaveChanges();

                TempData["Success"] = "New category added";
                return RedirectToAction("AddCategory");
            }

        }










        [HttpPost]
        public ActionResult updatecategory(int id, string Name)
        {

            Category car = db.Categories.Find(id);
            car.Name = Name;
            car.NameFix.Replace("&", "-").Replace(" ", "-").ToLower();
            db.SaveChanges();

            TempData["Success"] = "Category updated";
            return RedirectToAction("AddCategory");
        }









        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult AddProduct()
        {

            //Intialize Model
            ProductVM model = new ProductVM();
            //Add Select List Of Categories to Model
            using (Db db = new Db())
            {

                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            //Return view With Model
            return View(model);
        }











        [Authorize(Roles = "Admin")]
        //POST: Admin/shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            GetQuery query = new GetQuery();
            //check model state
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {

                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }

            }
            //make sure product name is unique
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "PAint exists!");
                    return View(model);
                }

            }
            //declare product id
            int id;
            //initilize and save productDTO
            using (Db db = new Db())
            {
                Product product = new Product();
                product.Name = model.Name;
                product.Query = model.Name.Replace(" ", "").Replace("&", "") + query.Main();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;
                product.Quantity = model.Quantity;
                product.color = model.Color;
                product.Liters = model.Liters;


                Category catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();

                //Get the id
                id = product.Id;
            }

            //set tempdata messsage
            TempData["Success"] = "New paint added!";

            #region Upload Image

            //create necessary directories
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
            {
                Directory.CreateDirectory(pathString1);
            }

            if (!Directory.Exists(pathString2))
            {
                Directory.CreateDirectory(pathString2);
            }
            if (!Directory.Exists(pathString3))
            {
                Directory.CreateDirectory(pathString3);
            }
            if (!Directory.Exists(pathString4))
            {
                Directory.CreateDirectory(pathString4);
            }
            if (!Directory.Exists(pathString5))
            {
                Directory.CreateDirectory(pathString5);
            }

            //check if a file was  uploaded
            if (file != null && file.ContentLength > 0)
            {
                //get file extension
                string ext = file.ContentType.ToLower();
                //verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {

                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "Wrong Image Extension !");
                        return View(model);


                    }
                }
                //initilize image name
                string imageName = file.FileName;
                //save mage name to DTO
                using (Db db = new Db())
                {
                    Product dto = db.Products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }
                //set orignial and thumb image path
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                //save original image
                file.SaveAs(path);
                //create and save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }
            #endregion

            //Redirect
            return RedirectToAction("AddProduct");
        }










        public ActionResult ProductDetails(string qname)
        {

            //Declare VM and DTO
            ProductVM model;
            Product dto;
            //Initialize Product id
            int id = 0;
            using (Db db = new Db())
            {
                //check if product exist
                if (!db.Products.Any(x => x.Query.Equals(qname)))
                {
                    return RedirectToAction("Products", "Admin");
                }
                //Initialize ProductDTO
                dto = db.Products.Where(x => x.Query == (qname)).FirstOrDefault();
                //Get  Id
                id = dto.Id;
                //Initialize Model
                model = new ProductVM(dto);



            }
          
            return View("ProductDetails", model);
        }










        [Authorize(Roles = "Admin")]
        public ActionResult DeleteProduct(int id)
        {
            //delete product from DB
            using (Db db = new Db())
            {
                Product dto = db.Products.Find(id);
                db.Products.Remove(dto);
                db.SaveChanges();

            }

            //delete Product Folder
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
            string pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
            {
                Directory.Delete(pathString, true);

            }
            //redirect

            return RedirectToAction("/Products");
        }










        [Authorize(Roles = "Admin")]
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









        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditProduct(int id)
        {

            //Declare productVM
            ProductVM model;
            using (Db db = new Db())
            {
                //Get The product
                Product dto = db.Products.Find(id);
                //Make Sure Product exist
                if (dto == null)
                {
                    return Content("Paint not availabel!");
                }
                //Intialize 

                model = new ProductVM(dto);
                //Make A select List
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                //Get All Gallery Images
                model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                       .Select(fn => Path.GetFileName(fn));
            }
            //return View with model
            return View(model);
        }









        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            GetQuery query = new GetQuery();

            //get product id
            int id = model.Id;
            //populate categories select list and gallery images
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

            }
            model.GalleryImages = Directory.EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                                     .Select(fn => Path.GetFileName(fn));
            //check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //make sure product name is unique
            using (Db db = new Db())
            {
                if (db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "Paint exist!");
                    return View(model);

                }

            }
            //update product
            using (Db db = new Db())
            {
                Product dto = db.Products.Find(id);

                dto.Name = model.Name;
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;
                dto.Quantity = model.Quantity;
                dto.Query = model.Name.Replace(" ", "").Replace("&", "") + query.Main();
                dto.Liters = model.Liters;


                Category catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;
                db.SaveChanges();
            }
            //Set TempData message
            TempData["Success"] = "Paint info updated!";


            #region Image Upload

            //check if a file was  uploaded
            if (file != null && file.ContentLength > 0)
            {
                //get  extension
                string ext = file.ContentType.ToLower();
                //verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {

                        ModelState.AddModelError("", "Wrong Image Extension !");
                        return View(model);


                    }
                }
                //Set upload Directory Paths
                var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));
                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                //Delete Files From Directories
                DirectoryInfo di1 = new DirectoryInfo(pathString1);

                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (FileInfo file2 in di1.GetFiles())
                    file2.Delete();

                foreach (FileInfo file3 in di2.GetFiles())
                    file3.Delete();
                //save Image Name

                string imageName = file.FileName;
                using (Db db = new Db())
                {
                    Product dto = db.Products.Find(id);
                    dto.ImageName = imageName;
                    db.SaveChanges();
                }
                //Save orignial and thumb images

                var path = string.Format("{0}\\{1}", pathString1, imageName);
                var path2 = string.Format("{0}\\{1}", pathString2, imageName);

                file.SaveAs(path);

                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);

            }



            #endregion

            //Redirect
            return RedirectToAction("EditProduct");
        }









        public ActionResult Drivers()
        {
            List<UserVM> drivers = new List<UserVM>();

            var userroles = db.UserRoles.Where(x => x.RoleId == 3).ToList();
            foreach(var item in userroles)
            {
                var driver = db.Users.Where(x => x.Id == item.UserId).ToList();
                foreach( var row in driver)
                {
                    drivers.Add(new UserVM()
                    {
                        FirstName = row.FirstName,
                        LastName = row.LastName,
                        EmailAddress = row.EmailAddress,
                        Password = row.Password,
                    });
                }
                
            }
           
            return View(drivers);
        }









        [HttpGet]
        public ActionResult Adddriver()
        {
            return View();
        }









        [Authorize]
        public ActionResult Orders()
        {



            //Intialize List Of OrdersForUserVM
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();

            using (Db db = new Db())
            {
                //get user id
                User user = db.Users.Where(x => x.EmailAddress == User.Identity.Name).FirstOrDefault();
                int userId = user.Id;
                //Intialize List Of OrderVM
                List<OrderVM> orders = db.Orders.Where(x =>x.Status == "PAID" && x.DeliveryStatus!= "DELIVERED" && x.DeliveryStatus !="COLLECTED").ToArray().Select(x => new OrderVM(x)).ToList();

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

                    }


                    ordersForUser.Add(new OrdersForUserVM()
                    {
                        OrderNumber = order.OrderId,
                        Total = total,
                        ProductsAndQty = productsAndQty,
                        CreatedAt = order.CreatedAt,
                        Status = order.Status,
                        DeliveryFee = order.DeliveryFee,
                        DeliveryStatus = order.DeliveryStatus,
                    });
                }

            }

            return View(ordersForUser);
        }









        public ActionResult ProcessedOrders()
        {
            //Intialize List Of OrdersForUserVM
            List<OrdersForUserVM> ordersForUser = new List<OrdersForUserVM>();

            using (Db db = new Db())
            {
                //get user id
                User user = db.Users.Where(x => x.EmailAddress == User.Identity.Name).FirstOrDefault();
                int userId = user.Id;
                //Intialize List Of OrderVM
                List<OrderVM> orders = db.Orders.Where(x => x.DeliveryStatus == "DELIVERED"|| x.DeliveryStatus=="COLLECTED").ToArray().Select(x => new OrderVM(x)).ToList();

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

                    }

                    //Add to ordersforuserVM List
                    ordersForUser.Add(new OrdersForUserVM()
                    {

                        OrderNumber = order.OrderId,
                        Total = total,
                        ProductsAndQty = productsAndQty,
                        CreatedAt = order.CreatedAt,
                        Status = order.Status,
                        DeliveryStatus = order.DeliveryStatus,
                        DeliveryFee = order.DeliveryFee,
                        

                    });
                }

            }

            //Return View With List Of OrdersForUserVM

            return View(ordersForUser);
        }









        [HttpPost]
        public ActionResult Adddriver(UserVM model)
        {

            if (!ModelState.IsValid)
            {
                return View("Adddriver", model);
            }
            //check if password match
            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password Does not Match");
                return View("Adddriver", model);
            }

            int idd;
           
                //make sure username is unique
                if (db.Users.Any(x => x.EmailAddress.Equals(model.EmailAddress)))
                {
                    ModelState.AddModelError("", "Please provide a valid Email address");
                    model.EmailAddress = "";
                    return View("Adddriver", model);
                }


                UserRole userRolesDTO = new UserRole()
                {
                    RoleId = 3

                };

                db.UserRoles.Add(userRolesDTO);
                db.SaveChanges();

                int id = userRolesDTO.UserId;
                idd = id;


                //create userDTO
                User userDTO = new User()
                {
                    Id = id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAddress = model.EmailAddress,
                    Password = model.Password,
                    PhoneNumber = model.PhoneNumber,
                    Status = "VERIFIED",
                    AvailabilityStatus ="AVAILABLE"
                    
                };
                //Add The DTO
                db.Users.Add(userDTO);
                //Save
                db.SaveChanges();
            TempData["Success"] = "Driver added!";

            return RedirectToAction("Adddriver");
        }




        public ActionResult CustomerQuer()
        {
            var qry = db.Reviews.ToList();
            return View(qry);
        }


        public ActionResult Reviews()
        {
            var rev = db.ProductReviews.ToList();

            return View(rev);
        }
    }
}