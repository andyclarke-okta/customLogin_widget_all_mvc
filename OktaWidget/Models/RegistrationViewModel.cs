using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OktaJS_SDK.Models
{
    public class RegistrationViewModel
    {
        [DisplayName("First Name")]
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[\w-\s]+$", ErrorMessage = "The First Name should be just letters, hyphens, and spaces.")]
        public string FirstName { get; set; }

        //[DisplayName("Middle Name")]
        //[MaxLength(100)]
        //[RegularExpression(@"^[\w-\s]+$", ErrorMessage = "The Middle Name should be just letters, hyphens, and spaces.")]
        //public string MiddleName { get; set; }

        [DisplayName("Last Name")]
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[\w-\s]+$", ErrorMessage = "The Last Name should be just letters, hyphens, and spaces.")]
        public string LastName { get; set; }

        //[DisplayName("Prefix")]
        //public string NamePrefix { get; set; }

        public MailingAddressViewModel MailingAddress { get; set; }


        [DisplayName("Email Address")]
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^.+@.+\.[a-zA-Z\.]{2,64}$", ErrorMessage = "The Email Address should resemble user@domain.com.")]
        public string Email { get; set; }

        public PhoneViewModel Phone { get; set; }

        //[DisplayName("Contact By")]
        //[MaxLength(10)]
        //[RegularExpression("^(Email|Mail|Fax|Phone)$", ErrorMessage = "The Contact By method should be one of \"Email\", \"Mail\", \"Fax\", or \"Phone\".")]
        //public string PreferredContact { get; set; }


        [DisplayName("CustomId")]
        [DefaultValue(false)]
        public string customId { get; set; }


        //[DisplayName("Password")]
        //[Required]
        //[DataType(DataType.Password)]
        //[DefaultValue(false)]
        //public string password { get; set; }

        public static IEnumerable<SelectListItem> NamePrefixItemList = new List<SelectListItem>()
        {
            new SelectListItem() { Text = string.Empty, Value = string.Empty },
            new SelectListItem() { Text = "Dr.", Value = "Dr." },
            new SelectListItem() { Text = "Gov.", Value = "Gov." },
            new SelectListItem() { Text = "Hon.", Value = "Hon." },
            new SelectListItem() { Text = "Miss", Value = "Miss" },
            new SelectListItem() { Text = "Mr.", Value = "Mr." },
            new SelectListItem() { Text = "Mrs.", Value = "Mrs." },
            new SelectListItem() { Text = "Ms.", Value = "Ms." },
            new SelectListItem() { Text = "Rep.", Value = "Rep." },
            new SelectListItem() { Text = "Sec.", Value = "Sec." },
            new SelectListItem() { Text = "Sen.", Value = "Sen." },
            new SelectListItem() { Text = "Sir", Value = "Sir" }
        };

        public static IEnumerable<SelectListItem> PreferredContactItemList = new List<SelectListItem>()
        {
            new SelectListItem() { Text = string.Empty, Value = string.Empty },
            new SelectListItem() { Text = "Email", Value = "Email" },
            new SelectListItem() { Text = "Fax", Value = "Fax" },
            new SelectListItem() { Text = "Mail", Value = "Mail" },
            new SelectListItem() { Text = "Phone", Value = "Phone" }
        };

        public static IEnumerable<SelectListItem> CountryItemList = new List<SelectListItem>()
        {
            new SelectListItem() {Text = "United States", Value = "UNITED STATES" },
            new SelectListItem() {Text = "United States Minor Outlying Islands", Value = "United States Minor Outlying Islands" },
            new SelectListItem() {Text = "United States", Value = "UNITED STATES VIRGIN ISLANDS" }
        };
    }
}