using BuildingBlocks.CommonAuthorization.CommonAuthorizationAttributes;
using BuildingBlocks.Core.Response;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.Bookings.Commands.CreateBooking;
using Order.Application.Features.Bookings.Commands.UpdateBooking;
using Order.Application.Features.Bookings.Models;
using Order.Application.Features.Bookings.Queries.GetBooking;
using Order.Application.Features.Notifications.Commands.CreateNotification;
using Order.Application.Features.Notifications.Commands.DeleteNotification;
using Order.Application.Features.Notifications.Commands.UpdateNotification;
using Order.Application.Features.Notifications.Models;
using Order.Application.Features.Notifications.Queries.GetNotification;
using Order.Application.Features.Notifications.Queries.GetNotificationCount;
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
    //[AccessGroup("booking.create")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<NotificationDto>>> CreateAsync([FromForm] CreateNotificationCommand command)
    {
        return await Mediator.Send(command);
    }
    [AccessGroup("notification.delete")]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteAsync(int id)
    {
        return await Mediator.Send(new DeleteNotificationCommand(id));
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<NotificationDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<NotificationDto>>>> GetNotificationsForMeWithPaginationAsync([FromQuery] GetNotificationsForMeWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpDelete("mobile/{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteForMobileAsync(int id)
    {
        return await Mediator.Send(new DeleteNotificationCommand(id));
    }

    [HttpPut("mobile-read-all")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> UpdateReadAllForMobileAsync()
    {
        
        return await Mediator.Send(new UpdateReadAllNotificationCommand());
    }

    [HttpPut("mobile-read/{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> UpdateReadForMobileAsync(int id)
    {
        return await Mediator.Send(new UpdateReadNotificationCommand(id));
    }

    [HttpGet("merchant/{id}")]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<NotificationDto>>> GetForMerchantByIdAsync(int id)
    {
        return await Mediator.Send(new GetNotificationByIdQuery { Id = id });
    }

    [HttpGet("mobile/{id}")]
    [ProducesResponseType(typeof(ApiResponse<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<NotificationDto>>> GetForMobileByIdAsync(int id)
    {
        return await Mediator.Send(new GetNotificationByIdQuery { Id = id });
    }

    [HttpGet("unread-me")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadForMeAsync()
    {
        return await Mediator.Send(new GetNotificationCountByMeQuery());
    }
}
