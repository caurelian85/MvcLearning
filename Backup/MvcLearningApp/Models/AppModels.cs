using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MvcLearningApp.Models
{
    public class ClientView
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        public DateTime Date { get; set; }

        public int Id_User { get; set; }

        public byte[] Avatar { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string AvatarMimeType { get; set; }

        public string Phone { get; set; }

        public int Id_Address { get; set; }

        public bool CanLoad { get; set; }
    }

    public class ClientIndexView
    {
        public List<ClientView> List { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class UserView
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "UserName is Required")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage="Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        public bool Enable { get; set; }

        public bool CanDelete { get; set; }
    }

    public class Address
    {
        public int? Id { get; set; }

        [Display(Name = "Street")]
        public string Street { get; set; }

        [Display(Name = "City")]
        public string City { get; set;}

        [Display(Name = "Zip")]
        public string Zip { get; set; }
    }
}