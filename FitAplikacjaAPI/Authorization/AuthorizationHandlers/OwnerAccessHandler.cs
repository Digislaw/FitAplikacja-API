using FitAplikacja.Core.Interfaces;
using FitAplikacjaAPI.Services.Requirements;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FitAplikacjaAPI.Services.AuthorizationHandlers
{
    /// <summary>
    /// The user is resource owner or an admin
    /// </summary>
    public class OwnerAccessHandler : AuthorizationHandler<OwnershipRequirement, IUserRelatedEntity>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OwnershipRequirement requirement, IUserRelatedEntity resource)
        {
            var userId = context.User.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (userId.Equals(resource.ApplicationUserId) || context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
