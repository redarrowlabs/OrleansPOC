using AppSettingsByConvention;
using ChatClient.Infrastructure;
using IdentityModel.Client;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Linq;
using System.Security.Claims;

[assembly: OwinStartup(typeof(ChatClient.Startup))]

namespace ChatClient
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = SettingsByConvention.ForInterface<IConfiguration>();

            app.UseCookieAuthentication(
                new CookieAuthenticationOptions
                {
                    AuthenticationType = "Cookies"
                }
            );

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    Authority = config.Authority,
                    ClientId = "00000000-0000-0000-0000-000000000002",
                    RedirectUri = config.RedirectUri,
                    ResponseType = "id_token token",
                    Scope = "openid profile email roles api",
                    SignInAsAuthenticationType = "Cookies",
                    Notifications = new OpenIdConnectAuthenticationNotifications
                    {
                        SecurityTokenValidated = async n =>
                        {
                            var userInfoClient = new UserInfoClient(
                                new Uri(n.Options.Authority + "/connect/userinfo"),
                                n.ProtocolMessage.AccessToken
                            );

                            var userInfo = await userInfoClient.GetAsync();
                            userInfo.Claims
                                .ToList()
                                .ForEach(c => n.AuthenticationTicket.Identity.AddClaim(new Claim(c.Item1, c.Item2)));

                            var accessTokenClaim = new Claim("token", n.ProtocolMessage.AccessToken);
                            n.AuthenticationTicket.Identity.AddClaim(accessTokenClaim);
                        }
                    }
                }
            );
        }
    }
}