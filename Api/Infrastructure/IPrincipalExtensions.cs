using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Api.Infrastructure
{
    public static class IPrincipalExtensions
    {
        public static Guid GetUserId(this IPrincipal principal)
        {
            var id = Guid.Empty;
            var cp = principal as ClaimsPrincipal;
            if (cp != null)
            {
                id = Guid.Parse(cp.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            }

            return id;
        }

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

        public static bool IsPatient(this IPrincipal principal)
        {
            return IsInRole(principal, "patient");
        }

        public static bool IsProvider(this IPrincipal principal)
        {
            return IsInRole(principal, "provider");
        }

        private static bool IsInRole(IPrincipal principal, string roleName)
        {
            var cp = principal as ClaimsPrincipal;
            if (cp != null)
            {
                var roles = cp.Claims.Where(x => x.Type == ClaimTypes.Role);
                return roles.Any(x => x.Value == roleName);
            }

            return false;
        }
    }
}