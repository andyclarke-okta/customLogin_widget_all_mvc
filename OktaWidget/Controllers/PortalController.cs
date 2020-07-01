using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using log4net;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using OktaJS_SDK.Services;
using System.Reflection;
using RestSharp;
using System.Configuration;
using Okta.Core.Models;
using System.Collections.Generic;

namespace OktaJS_SDK.Controllers
{
    public class PortalController : Controller
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        NameValueCollection appSettings = ConfigurationManager.AppSettings;

        // Org settings for primary Org
        private static string apiUrl = ConfigurationManager.AppSettings["okta.apiUrl"];
        private static string apiToken = ConfigurationManager.AppSettings["okta.apiToken"];
        private OktaUserMgmt oktaUserMgmt = new OktaUserMgmt(apiUrl, apiToken);
        private OktaAppMgmt oktaAppMgmt = new OktaAppMgmt(apiUrl, apiToken);


        // GET: AltLanding
        public ActionResult Landing(string oktaId)
        {
 
            Okta.Core.Models.User oktaUser = new Okta.Core.Models.User();
            oktaUser.Id = oktaId;
            List<AppLink> myApps = oktaAppMgmt.GetUserAppLinks(oktaUser);

            return View(myApps);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult LandingRoute()
        {
            logger.Debug("LandingRoute");
            string appChoice = Request["appChoice"];

            //string appLink = Request["appLink"];

            string chooseApp_but = Request["chooseApp_but"];

            string myStatus;
            string myStateToken;
            string mySessionToken;

            //return Redirect(appLink);
            return Redirect(appChoice);
        }
    }
}