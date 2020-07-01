using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Okta.Core.Models;


namespace OktaJS_SDK.Models
{
    public class ExpiredPasswordUser
    {
        [Display(Name = "Old Password")]
        [Required]
        [DataType(DataType.Password)]
       // [StringLength(20, MinimumLength = 8)]
        public string oldPassword { get; set; }

        [Display(Name = "New Password")]
      //  [StringLength(20, MinimumLength = 8)]
        [Required]
        [DataType(DataType.Password)]
        public string newPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [Compare("newPassword")]
        [DataType(DataType.Password)]
        public string confPassword { get; set; }
        public string userName { get; set; }
        public int minLength { get; set; }
        public int minLowerCase { get; set; }
        public int minUpperCase { get; set; }
        public int minNumber { get; set; }
        public int minSymbol { get; set; }
    }


    public class SetUserPassword
    {
        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        // [StringLength(20, MinimumLength = 8)]
        public string oldPassword { get; set; }
        [Display(Name = "New Password")]
        //  [StringLength(20, MinimumLength = 8)]
        [Required]
        [DataType(DataType.Password)]
        public string newPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [Compare("newPassword")]
        [DataType(DataType.Password)]
        public string confPassword { get; set; }
        public string userName { get; set; }
        public int minLength { get; set; }
        public int minLowerCase { get; set; }
        public int minUpperCase { get; set; }
        public int minNumber { get; set; }
        public int minSymbol { get; set; }
    }

    public class ForgotPasswordUser
    {
        [Display(Name = "Username")]
        public string userName { get; set; }
    }




    public class MfaChallengeOptions
    {
        // public Dictionary<string,System.Uri> mfaLinks { get; set; }   
        public string mfaRequired { get; set; }
        public int mfaCount { get; set; }
        public List<MfaChallengeFactors> challengeFactors { get; set; }
    }

    public class MfaChallengeFactors
    {
        public string factorType { get; set; }
        public string provider { get; set; }
        public string status { get; set; }
        public string mfaIcon { get; set; }
        public Dictionary<string, System.Uri> mfaLinks { get; set; }
    }

    public class MfaEnrollOptions
    {
        public int mfaCount { get; set; }
        public int mfaRequiredCount { get; set; }
        public List<MfaEnrollFactors> enrollFactors { get; set; }
    }

    public class MfaEnrollFactors
    {
        public string factorType { get; set; }
        public string provider { get; set; }
        public string status { get; set; }
        public string mfaIcon { get; set; }
        public string enrollLink { get; set; }
        public string enrollment { get; set; }
        public Dictionary<string, System.Uri> mfaLinks { get; set; }
    }


    public class MfaActive
    {
        public int mfaCount { get; set; }
        public List<MfaActiveFactors> activeFactors { get; set; }
    }

    public class MfaActiveFactors
    {
        public string id { get; set; }
        public string factorType { get; set; }
        public string provider { get; set; }
        public string vendorName { get; set; }
        public string status { get; set; }

        public DateTime created { get; set; }

        public DateTime lastUpdated { get; set; }

    }

    public class MfaActivateOptions
    {
        public Dictionary<string, System.Uri> mfaLinks { get; set; }
        public int linkCount { get; set; }
    }

    public class SecQuestionList
    {
        public List<Question> listSecQuestion { get; set; }
    }

    //public class HelpDeskQuestionList
    //{
    //    public List<HelpDeskQuestion> listHelpDeskQuestion { get; set; }
    //}

    //public class HelpDeskQuestion
    //{
    //    public string helpDeskQuestion { get; set; }
    //}


    public class SmsCountryCodeList
    {
        public List<SmsCountryCode> codeList { get; set; }
    }


    public class SmsCountryCode
    {
        public string countryCode { get; set; }
        public string prefix { get; set; }
        public string countryName { get; set; }
    }


    public class UserCreds
    {
        public string userName { get; set; }
        public string passWord { get; set; }
        public string oktaOrg { get; set; }
        public string relayState { get; set; }
    }


    public class IdentityProviderList
    {
        public List<IdentityProviderConfig> identityProviderConfig { get; set; }
    }

    public class IdentityProviderConfig
    {
        public string idpName { get; set; }
        public string idpDomain { get; set; }
        public string idpUrl { get; set; }
        public string idpACS { get; set; }
        public string idpApp { get; set; }
    }


}