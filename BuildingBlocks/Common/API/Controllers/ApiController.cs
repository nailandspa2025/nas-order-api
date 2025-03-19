using BuildingBlocks.Common.API.ExceptionHandlers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BuildingBlocks.Common.API.Controllers;

[ApiController]
[ApiExceptionFilter]
[Route("api/v{version:apiVersion}/[controller]")]
public class ApiController : ControllerBase
{
    protected string GetLoggedInUserName()
    {
        if (User.Identity == null)
            throw new ArgumentNullException();

        return User.Identity.Name ?? "system";
    }

    //protected string GetLoggedInUserId()
    //{
    //    if (User.Identity == null || User.Identity.Name == null)
    //        throw new ArgumentNullException();

    //    return User.FindFirstValue(ClaimTypes.NameIdentifier);
    //}
}