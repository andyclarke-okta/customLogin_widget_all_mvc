using OktaJS_SDK.Models;
using OktaJS_SDK.Services;
using log4net;
using Okta.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OktaJS_SDK.Controllers
{
    public class ForgotUsernameController : Controller
    {
        //private IUserAccountService userAccountService;
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        NameValueCollection appSettings = ConfigurationManager.AppSettings;

        // Org settings for primary Org
        private static string primaryOrgUrl = ConfigurationManager.AppSettings["okta.ApiUrl"];
        private static string primaryOrgApiToken = ConfigurationManager.AppSettings["okta.ApiToken"];
        private  OktaAPIHelper oktaApiHelper = new OktaAPIHelper(primaryOrgUrl, primaryOrgApiToken);

        private OktaUserMgmt oktaUserMgmt = new OktaUserMgmt(primaryOrgUrl, primaryOrgApiToken);



        //public ForgotUsernameController()
        //{
        //    userAccountService = new UserAccountService();
        //}

        // GET: ForgotUsername
        public ActionResult InitiateFindUser()
        {
            logger.Debug("ForgotUsername_Index");

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


            ForgotUsernameViewModel forgotUsername = new ForgotUsernameViewModel();
            ViewBag.IsNotFound = false;
            return View("InitiateFindUser", forgotUsername);
        }

        // POST: ForgotUsername/CheckForUser
        [HttpPost]
        public ActionResult CheckForUser(ForgotUsernameViewModel forgotUsernameViewModel)
        {
            logger.Debug("CheckForUser");
            CustomUser customUser = null;

            ViewBag.IsNotFound = false;
            if (!ModelState.IsValid)
            {
                return View("Index", forgotUsernameViewModel);
            }

            //need to check against custom user profile since aicpaId is part of criteria
            //ajc debug
            //this implementation contains support for multiple return users
            //for now, returned records greater than 1 is considered an error

            System.Text.StringBuilder buildFilter = null;
            // build buildFilter
            //pagelimit cannot exceed 200
            buildFilter = new System.Text.StringBuilder();
            buildFilter.Append("status eq \"ACTIVE\" and ");
            buildFilter.Append("profile.lastName eq \"" + forgotUsernameViewModel.LastName + "\" and ");
            buildFilter.Append("profile.policyNumber eq \"" + forgotUsernameViewModel.policyNumber + "\"");
            //buildFilter.Append("&limit=8");

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
                    logger.Debug("successful; found single user ");
                    //send email with userName/email
                    customUser = pagedCustomUser.Results[0];
                    TempData["firstName"] = customUser.Profile.FirstName;
                    TempData["userName"] = customUser.Profile.Login;
                    SendEmail(customUser);
                    var routeValues = new RouteValueDictionary();
                    routeValues.Add("login", pagedCustomUser.Results[0].Profile.Login);
                    routeValues.Add("email", pagedCustomUser.Results[0].Profile.Email);
                    return RedirectToAction("UserFound", routeValues);
                }
                else
                {
                    logger.Error("Error found " + rspCount.ToString() + " users matching criteria");
                    return RedirectToAction("UserNOTFound");
                }

            } while (!isThisLastPage);


        }

        public ActionResult UserFound(string login,string email)
        {
            ViewBag.email = email;
            ViewBag.MaskedEmail = MaskEmail(login);
            return View();
        }

        public ActionResult UserNotFound()
        {

            return View();
        }


        private string MaskEmail(string email)
        {
            var indexOfAt = email.LastIndexOf('@');

            return email.Substring(0, 1) + "****" + email.Substring(indexOfAt - 1, 1) + "@" + email.Substring(indexOfAt + 1);
        }

        private void SendEmail(CustomUser customUser, string relayState = null)
        {
            logger.Debug("enter send email from ForgotPassword");

            EmailMgmt emailService = new EmailMgmt(
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
                string body = oktaApiHelper.RenderPartialToString(this.ControllerContext, "_ForgotUsernameEmail", ViewData, TempData);

                logger.Debug("SendEmail " + toEmailAddress + " message " + body);

                emailService.SendEmail(fromEmailAddress, fromEmailDisplayName, toEmailAddress, "Account Activation Request", body, true);
            }
            catch (Exception ex)
            {
                logger.Error("Mail send failed to " + customUser.Profile.Email + " error; " + ex.Message);
                TempData["errMessage"] = "We could not send an email to " + customUser.Profile.Email +
                    ". If this error persists, please contact the help desk via the contact information at the bottom of the page.";
            }

            try
            {
                var fromEmailAddress = appSettings["FromEmailAddress"].ToString();
                var fromEmailDisplayName = appSettings["FromEmailDisplayName"].ToString();
                var fromEmailPassword = appSettings["FromEmailPassword"].ToString();
                var smtpHost = appSettings["SMTPHost"].ToString();
                var smtpPort = appSettings["SMTPPort"].ToString();
                var emailSSLEnable = appSettings["EmailSSLEnable"].ToString();
                var toEmailAddress = customUser.Profile.Email;
                var toEmailDisplayName = customUser.Profile.LastName + ", " + customUser.Profile.FirstName;


                string body = oktaApiHelper.RenderPartialToString(this.ControllerContext, "_ForgotUsernameEmail", ViewData, TempData);
                MailMessage message = new MailMessage();

                message.From = new MailAddress(fromEmailAddress, fromEmailDisplayName);
                message.To.Add(toEmailAddress);  //new MailAddress(toEmailAddress, toEmailDisplayName));
                message.Subject = "Forgotten Password Reset Request";
                message.IsBodyHtml = true;
                message.Body = body;

                var client = new SmtpClient();
                client.Credentials = new NetworkCredential(fromEmailAddress, fromEmailPassword);
                client.Host = smtpHost;
                if (emailSSLEnable == "true")
                {
                    client.EnableSsl = true;
                }
                else
                {
                    client.EnableSsl = false;
                }

                client.Port = !string.IsNullOrEmpty(smtpPort) ? Convert.ToInt32(smtpPort) : 0;
                logger.Debug("SendEmail " + toEmailAddress + " message " + message.Body.ToString());
                client.Send(message);


            }
            catch (Exception ex)
            {
                logger.Error("Mail send failed to " + customUser.Profile.Email + " error; " + ex.Message);
                TempData["errMessage"] = "Mail send failed to " + customUser.Profile.Email + " error; " + ex.Message;
                // throw (new Exception("Mail send failed to " + oktaUserObj.Profile.Email + ", though password change was done." + ex.Message));
            }
        }


    }
}