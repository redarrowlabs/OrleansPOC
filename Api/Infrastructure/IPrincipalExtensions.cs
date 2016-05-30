using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Api.Infrastructure
{
    public static class IPrincipalExtensions
    {
        public static string GetFullName(this IPrincipal principal)
        {
            var fullName = String.Empty;
            var cp = principal as ClaimsPrincipal;
            if (cp != null)
            {
                var given = cp.Claims.First(x => x.Type == ClaimTypes.GivenName).Value;
                var surname = cp.Claims.First(x => x.Type == ClaimTypes.Surname).Value;
                fullName = $"{given} {surname}";
            }

            return fullName;
        }
    }
}