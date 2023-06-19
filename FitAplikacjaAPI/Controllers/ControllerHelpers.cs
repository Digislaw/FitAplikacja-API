using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace FitAplikacjaAPI.Controllers
{
    public static class ControllerHelpers
    {
        public static string GetCurrentUserId(this ControllerBase controller)
        {
            return controller.User.Claims
                .FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}
