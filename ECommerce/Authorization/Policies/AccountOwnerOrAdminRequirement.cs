using Microsoft.AspNetCore.Authorization;

namespace ECommerce.Authorization.Policies
{
    public class AccountOwnerOrAdminRequirement : IAuthorizationRequirement
    {
    }
}
