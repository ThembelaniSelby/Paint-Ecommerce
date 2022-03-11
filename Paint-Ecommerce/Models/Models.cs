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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.IO;

namespace Paint_Ecommerce.Models
{
    public class Models
    {
    }
    public class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UserRole
    {
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Key, Column(Order = 1)]
        public int RoleId { get; set; }


        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
    }

    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [Display(Name = "First Name(s)")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]

        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public int Credit { get; set; }

        public string Status { get; set; }
        public string AvailabilityStatus { get; set; }



    }



    public class GetQuery
    {

        public string Main()
        {
            int length = 20;

            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }
            return str_build.ToString();
        }
    }

    public class Verify
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccountId { get; set; }
        public string Code { get; set; }

    }


    public class Address
    {
        [Key]

        public int Id { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public int Zip { get; set; }
        public string Addres { get; set; }
        public string Username { get; set; }
    }



    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameFix { get; set; }

    }
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]

        public string Name { get; set; }

        public string Query { get; set; }
        [Required]
        public int Quantity { get; set; }

        [AllowHtml]
        public string Description { get; set; }
        [Required]

        public int Price { get; set; }
        public string CategoryName { get; set; }
        public string ImageName { get; set; }
        public int CategoryId { get; set; }
        public string color { get; set; }
        public int Liters { get; set; }


        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

    }



    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int DeliveryFee { get; set; }
        [Display(Name = "Date")]

        public DateTime CreatedAt { get; set; }
        public string Destination { get; set; }
        public string Status { get; set; }
        [Display(Name = "Order code")]

        public string OrderCode { get; set; }
        [Display(Name = "Delivery status")]

        public string DeliveryStatus { get; set; }
        [Display(Name = "Total price")]

        public int TotalPrice { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Orders { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

    }


    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Qr { get; set; }
        public string Email { get; set; }
        public int PhoneNumber { get; set; }
        public string Address { get; set; }
        public string SUrbace { get; set; }
        public int ZipCode { get; set; }
        public string Servicetype { get; set; }
        public DateTime InspectionDate { get; set; }
        public DateTime ServicingDate { get; set; }
        public string Satatus { get; set; }
        public string Constractor { get; set; }
        public string InspectinStatus { get; set; }
        public string ServiceStatus { get; set; }
        public int Total { get; set; }

        public int StatusNum { get; set; }
    }

    public class Vehicle
    {
        [Key]
        public int Id { get; set; }
        public string NumberPlate { get; set; }
        public string Status { get; set; }
    }

    public class Delivery
    {
        [Key]
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int VehicleId { get; set; }
        [Display(Name = "Vehicle")]
        public string NumberPlate { get; set; }
        public int DriverId { get; set; }
        [Display(Name = "Driver Username")]
        public string DriverName { get; set; }
        public string Destination { get; set; }
        public DateTime PickUpTime { get; set; }
        public string PickUpAddress { get; set; }
        public DateTime Date { get; set; }

        public string DriverConfirm { get; set; }

        [ForeignKey("DriverId")]
        public virtual User Driver { get; set; }

        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; }
    }


    public class PickPoint
    {
        [Key]
        public int Id { get; set; }
        public string PointAddress { get; set; }
        public string DriverEmail { get; set; }
        public string PickUpPhone { get; set; }
        public int PickVehId { get; set; }
        public string NumberPlate { get; set; }
    }

    public class Refund
    {
        [Key]
        public int Id { get; set; }
        public string Reason { get; set; }
        public string Destination { get; set; }
        public string PickupAddress { get; set; }
        public int OrderNum { get; set; }
        public string CustomerEmail { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }

    }

    public class MapsResponse
    {
        public class Rootobject
        {
            public Result[] results { get; set; }
            public string status { get; set; }
        }

        public class Result
        {
            public Address_Components[] address_components { get; set; }
            public string formatted_address { get; set; }
            public Geometry geometry { get; set; }
            public string place_id { get; set; }
            public string[] types { get; set; }
        }

        public class Geometry
        {
            public Location location { get; set; }
            public string location_type { get; set; }
            public Viewport viewport { get; set; }
        }

        public class Location
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Viewport
        {
            public Northeast northeast { get; set; }
            public Southwest southwest { get; set; }
        }

        public class Northeast
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Southwest
        {
            public float lat { get; set; }
            public float lng { get; set; }
        }

        public class Address_Components
        {
            public string long_name { get; set; }
            public string short_name { get; set; }
            public string[] types { get; set; }
        }




    }


    public class Review
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Comment { get; set; }
        public string Email { get; set; }
        public string Service { get; set; }
    }

    public class ProductReview
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Comment { get; set; }
        public string Email { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Status { get; set; }
        public double Score { get; set; }
    }



    public class ApikeyServiceClientCredentials : ServiceClientCredentials
    {

      


        private readonly string subscriptionkey;

        public ApikeyServiceClientCredentials(string subscriptionkey)
        {
            this.subscriptionkey = subscriptionkey;
        }

        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentException("request");
            }

            request.Headers.Add("Ocp-Apim-Subscription-Key", this.subscriptionkey);
            return base.ProcessHttpRequestAsync(request, cancellationToken);
        }


    }



    public class Program
    {


        public int ProductId { get; set; }
        public string Comment { get; set; }
        public double Score { get; set; }
        public string Status { get; set; }


        private const string CogSecreet = "fb11a82e8fd448e895b9f6f9639c98dc";
        private const string EndPoint = "https://2021grp31text.cognitiveservices.azure.com/";


        public  string Main()
        {

            var credintials = new ApikeyServiceClientCredentials(CogSecreet);
            var client = new TextAnalyticsClient(credintials)
            {
                Endpoint = EndPoint
            };

            var InputData = new MultiLanguageBatchInput(
                new List<MultiLanguageInput>
                {
                    new MultiLanguageInput(ProductId.ToString(),Comment,"en"),

                });

            var results = client.SentimentBatch(InputData);


            foreach (var document in results.Documents)
            {

                Score =double.Parse(document.Score.ToString());

                if (document.Score > 0.5)
                {

                    Status = "Positive";

                }
                else
                {
                    Status = "Negative";
                }

            }

           return Status;


        }


        public double GetScore()
        {

            var credintials = new ApikeyServiceClientCredentials(CogSecreet);
            var client = new TextAnalyticsClient(credintials)
            {
                Endpoint = EndPoint
            };

            var InputData = new MultiLanguageBatchInput(
                new List<MultiLanguageInput>
                {
                    new MultiLanguageInput(ProductId.ToString(),Comment,"en"),

                });

            var results = client.SentimentBatch(InputData);


            foreach (var document in results.Documents)
            {
                Score = double.Parse(document.Score.ToString());
            }

            return Score;


        }

    }
}

public class Encr
{

    public static readonly byte[] salt = System.Text.Encoding.Unicode.GetBytes(gkhjfmgvkkqqwecmrvmfmgbfgllpodnbjgnx.lfgvnfknnamqnwcnfmvmnfv().ToString());
    private static readonly int iterations = 2000;


    public static string Decrypt(string cryptoText)
    {
        byte[] plainBytes;
        byte[] cryptoBytes = Convert.FromBase64String(cryptoText);
        var aes = Aes.Create();
        var pbkdf2 = new Rfc2898DeriveBytes("", salt, iterations);
        aes.Key = pbkdf2.GetBytes(32);
        aes.IV = pbkdf2.GetBytes(16);
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(cryptoBytes, 0, cryptoBytes.Length);
            }
            plainBytes = ms.ToArray();
        }
        return Encoding.Unicode.GetString(plainBytes);
    }


    public static string Encrypt(string plainText)
    {
        byte[] encryptedBytes;
        byte[] plainBytes = Encoding.Unicode.GetBytes(plainText);
        var aes = Aes.Create();
        var pbkdf2 = new Rfc2898DeriveBytes("", salt, iterations);

        aes.Key = pbkdf2.GetBytes(32); // set a 256-bit key

        aes.IV = pbkdf2.GetBytes(16); // set a 128-bit IV
        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(plainBytes, 0, plainBytes.Length);
            }
            encryptedBytes = ms.ToArray();
        }

        return Convert.ToBase64String(encryptedBytes);
    }
}      /// <summary>
       /// Key Phrases
       /// </summary>










