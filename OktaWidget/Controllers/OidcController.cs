using log4net;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

using System.Text;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using OktaJS_SDK.Models;
using OktaJS_SDK.Services;
using System.Web.Routing;
//using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace OktaJS_SDK.Controllers
{
    public class OidcController : Controller
    {
        ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // ILog logger = LogManager.GetLogger("SpecialLogFile");

        NameValueCollection appSettings = ConfigurationManager.AppSettings;

        // Org settings for primary Org
        private static string apiUrl = ConfigurationManager.AppSettings["okta.apiUrl"];
        private static string apiToken = ConfigurationManager.AppSettings["okta.apiToken"];
        private OktaOidcHelper oktaOidcHelper = new OktaOidcHelper(apiUrl, apiToken);

        [HttpGet]
        public ActionResult InitiateService()
        {
            //Default endoint for Custom Authorization Server
            logger.Debug("Get OIDC Endpoint_Service");
            return RedirectToAction("UnprotectedLanding", "AltLanding");
        }

        [HttpGet]
        public ActionResult ValidationEndpoint(string code, string state)
        {
            //use this for auth code workflow
            logger.Debug("Get OIDC for auth code workflow");


            logger.Debug(" code = " + code + " state " + state);

            string error = null;
            string error_description = null;
            string token_type = null;
            string scope = null;
            string id_token_status = null;
            string idToken = null;
            string access_token_status = null;
            string accessToken = null;
            string refresh_token_status = null;
            string refreshToken = null;
            System.Security.Claims.ClaimsPrincipal jsonPayload = null;
            IRestResponse<TokenRequestResponse> response = null;
            OidcIdToken oidcIdToken = new OidcIdToken();
            OidcAccessToken oidcAccessToken = new OidcAccessToken();
            string basicAuth = appSettings["oidc.spintweb.clientId"] + ":" + appSettings["oidc.spintweb.clientSecret"];

            var bytesBasicAuth = System.Text.Encoding.UTF8.GetBytes(basicAuth);
            string encodedBasicAuth = System.Convert.ToBase64String(bytesBasicAuth);


            try
            {
                //var client = new RestClient(MvcApplication.apiUrl + "/oauth2/v1/token");
                var client = new RestClient(appSettings["oidc.authServer"] + "/v1/token");
                var request = new RestRequest(Method.POST);
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                request.AddHeader("Authorization", " Basic " + encodedBasicAuth);
                request.AddQueryParameter("grant_type", "authorization_code");
                request.AddQueryParameter("code", code);
                request.AddQueryParameter("redirect_uri", appSettings["oidc.spintweb.RedirectUri"]);
                response = client.Execute<TokenRequestResponse>(request);
                if (response.Data != null)
                {
                    error = response.Data.error;
                    error_description = response.Data.error_description;
                    token_type = response.Data.token_type;
                    scope = response.Data.scope;
                }

                if (response.Data.id_token != null)
                {
                    id_token_status = "id_token present";
                    idToken = response.Data.id_token;

                    string issuer = appSettings["oidc.issuer"];
                    string audience = appSettings["oidc.spintweb.clientId"];
                    //jsonPayload = oktaOidcHelper.DecodeAndValidateIdToken(idToken, clientId, issuer, audience);
                    jsonPayload = oktaOidcHelper.ValidateIdToken(idToken, issuer, audience);
                    if (jsonPayload.Identity.IsAuthenticated)
                    {
                        TempData["errMessage"] = jsonPayload.ToString();
                        //System.IdentityModel.Tokens.Jwt.JwtSecurityToken tokenReceived = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(idToken);
                        //oidcIdToken = Newtonsoft.Json.JsonConvert.DeserializeObject<OidcIdToken>(idToken);
                    }
                    else
                    {
                        TempData["errMessage"] = "Invalid ID Token!";

                    }
                    TempData["idToken"] = idToken;
                }
                else
                {
                    id_token_status = "id_token NOT present";
                }

                if (response.Data.access_token != null)
                {
                    accessToken = response.Data.access_token;
                    access_token_status = "access_token present";
                    TempData["accessToken"] = response.Data.access_token;
                }
                else
                {
                    access_token_status = "access_token NOT present";
                }

                if (response.Data.refresh_token != null)
                {
                    refreshToken = response.Data.refresh_token;
                    refresh_token_status = "refresh_token present";
                    TempData["refreshToken"] = response.Data.refresh_token;

                }
                else
                {
                    refresh_token_status = "refresh_token NOT present";
                }
            }
            catch (Exception ex)
            {

                logger.Error(ex.ToString());
            }

            if (error != null)
            {

                TempData["errMessage"] = "Error " + error_description;
                TempData["oktaOrg"] = apiUrl;
                return RedirectToAction("UnprotectedLanding", "AltLanding");
            }
            else
            {

                TempData["errMessage"] = "SUCCESS token_type = " + token_type + " scope = " + scope + " : " + id_token_status + " : " + access_token_status + " oktaId = " + oidcIdToken.sub;
                TempData["oktaOrg"] = apiUrl;
                return RedirectToAction("AuthCodeLanding", "AltLanding");

            }

        }

        [HttpPost]
        public ActionResult ValidationEndpoint()
        {
            //use this for implicit workflow
            logger.Debug("Post OIDC for implicit workflow");


            string myState = Request["state"];
            string idToken = Request["id_token"];
            string accessToken = Request["access_token"];
            string tokenType = Request["token_type"];
            string expires = Request["expires_in"];
            string scope = Request["scope"];

            System.Security.Claims.ClaimsPrincipal jsonPayload = null;
            string accessTokenStatus = null;
            string idTokenStatus = null;

            OidcIdToken oidcIdToken = new OidcIdToken();
            OidcIdTokenMin oidcIdTokeMin = new OidcIdTokenMin();

            if (idToken != null)
            {
                idTokenStatus = " ID Token Present";
                TempData["idToken"] = idToken;
                //string clientId = appSettings["oidc.spintnative.clientId"];
                string issuer = appSettings["oidc.issuer"];
                string audience = appSettings["oidc.spintweb.clientId"];
                //jsonPayload = oktaOidcHelper.DecodeAndValidateIdToken(idToken, clientId, issuer, audience);
                jsonPayload = oktaOidcHelper.ValidateIdToken(idToken, issuer, audience);
                if (jsonPayload.Identity.IsAuthenticated)
                {
                    TempData["errMessage"] = jsonPayload.ToString();
                    System.IdentityModel.Tokens.Jwt.JwtSecurityToken tokenReceived = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(idToken);
                    //oidcIdToken = Newtonsoft.Json.JsonConvert.DeserializeObject<OidcIdToken>(tokenReceived.ToString());
                }
                else
                {
                    TempData["errMessage"] = "Invalid ID Token!";

                }
                TempData["idToken"] = idToken;
            }
            else
            {
                idTokenStatus = " ID Token Not Found";
            }

            if (accessToken != null)
            {
                accessTokenStatus = "access_token Present";
                TempData["accessToken"] = accessToken;

            }
            else
            {
                accessTokenStatus = "access_token NOT Found";
            }


            if (accessToken != null || idToken != null)
            {
                TempData["errMessage"] = "SUCCESS token_type = " + tokenType + " expires = " + expires + " scope = " + scope + " : " + idTokenStatus + " : " + accessTokenStatus + " oktaId = " + oidcIdToken.sub;
                TempData["oktaOrg"] = apiUrl;

                return View("../AltLanding/ImplicitLanding", oidcIdTokeMin);
            }
            else
            {
                TempData["errMessage"] = "Error token_type = " + tokenType + " expires = " + expires + " scope = " + scope + " : " + idTokenStatus + " : " + accessTokenStatus + " oktaId = " + oidcIdToken.sub;
                TempData["oktaOrg"] = apiUrl;
                return View("../AltLanding/UnprotectedLanding");
            }


        }




    }
}