using Microsoft.Owin.Security.OAuth;
using System;
using System.Threading.Tasks;

namespace Api.Infrastructure
{
    public class QueryStringOAuthTokenProvider : OAuthBearerAuthenticationProvider
    {
        public override Task RequestToken(OAuthRequestTokenContext context)
        {
            var value = context.Request.Query.Get("authToken");
            if (!String.IsNullOrEmpty(value))
            {
                context.Token = value;
            }

            return Task.FromResult<object>(null);
        }
    }
}