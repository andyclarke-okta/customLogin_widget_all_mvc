using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OktaJS_SDK.Models
{
    public class MailingAddressViewModel
    {
        //[DisplayName("Address Type")]
        //[Required]
        //[MaxLength(6)]
        //[RegularExpression(@"^(Home|Work|School)$", ErrorMessage = "Address Type should be one of \"Home\", \"Work\", or \"School\".")]
        //public string Type { get; set; }

        //[DisplayName("Country")]
        //[Required]
        //[MaxLength(100)]
        //[DefaultValue("United States")]
        //public string Country { get; set; }

        [DisplayName("Street Address")]
        [Required]
        [MaxLength(250)]
        public string Street1 { get; set; }

        //[DisplayName("Street Address Line 2")]
        //[MaxLength(250)]
        //public string Street2 { get; set; }

        //[DisplayName("Street Address Line 3")]
        //[MaxLength(250)]
        //public string Street3 { get; set; }

        [DisplayName("City")]
        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [DisplayName("State")]
        [MaxLength(100)]
        public string State { get; set; }

        //[DisplayName("Province")]
        //[MaxLength(100)]
        //public string Province { get; set; }

        [DisplayName("Postal Code")]
        [Required]
        [MaxLength(10)]
        public string ZIP { get; set; }

        //public static IEnumerable<SelectListItem> AddressTypeList = new List<SelectListItem>()
        //{
        //    new SelectListItem() {Text = "Home", Value = "Home" },
        //    new SelectListItem() {Text = "Work", Value = "Work" }
        //    //new SelectListItem() {Text = "School", Value = "School" }
        //};

        // TODO: Complete list of states
        public static IEnumerable<SelectListItem> StateList = new List<SelectListItem>()
        {
            new SelectListItem() {Text = string.Empty, Value = string.Empty },
            new SelectListItem() {Text = "California", Value = "CA" },
            new SelectListItem() {Text = "North Carolina", Value = "NC" },
            new SelectListItem() {Text = "South Carolina", Value = "SC" },
            new SelectListItem() {Text = "Ohio", Value = "OH" },
            new SelectListItem() {Text = "Washington DC", Value = "DC" },
            new SelectListItem() {Text = "Virginia", Value = "VA" }
        };
    }
}