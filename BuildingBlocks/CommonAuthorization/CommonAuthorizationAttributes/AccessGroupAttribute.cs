using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.CommonAuthorization.CommonAuthorizationAttributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AccessGroupAttribute : TypeFilterAttribute
{
    public AccessGroupAttribute(string accessGroup) : base(typeof(AccessGroupFilter))
    {
        Arguments = new object[] { accessGroup };
    }

    private class AccessGroupFilter : IAsyncAuthorizationFilter
    {
        private readonly string _accessGroups;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AccessGroupFilter(string accessGroups, IServiceScopeFactory serviceScopeFactory)
        {
            _accessGroups = accessGroups;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var userName = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userName))
            {
                context.Result = new JsonResult(new { Succeeded = false, Message = "No access"})
                {
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var authorizationService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
            var hasAccess = await authorizationService.HasPermissionAsync(_accessGroups);
            if (!hasAccess)
            {
                context.Result = new JsonResult(new { Succeeded = false, Message = "No access" });
            }
        }
    }
}