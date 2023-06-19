using FitAplikacjaAPI.Services.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FitAplikacjaAPI.Services.AuthorizationHandlers
{
    /// <summary>
    /// Route data must specify owner's ID or user must be an admin
    /// </summary>
    public class UserRouteAccessHandler : AuthorizationHandler<UserRouteRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRouteRequirement requirement)
        {
            if(context.Resource is DefaultHttpContext httpContext)
            {
                // user ID as param
                var contextUserId = httpContext.GetRouteData().Values["userId"]?.ToString();

                // user ID from claims
                var claimsUserId = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if(contextUserId == null || claimsUserId == null)
                {
                    context.Fail();
                }
                else if (contextUserId.Equals(claimsUserId) || context.User.IsInRole("Admin"))
                {
                    // the user is the owner or an admin
                    context.Succeed(requirement);
                }
                else
                {
                    // forbidden
                    context.Fail();
                }
            }

            return Task.CompletedTask;
        }
    }
}
