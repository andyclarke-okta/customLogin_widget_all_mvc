# okta-dotnet4mvc-clientapp-oktaWidget

>Okta Widget integration example using either authn or OpenID Connect (OIDC)


![Version](https://img.shields.io/badge/version-v0.3.0.beta-blue.svg)


## System Requirements ##
Thsi sample was built using Visual Studio 2017 with .Net Framework 4.7.1.

## Usage ##
If you do not have access to an Okta Tenant you can get a free Okta Developer Tenant [here](https://developer.okta.com/signup/)
The Okta tenent requires configuration to integrate your client application.
Please refer to [OpenId Connect](https://developer.okta.com/docs/api/resources/oidc)

The web.config file needs to be edited to point to your configuration and your okta tenent.
The default OIDC configuration is 'Okta Integration Examples' in DEV environment

For Example;
  <appSettings>

    <!--Okta Config-->
    <add key="okta.ApiUrl" value="https://dev-assurant.oktapreview.com"/>
    <add key="okta.ApiToken" value="00dSM4wW3kmaxoBz-KjrUBtQaDuKOyEeqfEGe7pzm-"/>
    <!-- oidc workflow-->
    <add key="oidc.spintweb.clientId" value="0oadt4xjwdPgIrMP00h7"/>
    <add key="oidc.spintweb.clientSecret" value="zyWA6duYVrjOoRHpCUCkGAXZdeUpl35dFFCSFHbW"/>
    <add key="oidc.spintweb.RedirectUri_Implicit" value="http://localhost:60544/Oidc/Endpoint_Implicit"/>
    <add key="oidc.spintweb.RedirectUri_AuthCode" value="http://localhost:60544/Oidc/Endpoint_AuthCode"/>
    <add key="oidc.issuer" value="https://dev-assurant.oktapreview.com"/>
    <add key="app.appCode" value="TST"/>
    <add key="app.endpointGL" value="https://id-dev.assurant.com/"/>
  </appSettings>

## Features ##

The application has multiple authentication workflows all use the [Okta SignIn Widget] (https://github.com/okta/okta-signin-widget). 
The parameters are passed in through the HomeController.
These workflows are;
* Authn
  * authentication only
  * creates Okta session cookie
* OIDC Implicit
  * authentication and authorization to OIDC integration
  * OIDC implicit workflow
  * validates idToken at client and on server-side
  * creates Okta session cookie
* OIDC Auth Code
  * authentication and authorization to OIDC integration
  * OIDC Auth Code workflow
  * exchanges code for idToken on server-side
  *	validates idToken on server-side 
  * creates Okta session cookie
* OIDC Implicit with Password Migration
  * same as Implicit workflow above
  * uses SF Global Login Service Layer to migrate LDAP password to Okta
    * requires further provisioning in SF Global Login application 

## Contributing

1. Fork it
2. Create your feature branch (`git checkout -b feature/fooBar`)
3. Commit your changes (`git commit -am 'Add some fooBar'`)
4. Push to the branch (`git push origin feature/fooBar`)
5. Create a new Pull Request

**Note**: Contributing is very similar to the Github contribution process as described in detail 
[here](https://guides.github.com/activities/forking/).

## Contacts

- [Wayne Carson](https://assurhub.assurant.com/yc6235) – [wayne.carson@assurant.com](mailto:wayne.carson@assurant.com)
- [Andy Clarke](https://assurhub.assurant.com/fz6302) - [andy.clarke@assurant.com](mailto:andy.clarke@assurant.com)
