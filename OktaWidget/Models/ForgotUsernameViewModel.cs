using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OktaJS_SDK.Models
{
    public class ForgotUsernameViewModel
    {
        [DisplayName("Policy Number")]
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^\d+$", ErrorMessage = "That Policy Number does not look quite right. It should be just numbers (ex. 123456789).")]
        public string policyNumber { get; set; }

        [DisplayName("Last Name")]
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[\w-\s]+$", ErrorMessage = "That Last Name does not look quite right. It should be just letters, hyphens, and spaces.")]
        public string LastName { get; set; }
    }
}