using BuildingBlocks.CommonAuthorization.CommonAuthorizationAttributes;
using BuildingBlocks.Core.Response;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.Notifications.Commands.DeleteNotification;
using Order.Application.Features.Notifications.Models;
using Order.Application.Features.Notifications.Queries.GetNotifications;
using Order.Application.Features.Notifications.Queries.GetNotificationsWithPagination;

namespace Order.Api.Controllers.V1;


[ApiVersion("1.0")]
public class NotificationsController : ApiControllerBase
{
    [AccessGroup("notification.view")]
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<NotificationDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<NotificationDto>>>> GetWithPaginationAsync([FromQuery] GetNotificationsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    //[HttpGet("user-notifications")]
    //[ProducesResponseType(typeof(ApiResponse<PaginatedList<NotificationDto>>), StatusCodes.Status200OK)]
    //public async Task<ActionResult<ApiResponse<PaginatedList<NotificationDto>>>> GetUserNotificationsPaginationAsync([FromQuery] GetUserNotificationsQuery query)
    //{
    //    return await Mediator.Send(query);
    //}

    [AccessGroup("notification.delete")]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteAsync(int id)
    {
        return await Mediator.Send(new DeleteNotificationCommand(id));
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<NotificationDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<NotificationDto>>>> GetNotificationsForMeWithPaginationAsync([FromQuery] GetNotificationsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

}
