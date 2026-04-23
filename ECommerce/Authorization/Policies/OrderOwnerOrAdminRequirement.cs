using Microsoft.AspNetCore.Authorization;

namespace ECommerce.Authorization.Policies
{
    public class OrderOwnerOrAdminRequirement : IAuthorizationRequirement
    { 
    }
}
