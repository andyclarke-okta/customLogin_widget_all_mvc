using log4net;

using Newtonsoft.Json.Linq;
using Okta.Core;
using Okta.Core.Clients;
using Okta.Core.Models;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.Web.Mvc;
using OktaJS_SDK.Models;
using OktaJS_SDK.Services;
using System.Reflection;

namespace OktaJS_SDK.Controllers
{
    public class RegistrationController : Controller
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        NameValueCollection appSettings = ConfigurationManager.AppSettings;


        // Org settings for primary Org
        private static string primaryOrgUrl = ConfigurationManager.AppSettings["okta.ApiUrl"];
        private static string primaryOrgApiToken = ConfigurationManager.AppSettings["okta.ApiToken"];
        private OktaAPIHelper oktaApiHelper = new OktaAPIHelper(primaryOrgUrl, primaryOrgApiToken);

        private OktaUserMgmt oktaUserMgmt = new OktaUserMgmt(primaryOrgUrl, primaryOrgApiToken);
        //private OktaAuthMgmt oktaAuthMgmt = new OktaAuthMgmt(primaryOrgUrl, primaryOrgApiToken);



        //private IUserAccountService userAccountService;
        //private IEmailService emailService;

        //public RegistrationController()
        //{
            //userAccountService = new UserAccountService();
            //emailService = SmtpEmailServiceFactory.CreateInstance();
        //}


        public ActionResult InitiateRegistration(string email = null)
        {
        logger.Debug("InitiateRegistration");

            //set parameters
            string relayState = Request["relayState"];
            if (string.IsNullOrEmpty(relayState) && Request.QueryString["RelayState"] != null)
            {
                relayState = Request.QueryString["RelayState"];
            }
            else if (string.IsNullOrEmpty(relayState) && TempData["relayState"] != null)
            {
                relayState = (string)TempData["relayState"];
            }
            TempData["relayState"] = relayState;
            string stateToken = Request["stateToken"];
            if (string.IsNullOrEmpty(stateToken) && TempData["stateToken"] != null)
            {
                stateToken = TempData["stateToken"].ToString();
            }
            TempData["stateToken"] = stateToken;
            string oktaId = Request["oktaId"];
            if (string.IsNullOrEmpty(oktaId) && TempData["oktaId"] != null)
            {
                oktaId = TempData["oktaId"].ToString();
            }
            TempData["oktaId"] = oktaId;
            string userName = Request["userName"];
            if (string.IsNullOrEmpty(userName) && TempData["userName"] != null)
            {
                userName = TempData["userName"].ToString();
            }
            TempData["userName"] = userName;


            //TempData["helpLink"] = MvcApplication.helpLink;

            var registration = new RegistrationViewModel();
            registration.MailingAddress = new MailingAddressViewModel();
            registration.Phone = new PhoneViewModel();

            if (!string.IsNullOrWhiteSpace(email))
            {
                CustomUser customUser = oktaUserMgmt.GetCustomUser(email);
                if (customUser != null)
                {
                    registration.FirstName = customUser.Profile.FirstName;
                    registration.LastName = customUser.Profile.LastName;
                    registration.MailingAddress.Street1 = customUser.Profile.streetAddress;
                    registration.MailingAddress.City = customUser.Profile.city;
                    registration.MailingAddress.State = customUser.Profile.state;
                    registration.MailingAddress.ZIP = customUser.Profile.zipCode;
                    registration.Email = customUser.Profile.Email;
                    registration.Phone.Phone = customUser.Profile.primaryPhone;
                    registration.customId = customUser.Profile.customId;
                }
            }


            return View("StartRegistration", registration);
        }

        // POST: Registration/Register
        public ActionResult Register(RegistrationViewModel registration)
        {
            logger.Debug("Register user " + registration.Email);
            //set parameters
            string relayState = Request["relayState"];
            if (string.IsNullOrEmpty(relayState) && Request.QueryString["RelayState"] != null)
            {
                relayState = Request.QueryString["RelayState"];
            }
            else if (string.IsNullOrEmpty(relayState) && TempData["relayState"] != null)
            {
                relayState = (string)TempData["relayState"];
            }
            TempData["relayState"] = relayState;
            //string stateToken = Request["stateToken"];
            //if (string.IsNullOrEmpty(stateToken) && TempData["stateToken"] != null)
            //{
            //    stateToken = TempData["stateToken"].ToString();
            //}
            //TempData["stateToken"] = stateToken;
            //string oktaId = Request["oktaId"];
            //if (string.IsNullOrEmpty(oktaId) && TempData["oktaId"] != null)
            //{
            //    oktaId = TempData["oktaId"].ToString();
            //}
            //TempData["oktaId"] = oktaId;
            //string userName = Request["userName"];
            //if (string.IsNullOrEmpty(userName) && TempData["userName"] != null)
            //{
            //    userName = TempData["userName"].ToString();
            //}
            //TempData["userName"] = userName;

            CustomUser newCustomUser = null;
            CustomUser addedCustomUser = null;
            //User addedOktaUser = new Okta.Core.Models.User();
            //CustomUser addedCustomUser = new CustomUser(addedOktaUser);

            if (!ModelState.IsValid)
            {
                logger.Debug("registration data was not received correctly");
                TempData["errMessage"] = "We found a few errors with your registration data. Please check the fields below for specific messages.";
                return View("Index", registration);
            }


            //check for existing account
            newCustomUser = VerifyIdentity(registration);
            if (newCustomUser != null && newCustomUser.Id != null)
            {
                // If the user exists, redirect back to registration where they can choose to reset password

                return RedirectToAction("Index","Home", new { email = registration.Email });
            }
            else
            {
                //new user not found 
                User newOktaUser = new Okta.Core.Models.User();
                newCustomUser = new CustomUser(newOktaUser);
                //tranform user profile from registration to customuser
                newCustomUser.Profile.Login = registration.Email;
                newCustomUser.Profile.Email = registration.Email;
                newCustomUser.Profile.FirstName = registration.FirstName;
                newCustomUser.Profile.LastName = registration.LastName;
                newCustomUser.Profile.customId = registration.customId;
            }

            ////CustomUser customUserProfileExt = new CustomUser();
            //CustomUser customUserProfileExt = null;
            //customUserProfileExt = oktaUserMgmt.GetCustomUser(registration.Email);
            //if (customUserProfileExt != null && customUserProfileExt.Id != null)
            //{
            //    // If the user exists, redirect to ForgotUsername/UserFound
            //    // TODO: Update to VerifyIdentity when available (not the same flow as ForgotUsername
            //    //return RedirectToAction("UserFound", "ForgotUsername", new { email = registration.Email });
            //    return RedirectToAction("UserFound", new { email = registration.Email });
            //}


            //new user creation
            //create user with profile
            //will create user with multiple api calls
            //first user is created with status=STAGED with no password
            //branded email sent to user with custom activation link
            //hitting custom activation link will activate account and prompt user for password
            //lastly set password
            Random random = new Random();
            string firstName = null;
            //generate passCode as one time activation token
            string activation_passCode = random.Next(99999, 1000000).ToString();
            string activation_setDate = DateTime.Now.ToString();
            string createFailedErrorMessage = "We could not register your account at this time. Please try again or contact the service center via the information" +
                " at the bottom of the page if this has happened multiple times.";

            newCustomUser.Profile.activation_passCode = activation_passCode;
            newCustomUser.Profile.activation_setDate = activation_setDate;

            //addedCustomUser = oktaUserMgmt.AddCustomUser(newCustomUser, activation_passCode, activation_setDate);
            addedCustomUser = oktaUserMgmt.AddCustomUser(newCustomUser);
            if (addedCustomUser != null)
            {
                //if (addedCustomUser.Status == "ACTIVE")
                //{
                //    //error, newly created user should be STAGED and must have okta Id
                //    logger.Error("user creation success for email " + registration.Email + " status " + addedCustomUser.Status);
                //    TempData["errMessage"] = "Created User success for " + registration.Email;
                //    TempData["txtUserName"] = addedCustomUser.Profile.Login;
                //    TempData["txtPassword"] = newCustomUser.Credentials.Password.Value;
                //    return RedirectToAction("SilentLogin", "Home");
                //}

                if (addedCustomUser.Status != "STAGED" || string.IsNullOrEmpty(addedCustomUser.Id))
                {
                    //error, newly created user should be STAGED and must have okta Id
                    logger.Error("user creation failed for email " + registration.Email + " status " + addedCustomUser.Status);
                    TempData["errMessage"] = createFailedErrorMessage;
                    return View("Index", registration);
                }
            }
            else
            {
                logger.Error("user creation failed for email " + registration.Email);
                TempData["errMessage"] = createFailedErrorMessage;
                return View("Index", registration);
            }

            //send branded email with activation link
            TempData["recoveryToken"] = addedCustomUser.Profile.activation_passCode;
            TempData["oktaId"] = addedCustomUser.Id;
            if (string.IsNullOrEmpty(addedCustomUser.Profile.FirstName))
            {
                var index = registration.Email.IndexOf(".");
                if (index > 0)
                {
                    firstName = registration.Email.Substring(0, index);
                }
                else
                {
                    var index2 = registration.Email.IndexOf("@");
                    if (index2 > 0)
                    {
                        firstName = registration.Email.Substring(0, index2);
                    }
                    else
                    {
                        firstName = registration.Email;
                    }
                }
            }
            else
            {
                firstName = addedCustomUser.Profile.FirstName;
            }
            firstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(firstName);
            TempData["firstName"] = firstName;

            TempData["linkExpiry"] = appSettings["custom.ActivationExpire_hours"];
            SendEmail(addedCustomUser, TempData["relayState"] == null ? null : TempData["relayState"].ToString());
            logger.Debug("Branded Email Sent " + addedCustomUser.Profile.Email + " with passCode " + activation_passCode);

            //with branded, display sent message
            return RedirectToAction("Success");
        }

        // GET Registration/Activate
        [HttpGet]
        public ActionResult Activate(string token, string locator)
        {

            //TempData["helpLink"] = MvcApplication.helpLink;

            return VerifyActivate(token, locator);
        }

        // POST Registration/VerifyActivate
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult VerifyActivate(string recoveryToken, string oktaId)
        {
            logger.Debug("VerifyActivate ");

            if (string.IsNullOrEmpty(recoveryToken) && TempData["recoveryToken"] != null)
            {
                recoveryToken = TempData["recoveryToken"].ToString();
            }

            //set parameters
            string relayState = Request["relayState"];
            if (string.IsNullOrEmpty(relayState) && Request.QueryString["RelayState"] != null)
            {
                relayState = Request.QueryString["RelayState"];
            }
            else if (string.IsNullOrEmpty(relayState) && TempData["relayState"] != null)
            {
                relayState = (string)TempData["relayState"];
            }
            TempData["relayState"] = relayState;
            string stateToken = Request["stateToken"];
            if (string.IsNullOrEmpty(stateToken) && TempData["stateToken"] != null)
            {
                stateToken = TempData["stateToken"].ToString();
            }
            TempData["stateToken"] = stateToken;

            string userName = Request["userName"];
            if (string.IsNullOrEmpty(userName) && TempData["userName"] != null)
            {
                userName = TempData["userName"].ToString();
            }
            TempData["userName"] = userName;




            //TempData["helpLink"] = MvcApplication.helpLink;
            string tokenFailedErrorMessage = "We could not activate your account at this time. Please try clicking the link in your email again " +
                "or contact the service center via the information at the bottom of the page.";

            //get UserClient based on Org credentials
            OktaClient oktaClient = new OktaClient(MvcApplication.apiToken, MvcApplication.apiUrl);
            UsersClient usersClient = oktaClient.GetUsersClient();

            //Validate Token from branded email response link
            //get user profile data
            //compare with received token
            if (!string.IsNullOrEmpty(recoveryToken) && !string.IsNullOrEmpty(oktaId))
            {
                //validate token
                //get custom profile for user
                User oktaBaseUser = new Okta.Core.Models.User();
                CustomUser customUser = new CustomUser(oktaBaseUser);
                customUser = oktaUserMgmt.GetCustomUser(oktaId);
                if (customUser.Status != "STAGED")
                {
                    //if it doesnt work out return to beginning
                    logger.Error("Token validation is not appropriate for current user state " + customUser.Status);
                    TempData["errMessage"] = tokenFailedErrorMessage;
                    return RedirectToAction("Index", "Home");
                }
                if (string.IsNullOrEmpty(customUser.Profile.activation_passCode) || string.IsNullOrEmpty(customUser.Profile.activation_setDate))
                {
                    logger.Error("Token validation information is not available");
                    TempData["errMessage"] = tokenFailedErrorMessage;
                    return RedirectToAction("Index", "Home");
                }


                //this is a one time token, so if profile received reset the data.
                string activation_passCode = customUser.Profile.activation_passCode;
                string activation_setDate = customUser.Profile.activation_setDate;
                customUser.Profile.activation_passCode = "";
                customUser.Profile.activation_setDate = "";

                bool rspSetCustomUserProfile = oktaUserMgmt.UpdateCustomUserAttributesOnly(customUser);
                if (!rspSetCustomUserProfile)
                {
                    logger.Error("Unable to Set User Custom Profile to reset activation passCode: " + customUser.Profile.Email);
                    TempData["errMessage"] = tokenFailedErrorMessage;
                    return RedirectToAction("Index", "Home");
                }

                //compare to passed in token
                //Get current time
                DateTime currentTime = DateTime.Now;
                DateTime setTime = DateTime.Parse(activation_setDate);
                //Get the difference in Minutes between the set time and Current Time.
                TimeSpan timeSpan = currentTime.Subtract(setTime);
                long myInterval = Convert.ToInt32(timeSpan.TotalHours);
                long authExpiry = Convert.ToInt32(appSettings["custom.ActivationExpire_hours"]);
                //check received passcode matches what was saved in storage app attribute

                if (activation_passCode == recoveryToken)
                {
                    //check for expired activation code
                    if (authExpiry > myInterval)
                    {
                        //continue to set password
                        User oktaUser = new User();
                        oktaUser.Id = oktaId;

                        try
                        {
                            //transistion user accout from STAGED to PROVISIONED
                            var rspUri = usersClient.Activate(oktaUser, sendEmail: false);
                            //rsp is email activation link for embedded workflow
                            //this cannot be branded so we are setting password directly

                        }
                        catch (OktaException ex)
                        {
                            if (ex.ErrorCode == "E0000001")
                            {
                                logger.Error("Api Valiadation Failed: " + userName);
                            }
                            else
                            {
                                logger.Error(userName + " = " + ex.ErrorCode + ":" + ex.ErrorSummary);
                                // generic failure
                            }
                            TempData["errMessage"] = tokenFailedErrorMessage;
                            return RedirectToAction("Index", "Home");
                        }//end catch

                        logger.Debug("Token Validated Successfully proceed to create password");
                        return RedirectToAction("GetPassword");
                    }
                    else
                    {
                        logger.Debug("Token is correct, but has exceeded time limit " + appSettings["custom.ActivationExpire_hours"] + " hours");
                        TempData["errMessage"] = tokenFailedErrorMessage;
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    logger.Error("Cannot Validate Token: " + recoveryToken + " locator: " + oktaId);
                    TempData["errMessage"] = tokenFailedErrorMessage;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                //if it doesnt work out return to beginning
                logger.Error("Cannot Validate Token: " + recoveryToken + " locator: " + oktaId);
                TempData["errMessage"] = tokenFailedErrorMessage;
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Registration/Success
        public ActionResult Success()
        {
            logger.Debug("Registration Success");

            return View();
        }

        // POST: Registration/ValidateHomeMailingAddress
        //[HttpPost]
        //public ActionResult ValidateHomeMailingAddress(MailingAddressViewModel mailingAddress)
        //{
        //    return Json(CheckHomeMailingAddress(mailingAddress));
        //}

        //// POST: Registration/ValidateWorkMailingAddress
        //[HttpPost]
        //public ActionResult ValidateWorkMailingAddress(MailingAddressViewModel mailingAddress)
        //{
        //    return Json(CheckWorkMailingAddress(mailingAddress));
        //}

        //private MailingAddressEnvelope CheckHomeMailingAddress(MailingAddressViewModel mailingAddress)
        //{
        //    var targetMailingAddress = mailingAddress;
        //    var isKnownAddress = false;
        //    if (mailingAddress.Street1.ToLower().Contains("123 main st") &&
        //        string.IsNullOrEmpty(mailingAddress.Street2) &&
        //        string.IsNullOrEmpty(mailingAddress.Street3) &&
        //        mailingAddress.City.ToLower().Contains("small town") &&
        //        mailingAddress.State.ToUpper().Equals("NC") &&
        //        mailingAddress.ZIP.Contains("55555") &&
        //        string.IsNullOrEmpty(mailingAddress.Province) &&
        //        mailingAddress.Country.ToLower().Equals("united states"))
        //    {
        //        targetMailingAddress = new MailingAddressViewModel()
        //        {
        //            Type = "Home",
        //            Street1 = "123 Main St",
        //            City = "Small Town",
        //            State = "NC",
        //            ZIP = "55555-5555",
        //            Country = "United States"
        //        };
        //        isKnownAddress = true;
        //    }
        //    return new MailingAddressEnvelope()
        //    {
        //        MailingAddress = targetMailingAddress,
        //        IsKnownAddress = isKnownAddress
        //    };
        //}


        //private MailingAddressEnvelope CheckWorkMailingAddress(MailingAddressViewModel mailingAddress)
        //{
        //    var targetMailingAddress = mailingAddress;
        //    var isKnownAddress = false;
        //    if (mailingAddress.Street1.ToLower().Contains("220 leigh farm") &&
        //        string.IsNullOrEmpty(mailingAddress.Street2) &&
        //        string.IsNullOrEmpty(mailingAddress.Street3) &&
        //        mailingAddress.City.ToLower().Contains("durham") &&
        //        mailingAddress.State.ToUpper().Equals("NC") &&
        //        mailingAddress.ZIP.Contains("27707") &&
        //        string.IsNullOrEmpty(mailingAddress.Province) &&
        //        mailingAddress.Country.ToLower().Equals("united states"))
        //    {
        //        targetMailingAddress = new MailingAddressViewModel()
        //        {
        //            Street1 = "220 Leigh Farm Rd",
        //            City = "Durham",
        //            State = "NC",
        //            ZIP = "27707-8110"
        //        };
        //        isKnownAddress = true;
        //    }
        //    return new MailingAddressEnvelope()
        //    {
        //        MailingAddress = targetMailingAddress,
        //        IsKnownAddress = isKnownAddress
        //    };
        //}


        private CustomUser VerifyIdentity(RegistrationViewModel registration)
        {
            logger.Debug("VerifyIdentity");

            //primary search check registration email verses okta login
            CustomUser customUser = null;
            customUser = oktaUserMgmt.GetCustomUser(registration.Email);
            if (customUser != null && customUser.Id != null)
            {
                //if user exists
                //return user info
                return customUser;
            }
            else
            {

                //note: more sophistcated logic cann be inserted to check for duplicates
                //ie lastname, primaryPhone, mailing address, etc
                //use extended search criteria to search for possible duplicate user
                System.Text.StringBuilder buildFilter = null;
                // build buildFilter
                //pagelimit cannot exceed 200
                buildFilter = new System.Text.StringBuilder();
                //buildFilter.Append("created lt \"" + formatTime + "\"");
                //buildFilter.Append("status eq \"ACTIVE\"");
                buildFilter.Append("profile.customId eq \"" + registration.customId + "\"");

                //buildFilter.Append("&limit=20");

                PagedResults<CustomUser> pagedCustomUser;
                //CustomUserProfile customUserProfile = new CustomUserProfile();
                Uri myNextPage = null;
                bool isThisLastPage = true;
                do
                {
                    if (isThisLastPage)
                    {
                        pagedCustomUser = oktaUserMgmt.ListCustomUsersExtended(searchType: "search", criteria: buildFilter.ToString());
                    }
                    else
                    {
                        pagedCustomUser = oktaUserMgmt.ListCustomUsersExtended(searchType: "search", criteria: buildFilter.ToString(), nextPage: myNextPage);
                    }


                    isThisLastPage = pagedCustomUser.IsLastPage;
                    myNextPage = pagedCustomUser.NextPage;
                    int rspCount = pagedCustomUser.Results.Count;
                    foreach (var customUser1 in pagedCustomUser.Results)
                    {
                        logger.Debug("oktaId= " + customUser1.Id + " customId= " + customUser1.Profile.customId + " lastName= " + customUser1.Profile.LastName);
                    }
                    if (rspCount == 1)
                    {
                        logger.Debug("found single user ");
                        customUser = pagedCustomUser.Results[0];
                        return customUser;
                    }
                    else if (rspCount > 1)
                    {
                        logger.Error("found " + rspCount.ToString() + " users matching criteria");
                        //picking first member of list, since any return more than empty processes the same
                        customUser = pagedCustomUser.Results[0];
                        return customUser;
                    }
                    else
                    {
                        customUser = null;
                        return customUser;
                    }
                } while (!isThisLastPage);
            }//end if else
        }

        [HttpGet]
        public ActionResult GetPassword()
        {
            logger.Debug("GetPassword GET ");
            //set parameters
            string relayState = Request["relayState"];
            if (string.IsNullOrEmpty(relayState) && Request.QueryString["RelayState"] != null)
            {
                relayState = Request.QueryString["RelayState"];
            }
            else if (string.IsNullOrEmpty(relayState) && TempData["relayState"] != null)
            {
                relayState = (string)TempData["relayState"];
            }
            TempData["relayState"] = relayState;
            string stateToken = Request["stateToken"];
            if (string.IsNullOrEmpty(stateToken) && TempData["stateToken"] != null)
            {
                stateToken = TempData["stateToken"].ToString();
            }
            TempData["stateToken"] = stateToken;

            string userName = Request["userName"];
            if (string.IsNullOrEmpty(userName) && TempData["userName"] != null)
            {
                userName = TempData["userName"].ToString();
            }
            TempData["userName"] = userName;



            TempData["complexity1"] = appSettings["custom.PasswordComplexity_01"];
            TempData["complexity2"] = appSettings["custom.PasswordComplexity_02"];
            TempData["complexity3"] = appSettings["custom.PasswordComplexity_03"];
            TempData["complexity4"] = appSettings["custom.PasswordComplexity_04"];
            //TempData["helpLink"] = MvcApplication.helpLink;
            return View("GetPassword");
        }



        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult SetPassword(SetUserPassword setPassword)
        {
            logger.Debug("SetPassword ");
            //set parameters
            string relayState = Request["relayState"];
            if (string.IsNullOrEmpty(relayState) && Request.QueryString["RelayState"] != null)
            {
                relayState = Request.QueryString["RelayState"];
            }
            else if (string.IsNullOrEmpty(relayState) && TempData["relayState"] != null)
            {
                relayState = (string)TempData["relayState"];
            }
            TempData["relayState"] = relayState;
            string stateToken = Request["stateToken"];
            if (string.IsNullOrEmpty(stateToken) && TempData["stateToken"] != null)
            {
                stateToken = TempData["stateToken"].ToString();
            }
            TempData["stateToken"] = stateToken;
            string oktaId = Request["oktaId"];
            if (string.IsNullOrEmpty(oktaId) && TempData["oktaId"] != null)
            {
                oktaId = TempData["oktaId"].ToString();
            }
            TempData["oktaId"] = oktaId;
            string userName = Request["userName"];
            if (string.IsNullOrEmpty(userName) && TempData["userName"] != null)
            {
                userName = TempData["userName"].ToString();
            }
            TempData["userName"] = userName;

            string myStatus = null;
            string myStateToken;
            string mySessionToken;


            string myResourcePath;
            System.Uri myUri;


            //get AuthClient based on Org credentials
            OktaClient oktaClient = new OktaClient(MvcApplication.apiToken, MvcApplication.apiUrl);
            AuthClient authClient = oktaClient.GetAuthClient();

            try
            {
                myResourcePath = MvcApplication.apiUrl + Constants.EndpointV1 + Constants.AuthnEndpoint + Constants.CredentialsResetPasswordEndpoint;
                logger.Debug("url " + myResourcePath);
                myUri = new System.Uri(myResourcePath);

                ApiObject myApiObject = new ApiObject();
                myApiObject.SetProperty("newPassword", setPassword.newPassword);
                AuthResponse resetPasswordRsp = authClient.Execute(stateToken, myUri, myApiObject);

                logger.Debug("resetPasswordRsp status " + resetPasswordRsp.Status);
                myStatus = resetPasswordRsp.Status;
                myStateToken = resetPasswordRsp.StateToken;
                mySessionToken = resetPasswordRsp.SessionToken;


                userName = resetPasswordRsp.Embedded.User.Profile.Login;
            }
            catch (OktaException ex)
            {
                if (ex.ErrorCode == "E0000085")
                {
                    logger.Error("Access Denied by Polciy for User: " + userName);
                    TempData["errMessage"] = "Access Denied by Polciy for User: " + userName;
                }
                else if (ex.ErrorCode == "E0000080")
                {
                    logger.Error("Password Does not meet Complexity Requirements " + userName);
                    TempData["errMessage"] = "Password Does not meet Complexity Requirements: " + userName;
                    return RedirectToAction("GetPassword", "ForgotPassword");
                }
                else
                {
                    logger.Error(userName + " = " + ex.ErrorCode + ":" + ex.ErrorSummary);
                    // generic failure
                    TempData["errMessage"] = "Failed to Set Password as provided!";
                }
                return RedirectToAction("Index", "Home");
            }//end catch

            //checking for limited number of possible statuses
            switch (myStatus)
            {
                case "MFA_ENROLL":
                    TempData["stateToken"] = myStateToken;
                    return RedirectToAction("InitiateMFAEnroll", "MfaEnroll");
                case "MFA_REQUIRED":
                    TempData["stateToken"] = myStateToken;
                    return RedirectToAction("InitiateMFAChallenge", "Mfa");
                case "SUCCESS":
                    //return RedirectToAction("ProcessSuccess", "EvaluateUser", new { passuser = userName, passId = oktaId, passSession = mySessionToken, passRelay = relayState });
                //break;
                default:
                    logger.Debug("Status: " + myStatus);
                    TempData["errMessage"] = "Status: " + myStatus;
                    break;
            }

            return RedirectToAction("Index", "Home");
        }

        private void SendEmail(CustomUser customUser, string relayState = null)
        {
            logger.Debug("enter send email from Registration");

            EmailMgmt emailService =  new EmailMgmt(
                                        appSettings["SMTPHost"],
                                        int.Parse(appSettings["SMTPPort"]),
                                        appSettings["EmailSSLEnable"].Equals("true"),
                                        appSettings["FromEmailAddress"],
                                        appSettings["FromEmailPassword"]
                                        );



            try
            {
                //var fromEmailAddress = CloudConfigurationManager.GetSetting("ReplyToEmailAddress");
                //var fromEmailDisplayName = CloudConfigurationManager.GetSetting("FromEmailDisplayName");
                var fromEmailAddress = appSettings["ReplyToEmailAddress"];
                var fromEmailDisplayName = appSettings["FromEmailDisplayName"];
                var toEmailAddress = customUser.Profile.Email;
                var activateUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Registration/Activate?token=" + TempData["recoveryToken"] + "&locator=" + TempData["oktaId"];

                if (!string.IsNullOrEmpty(relayState))
                {
                    activateUrl += "&relayState=" + relayState;
                }

                ViewData["activateUrl"] = activateUrl;
                string body = oktaApiHelper.RenderPartialToString(this.ControllerContext, "_AccountActivationEmail", ViewData, TempData);

                logger.Debug("SendEmail " + toEmailAddress + " message " + body);

                emailService.SendEmail(fromEmailAddress, fromEmailDisplayName, toEmailAddress, "Account Activation Request", body, true);
            }
            catch (Exception ex)
            {
                logger.Error("Mail send failed to " + customUser.Profile.Email + " error; " + ex.Message);
                TempData["errMessage"] = "We could not send an email to " + customUser.Profile.Email +
                    ". If this error persists, please contact the help desk via the contact information at the bottom of the page.";
            }
        }




    }
}