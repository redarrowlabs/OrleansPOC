using GrainInterfaces;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Orleans;
using System;
using System.Linq;

namespace ApiService.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AuthorizeUserAttribute : AuthorizeAttribute
    {
        public override bool AuthorizeHubMethodInvocation(
            IHubIncomingInvokerContext hubIncomingInvokerContext,
            bool appliesToMethod)
        {
            if (!hubIncomingInvokerContext.Hub.Context.User.Identity.IsAuthenticated)
            {
                return false;
            }

            var groupParam = hubIncomingInvokerContext.MethodDescriptor.Parameters
                .Select((x, i) => new
                {
                    Name = x.Name,
                    Type = x.ParameterType,
                    Index = i
                })
                .SingleOrDefault(x => x.Name == "groupId" && x.Type == typeof(Guid));

            if (groupParam != null)
            {
                var groupId = (Guid)hubIncomingInvokerContext.Args[groupParam.Index];
                var user = hubIncomingInvokerContext.Hub.Context.User;
                var userId = user.GetUserId();
                if (user.IsPatient())
                {
                    if (user.GetUserId() != groupId)
                    {
                        return false;
                    }
                }
                else if (user.IsProvider())
                {
                    var provider = GrainClient.GrainFactory.GetGrain<IProviderGrain>(userId);
                    var validPatients = provider.CurrentPatients().Result;
                    if (!validPatients.Select(x => x.Id).Any(x => x == groupId))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}