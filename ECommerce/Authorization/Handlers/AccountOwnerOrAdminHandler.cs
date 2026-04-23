using ECommerce.Authorization.Policies;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.Authorization.Handlers
{
    public class AccountOwnerOrAdminHandler : AuthorizationHandler<AccountOwnerOrAdminRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountOwnerOrAdminHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AccountOwnerOrAdminRequirement requirement)
        {
            var httpContext = context.Resource as HttpContext;

            var isAdmin = context.User.FindFirst("isAdmin")?.Value == "true";
            if (isAdmin)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var routeId = httpContext?.GetRouteValue("id")?.ToString();
            if (userIdClaim != routeId)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
