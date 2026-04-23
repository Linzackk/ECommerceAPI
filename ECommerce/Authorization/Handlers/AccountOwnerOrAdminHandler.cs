using ECommerce.Authorization.Policies;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

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
            var httpContext = _httpContextAccessor.HttpContext;

            var isAdmin = context.User.FindFirst("isAdmin")?.Value == "true";
            if (isAdmin)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var userIdClaim = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            var routeId = httpContext?.GetRouteValue("id")?.ToString();

            if (userIdClaim != null && userIdClaim == routeId)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
