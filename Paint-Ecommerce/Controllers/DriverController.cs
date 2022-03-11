using iTextSharp.text;
using iTextSharp.text.pdf;
using Paint_Ecommerce.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

namespace Paint_Ecommerce.Controllers
{
    public class DriverController : Controller
    {
        Db db = new Db();


        [Authorize]
        public ActionResult NewDeliveries()
        {
            return View(db.Deliveries.Where(x => x.DriverName ==User.Identity.Name));
        }

        public ActionResult StartDelivery(int id)
        {
            Delivery del = db.Deliveries.Find(id);

            Order ord = db.Orders.Find(del.OrderId);

            string _sender = "21817974@dut4life.ac.za";
            string _password = "Dut990310";

            string recipient = ord.User.EmailAddress;
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
                mail.Subject = "ORDER OUT FOR DELIVERY";
                mail.Body = "<HTML><BODY><p>Plese NOTE: Your Order is out for Delivery</p><br><br>" + "<br />DATE and TIME: "  + "</ div > " + " <br></BODY></HTML>";
                mail.IsBodyHtml = true;
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }



            return View(del);
        }

        [HttpGet]
        public ActionResult SignDelivery(int id)
        {
            Delivery del = db.Deliveries.Find(id);

            return View(del);
        }



      

        public ActionResult SignDeliveryPost(int ordid ,string qr, int delid)
        {

            var orders = db.Orders.Where(x => x.OrderId == ordid && x.OrderCode == qr).FirstOrDefault();
            Delivery del = db.Deliveries.Find(delid);
            if(orders != null)
            {
                int ordidd = orders.OrderId;
                Order order = db.Orders.Find(ordidd);
                order.DeliveryStatus = "DELIVERED";
                db.SaveChanges();

                Vehicle vh = db.Vehicles.Find(del.VehicleId);
                vh.Status = "AVAILABLE";
                db.SaveChanges();

                User d = db.Users.Find(del.DriverId);
                d.AvailabilityStatus = "AVAILABLE";
                db.SaveChanges();



                return Redirect("/Driver/SignResult?id=" +ordid);
            }
            else
            {
                return Redirect("/Driver/Signfailed?id=" + ordid);
            }


        }



        public ActionResult SignResult(int id)
        {
            return View();
        }


        public ActionResult Signfailed(int id)
        {
            return View();
        }

        
        public ActionResult PickUpOrder(int id)
        {
            Delivery del = db.Deliveries.Find(id);
            return View(del);
        }
        


        public ActionResult Picked(string qr,int ordid,int delid)
        {
            var or = db.Orders.Where(x => x.OrderCode == qr && x.OrderId == ordid).ToList();

            if(or!= null)
            {
                Order o = db.Orders.Find(ordid);
                o.DeliveryStatus = "PICKED";
                db.SaveChanges();

                return RedirectToAction("NewDeliveries");
            }

            else
            {
                return Redirect("/Driver/PickUpOrder?id=" + delid);
            }

        }

        public ActionResult PlanRoute()
        {
            var or = db.Deliveries.Where(x => x.DriverName == User.Identity.Name).ToList();

            List<CodinateVM> cod = new List<CodinateVM>();


            foreach(var item in or)
            {

                string url = "https://maps.googleapis.com/maps/api/geocode/json?key=AIzaSyAYgA0WDcNBnR7ewQIWEqUbNmDUdnHCL9M&address=" + "14 PINE STRRET DURABN 4001 SOUTH AFRICA"+ "&sensor=true";


                RestClient client = new RestClient(url);
                RestRequest request = new RestRequest(Method.GET);
                request.Timeout = 1000000;
                request.ReadWriteTimeout = 1000000;
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                var response = client.Execute(request);
                JavaScriptSerializer oJS = new JavaScriptSerializer();
                oJS.MaxJsonLength = int.MaxValue;

                var oRootObject = oJS.Deserialize<MapsResponse.Rootobject>(response.Content);

                string lati = string.Format("{0}", oRootObject.results[0].geometry.location.lat);
                string longi = string.Format("{0}", oRootObject.results[0].geometry.location.lng);

                cod.Add(new CodinateVM()
                {
                    Latitude = lati,
                    Longitude = longi,
                    OrdId = item.OrderId,
                    Address = item.Destination,
                });

            }

            return View(cod);
        }




        public ActionResult Inspections()
        {
            return View(db.Bookings.Where(x => x.Constractor == User.Identity.Name).ToList());
        }


        public ActionResult StInspect(int id)
        {

            return View();
        }

        public ActionResult GetCalcate(string qr, int heti)
        {


            return View();
        }



        public ActionResult StartInspection(string qr)
        {
            var bo = db.Bookings.Where(x => x.Qr == qr).FirstOrDefault();

            if(bo!= null)
            {
                int bid = bo.Id;
                Booking bbo = db.Bookings.Find(bid);
                bo.InspectinStatus = "INSPECTION STARTED";
                bo.StatusNum = 4;
                db.SaveChanges();
            }

            return RedirectToAction("Inspections");
        }


        public ActionResult ScanQr(int id)
        {
            ViewBag.Id = id;
            return View();
        }


        public ActionResult ApproveInspection(string qr, int id,int Area)
        {

            var ins = db.Bookings.Where(x => x.Qr == qr && x.Id == id).FirstOrDefault();

            if (ins != null)
            {
                Booking bo = db.Bookings.Find(id);
                bo.Satatus = "INSPECTED";
                bo.StatusNum = 5;
                bo.Total = bo.Total + Area;
                db.SaveChanges();

                return Redirect("/Driver/ApprovedInspection" + id);


            }
            else
            {
                return Content("Qr code invalid");

            }


        }




        public ActionResult ApprovedInspection(int id)
        {


            var book = db.Bookings.Where(x => x.Id == id).FirstOrDefault();

            if(book != null)
            {



                GetQuery invoname = new GetQuery();
                string invon = invoname.Main().ToString();
                System.IO.FileStream fs = new FileStream(Server.MapPath("~/Images/") + invon + ".pdf", FileMode.Create);

                Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, fs);
                pdfDoc.Open();




                try
                {

                    Booking odd = db.Bookings.Find(id);
                    Chunk chunk = new Chunk(DateTime.UtcNow.AddHours(2).ToString(), FontFactory.GetFont("Arial", 5, iTextSharp.text.Font.BOLDITALIC, BaseColor.BLACK));
                    pdfDoc.Add(chunk);



                    Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    pdfDoc.Add(line);

                    PdfPTable table = new PdfPTable(5);
                    table.WidthPercentage = 100;
                    //0=Left, 1=Centre, 2=Right
                    table.HorizontalAlignment = 0;
                    table.SpacingBefore = 20f;
                    table.SpacingAfter = 30f;



                    PdfPCell cell = new PdfPCell();
                    cell.Border = 0;
                    Image image = Image.GetInstance(Server.MapPath("~/images/Verify/" + odd.Qr+".png"));
                    image.ScaleAbsolute(100, 100);
                    cell.AddElement(image);
                    table.AddCell(cell);




                    chunk = new Chunk("BOOKING NUM: " + id + "\nDATE: \n" + odd.Id + "\nADDRESS: " + odd.Address + "\nBALANCE DUE :R "+odd.Total, FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
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
                    
                    
                    


                    chunk = new Chunk("DELCO \nILLOVU \nTOWNSHIP \nAMANZIMTOTI \nSOUTH AFRICA \nC4004\n", FontFactory.GetFont("Daytona Condensed Light", 8, iTextSharp.text.Font.NORMAL, BaseColor.BLACK));
                    cell = new PdfPCell();
                    cell.Border = 0;
                    var para1 = new Paragraph(chunk);
                    para1.Alignment = Element.ALIGN_RIGHT;
                    cell.AddElement(para1);
                    table.AddCell(cell);
                    pdfDoc.Add(table);





                    Paragraph para = new Paragraph();
                    para.Add("Please keep This Slip Safe");
                    pdfDoc.Add(para);
                    line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    pdfDoc.Add(line);
                    pdfWriter.CloseStream = false;
                    pdfDoc.Close();
                    pdfDoc.CloseDocument();
                    fs.Close();





                    string sender = "21817974@dut4life.ac.za";
                    string password = "Dut990310";
                    string recipient = odd.Email;
                    SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
                    client.Port = 587;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(sender, password);
                    client.EnableSsl = true;
                    client.Credentials = credentials;
                    Attachment data = new Attachment(Server.MapPath("~/Images/" + invon + ".pdf"));


                    var mail = new MailMessage(sender.Trim(), recipient.Trim());
                    mail.Subject = "INSPECTION INVOICE";
                    mail.Body = "Plese  find  Attachement";
                    mail.Attachments.Add(data);
                    client.Send(mail);



                    TempData["Success"] = "Invoice Sent!!";


                    return Redirect("/Driver/InspectionCOmplete");
                }


                catch (Exception e)
                {
                    return Content(e.ToString());
                }
            }

            else
            {
                return View();
            }

        }

        public ActionResult InspectionCOmplete()
        {
            return View();
        }



        public ActionResult StartSer(int id)
        {
            return View();
        }



        public ActionResult StartService(string qr, int id)
        {

            var bo = db.Bookings.Where(x => x.Qr == qr && x.Id == id).FirstOrDefault();
            if(bo != null)
            {
                Booking bbo = db.Bookings.Find(id);
                bbo.ServiceStatus = "STARTED";
                bbo.StatusNum = 7;
                db.SaveChanges();

                return View();

            }


            return Content("Qr code invalid");
        }



        public ActionResult ApproveSer(int id)
        {
            return View();
        }

        public ActionResult ApproveService(string qr , int id)
        {
            var bo = db.Bookings.Where(x => x.Qr == qr && x.Id == id).FirstOrDefault();
            if(bo != null)
            {
                Booking bbo = db.Bookings.Find(id);

                bbo.InspectinStatus = "COMPLETE";
                bbo.StatusNum = 8;
                db.SaveChanges();

                return RedirectToAction("ServiceCompleted");
            }


                return RedirectToAction("StartSer?id="+id);

        }

        public ActionResult ServiceCompleted()
        {
            return View();
        }
    }
}