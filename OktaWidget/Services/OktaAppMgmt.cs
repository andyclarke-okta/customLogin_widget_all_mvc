using log4net;
using Okta.Core;
using Okta.Core.Clients;
using Okta.Core.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using Newtonsoft.Json.Linq;
using OktaJS_SDK.Models;
using System.Collections;
using System.Diagnostics;
using System.Web.Mvc;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Web.UI;
using RestSharp;

namespace OktaJS_SDK.Services
{
    public class OktaAppMgmt
    {

        private OktaSettings _orgSettings;
        private string _apiToken;
        private string _orgUrl;
       // private AuthClient _authclient;
        private UsersClient _usersClient;
        private OktaClient _oktaClient;
       // private SessionsClient _sessionsClient;
       // private UserFactorsClient _userFactorClient;


        private static ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public OktaAppMgmt(string orgUrlParam, string apiToken)
        {
            _orgUrl = orgUrlParam;
            Uri orgUri = new Uri(OrgUrl);
            _orgSettings = new OktaSettings();
            _orgSettings.ApiToken = apiToken;
            _orgSettings.BaseUri = orgUri;
            //_authclient = new AuthClient(_orgSettings);
            _oktaClient = new OktaClient(_orgSettings);
            _usersClient = new UsersClient(_orgSettings);
            //_sessionsClient = new SessionsClient(_orgSettings);

        }

        public string OrgUrl { get { return _orgUrl; } }

        public List<AppLink> GetUserAppLinks(User user)
        {
            List<AppLink> result = null;
            UserAppLinksClient userAppclient = new UserAppLinksClient(user, _orgSettings);
            if (userAppclient != null)
            {
                result = userAppclient.ToList();
            }

            return result;
        }


        public  string FindAppByName()
        {
            ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            logger.Debug("FindAppByName  ");

            string appName = appSettings["custom.StorageAppName"];
            //check App profile where help desk credentials are stored

            try
            {
                App myApp = new App();
                //myApp.Id = appId;
                OktaClient oktaClient = new OktaClient(_orgSettings);
                AppsClient appsClient = oktaClient.GetAppsClient();

                PagedResults<App> integratedApps = appsClient.GetList(query: appName);
                myApp.Id = integratedApps.LastId;

                return myApp.Id;

            }
            catch (OktaException ex)
            {
                logger.Error(" ErrorCode: " + ex.ErrorCode + " " + ex.ErrorSummary);
                return null;
            }//end catch
        }


    }
}