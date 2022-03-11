using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Paint_Ecommerce.Models
{
    public class ViewModels
    {
    }
    public class LoginUserVM
    {
        [Required]
        [Display(Name = "Email")]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }


    public class CodinateVM
    {
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int OrdId { get; set; }
        public string Address { get; set; }
    }




    public class PickPointVM
    {
        public PickPointVM()
        {

        }

        public PickPointVM(PickPoint row)
        {
            Id = row.Id;
            PointAddress = row.PointAddress;
            DriverEmail = row.DriverEmail;
            PickUpPhone = row.PickUpPhone;
            PickVehId = row.PickVehId;
            NumberPlate = row.NumberPlate;
        }
        public int Id { get; set; }
        public string PointAddress { get; set; }
        public string DriverEmail { get; set; }
        public string PickUpPhone { get; set; }
        public int PickVehId { get; set; }
        public string NumberPlate { get; set; }

        public IEnumerable<SelectListItem> Drivers { get; set; }
        public IEnumerable<SelectListItem> Vihicles { get; set; }


    }


    public class RefundVM
    {
        public RefundVM()
        {

        }

        public RefundVM(Refund row)
        {
            Id = row.Id;
            Reason = row.Reason;
            Destination = row.Destination;
            PickupAddress = row.PickupAddress;
            OrderNum = row.OrderNum;
            CustomerEmail = row.CustomerEmail;
            Status = row.Status;
            Date = row.Date;
        }
        [Key]
        public int Id { get; set; }
        public string Reason { get; set; }
        public string Destination { get; set; }
        public string PickupAddress { get; set; }
        public int OrderNum { get; set; }
        public string CustomerEmail { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }



        public IEnumerable<SelectListItem> Orders { get; set; }
    }

    public class UserVM
    {

        public UserVM()
        {

        }


        public UserVM(User row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            EmailAddress = row.EmailAddress;
            Password = row.Password;
            PhoneNumber = row.PhoneNumber;

        }

        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

    }








    public class ResetVM
    {

        public ResetVM()
        {

        }


        public ResetVM(User row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            EmailAddress = row.EmailAddress;
            Password = row.Password;
        }

        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public string PhoneNumber { get; set; }

    }


    public class CategoryVM
    {

        public CategoryVM()
        {

        }

        public CategoryVM(Category row)
        {
            Id = row.Id;
            Name = row.Name;
            NameFix = row.NameFix;




        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string NameFix { get; set; }



    }




    public class BookingVM
    {

        public BookingVM()
        {

        }


        public BookingVM(Booking row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            Constractor = row.Constractor;

        }

        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
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
        public int Total { get; set; }



        public IEnumerable<SelectListItem> Constractors { get; set; }

    }




    public class PickVM
    {

        public PickVM()
        {

        }


        public PickVM(Delivery row)
        {
            Id = row.Id;
            OrderId = row.OrderId;
            VehicleId = row.VehicleId;
            NumberPlate = row.NumberPlate;
            DriverId = row.DriverId;
            DriverName = row.DriverName;
            Destination = row.Destination;
            PickUpTime = row.PickUpTime;
            PickUpAddress = row.PickUpAddress;
            Date = row.Date;
            DriverConfirm = row.DriverConfirm;

        }


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


        public IEnumerable<SelectListItem> PickPoints  { get; set; }

    }







    public class ProductVM
    {

        public ProductVM()
        {

        }


        public ProductVM(Product row)
        {
            Id = row.Id;
            Name = row.Name;
            Query = row.Query;
            Description = row.Description;
            Price = row.Price;
            CategoryName = row.CategoryName;
            CategoryId = row.CategoryId;
            Quantity = row.Quantity;
            ImageName = row.ImageName;
            Color = row.color;
            Liters = row.Liters;



        }

        public int Id { get; set; }
        [Required]

        public string Name { get; set; }
        public string Query { get; set; }
        [AllowHtml]
        [Required]
        public string Description { get; set; }
        [Required]
        public int Price { get; set; }
        [Display(Name = "Hire Price")]
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }

        public string ImageName { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; }
        public int Liters { get; set; }



        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<string> GalleryImages { get; set; }

    }
    public class UserNavPartialVM
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }



    public class UserProfileVM
    {

        public UserProfileVM()
        {

        }
        public UserProfileVM(User row)
        {
            Id = row.Id;
            FirstName = row.FirstName;
            LastName = row.LastName;
            EmailAddress = row.EmailAddress;
            Username = row.EmailAddress;
            Password = row.Password;

        }

        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
        [Required]
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class CartVM
    {

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }

        public string Color { get; set; }
        public int Price { get; set; }
        public int Total { get { return (Quantity * Price); } }
        public string Image { get; set; }
        public int ProductQuant { get; set; }


    }


    public class OrderVM
    {

        public OrderVM()
        {

        }


        public OrderVM(Order row)
        {
            OrderId = row.OrderId;
            UserId = row.UserId;
            CreatedAt = row.CreatedAt;
            Destination = row.Destination;
            Status = row.Status;
            TotalPrice = row.TotalPrice;
            DeliveryStatus = row.DeliveryStatus;
            DeliveryFee = row.DeliveryFee;


        }



        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Destination { get; set; }
        public string Status { get; set; }
        public int TotalPrice { get; set; }
        public string DeliveryStatus { get; set; }
        public int DeliveryFee { get; set; }



    }

    public class OrderDetailsVM
    {

        public OrderDetailsVM()
        {

        }


        public OrderDetailsVM(OrderDetails row)
        {
            Id = row.Id;
            OrderId = row.OrderId;
            ProductId = row.ProductId;
            Quantity = row.Quantity;
            Color = row.Color;

        }



        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; }

    }




    public class OrdersForUserVM
    {

        public int OrderNumber { get; set; }
        public int Total { get; set; }
        public Dictionary<string, int> ProductsAndQty { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public int ProductId { get; set; }
        public string Destination { get; set; }
        public string DeliveryStatus { get; set; }

        public int DeliveryFee { get; set; }



    }

}