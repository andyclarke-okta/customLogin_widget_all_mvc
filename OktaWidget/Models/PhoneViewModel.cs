using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OktaJS_SDK.Models
{
    public class PhoneViewModel
    {
        [DisplayName("Phone Country")]
        [MaxLength(100)]
        [DefaultValue("United States")]
        public string Country { get; set; }

        [DisplayName("Phone")]
        [MaxLength(15)]
        [RegularExpression(@"^\d{1,15}$", ErrorMessage = "The Phone Number should only include digits.")]
        public string Phone { get; set; }

        [DisplayName("Extension")]
        [MaxLength(5)]
        [RegularExpression(@"^\d{1,55}$", ErrorMessage = "The Phone Extension should only include digits.")]
        public string Extension { get; set; }
    }
}