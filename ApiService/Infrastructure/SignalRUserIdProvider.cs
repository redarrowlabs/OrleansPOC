using Microsoft.AspNet.SignalR;
using System.Linq;
using System.Security.Claims;

namespace ApiService.Infrastructure
{
    public class SignalRUserIdProvider : IUserIdProvider
    {
        public string GetUserId(IRequest request)
        {
            if (request.User != null)
            {
                var subClaim = ((ClaimsPrincipal)request.User).Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (subClaim != null)
                {
                    return subClaim.Value;
                }
            }

            return null;
        }
    }
}