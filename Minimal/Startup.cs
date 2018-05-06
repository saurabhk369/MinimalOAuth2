using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Minimal
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        private readonly ConcurrentDictionary<string, string> _authenticationCodes =
            new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

        public void Configuration(IAppBuilder app)
        {


            OAuthOptions = new OAuthAuthorizationServerOptions()
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new OAuthAuthorizationServerProvider()
                {
                    OnValidateClientAuthentication = async (context) =>
                    {
                        string clientId, clientSecret;
                        if (context.TryGetBasicCredentials(out clientId, out clientSecret) || context.TryGetFormCredentials(out clientId, out clientSecret))
                            context.Validated();
                    },
                    OnGrantResourceOwnerCredentials = async (context) =>
                    {
                        if (context.UserName == "User1" && context.Password == "Password1")
                        {
                            ClaimsIdentity oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
                            oAuthIdentity.AddClaim(new Claim("sub", context.UserName));
                            oAuthIdentity.AddClaim(new Claim("role", "user"));
                            context.Validated(oAuthIdentity);
                        }
                    },
                    OnGrantClientCredentials = async (context) =>
                    {
                        if (context.Scope != null)
                        {
                            ClaimsIdentity oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
                            oAuthIdentity.AddClaim(new Claim("sub", context.ClientId));
                            oAuthIdentity.AddClaim(new Claim("role", "user"));
                            context.Validated(oAuthIdentity);
                        }
                    },
                    //OnValidateClientRedirectUri = async (context) =>
                    //{
                    //    if (context.RedirectUri == "http://localhost:51300")
                    //        context.Validated();
                    //}
                },
                AllowInsecureHttp = true,
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(5),
                //AuthorizationCodeProvider = new AuthenticationTokenProvider
                //{
                //    OnCreate = (context) =>
                //    {
                //        context.SetToken(Guid.NewGuid().ToString("n") + Guid.NewGuid().ToString("n"));
                //        _authenticationCodes[context.Token] = context.SerializeTicket();
                //    },
                    
                //    OnReceive = (context) =>
                //    {
                //        string value;
                //        if (_authenticationCodes.TryRemove(context.Token, out value))
                //        {
                //            context.DeserializeTicket(value);
                //        }
                //    }
                //}
                
            };

            app.UseOAuthBearerTokens(OAuthOptions);
            app.UseOAuthAuthorizationServer(OAuthOptions);
        }
    }
}