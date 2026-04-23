using ECommerce.Authorization.Policies;
using ECommerce.Repositories.Pedidos;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace ECommerce.Authorization.Handlers
{
    public class OrderOwnerOrAdminHandler : AuthorizationHandler<OrderOwnerOrAdminRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPedidosRepository _pedidoRepository;

        public OrderOwnerOrAdminHandler(IHttpContextAccessor httpContextAccessor, IPedidosRepository pedidoRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _pedidoRepository = pedidoRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OrderOwnerOrAdminRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var isAdmin = context.User.FindFirst("isAdmin")?.Value == "true";
            if (isAdmin)
            {
                context.Succeed(requirement);
                return;
            }

            var userIdClaim = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (userIdClaim == null)
            {
                context.Fail();
                return;
            }

            var routeValue = httpContext?.GetRouteValue("pedidoId")?.ToString();
            if (!Guid.TryParse(routeValue, out var pedidoId))
            {
                context.Fail();
                return;
            }

            var pedido = await _pedidoRepository.ObterPedidoPorId(pedidoId);
            if (pedido == null)
            {
                context.Fail();
                return;
            }

            if (userIdClaim != null && pedido.IdUsuario == Guid.Parse(userIdClaim))
                context.Succeed(requirement);
        }
    }
}
