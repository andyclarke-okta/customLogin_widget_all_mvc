using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Configuration;
using Newtonsoft;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using log4net;
using System.Collections.Specialized;
using OktaJS_SDK.Models;

namespace OktaJS_SDK.Controllers
{
    public class HomeController : Controller
    {

        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        NameValueCollection appSettings = ConfigurationManager.AppSettings;
        // Org settings for primary Org
        private static string apiUrl = ConfigurationManager.AppSettings["okta.apiUrl"];
        private static string apiToken = ConfigurationManager.AppSettings["okta.apiToken"];

        public ActionResult Index()
        {
            //duplicated from below
            ViewBag.Message = "Okta Login Widget page.";

            TempData["version"] = appSettings["okta.widgetVersion"];
            TempData["issuer"] = appSettings["oidc.issuer"];
            TempData["clientId"] = appSettings["oidc.spintweb.clientId"];
            TempData["redirectUri"] = appSettings["oidc.spintweb.redirectUri"];
            TempData["oktaOrg"] = appSettings["okta.apiUrl"];
            TempData["usernamePrefix"] = appSettings["app.appCode"];

            //return View("authnLogin");
            //return View("authnIdpDiscoveryLogin");
            //return View("oidcIdpStaticLogin");
            return View("oidcImplicitLogin");
            //return View("oidcImplicitLogin_withConsent");
            //return View("oidcPswMigrationLogin");
            //return View("oidcAuthCodeLogin");
            ////return View("oidcHybridLogin");
            //return View("oidcImplicitLogin_noSession");
            //return View("sdkLogin");
            //return View("authnCustomExtensions");
            //return View("authnSSRegistrationLogin");
        }

        [HttpGet]
        public ActionResult Help()
        {
            return View("LoginHelp");
        }


        //public ActionResult authnLogin()
        //{
        //    ViewBag.Message = "Your Okta Standard Login Widget page.";

        //    TempData["oktaOrg"] = appSettings["okta.apiUrl"];
        //    TempData["clientId"] = appSettings["oidc.spintweb.clientId"];
        //    TempData["usernamePrefix"] = appSettings["app.appCode"];
        //    return View("authnLogin");
        //}


        //public ActionResult oidcImplicitLogin()
        //{
        //    ViewBag.Message = "Your Okta OpenID Connect Implicit Login Widget page.";

        //    TempData["clientId"] = appSettings["oidc.spintweb.clientId"];
        //    TempData["redirectUri"] = appSettings["oidc.spintweb.redirectUri"];
        //    TempData["oktaOrg"] = appSettings["okta.apiUrl"];
        //    TempData["usernamePrefix"] = appSettings["app.appCode"];
        //    return View("oidcImplicitLogin");
        //}

        //public ActionResult oidcPswMigrationLogin()
        //{
        //    ViewBag.Message = "Login Integrated with password migration";

        //    TempData["clientId"] = appSettings["oidc.spintweb.clientId"];
        //    TempData["redirectUri"] = appSettings["oidc.spintweb.redirectUri"];
        //    TempData["oktaOrg"] = appSettings["okta.apiUrl"];
        //    TempData["usernamePrefix"] = appSettings["app.appCode"];
        //    return View("oidcPswMigrationLogin");
        //}

        //public ActionResult oidcAuthCodeLogin()
        //{
        //    ViewBag.Message = "Your Okta OpenID Connect Auth Code Login Widget page.";

        //    TempData["clientId"] = appSettings["oidc.spintweb.clientId"];
        //    TempData["redirectUri"] = appSettings["oidc.spintweb.redirectUri"];
        //    TempData["oktaOrg"] = appSettings["okta.apiUrl"];
        //    TempData["usernamePrefix"] = appSettings["app.appCode"];
        //    return View("oidcAuthCodeLogin");
        //}

    }
}