using BuildingBlocks.CommonAuthorization.CommonAuthorizationAttributes;
using BuildingBlocks.Core.Response;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.BookingCancelReasons.Commands.CreateBookingCancelReason;
using Order.Application.Features.BookingCancelReasons.Commands.DeleteBookingCancelReason;
using Order.Application.Features.BookingCancelReasons.Commands.UpdateBookingCancelReason;
using Order.Application.Features.BookingCancelReasons.Models;
using Order.Application.Features.BookingCancelReasons.Queries.GetBookingCancelReason;
using Order.Application.Features.BookingCancelReasons.Queries.GetBookingCancelReasons;
using Order.Application.Features.BookingCancelReasons.Queries.GetBookingCancelReasonsWithPagination;

namespace Order.Api.Controllers.V1;

[ApiVersion("1.0")]
public class BookingCancelReasonsController: ApiControllerBase
{
    [AccessGroup("config-reason.view")]
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<BookingCancelReasonDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<BookingCancelReasonDto>>>> GetBookingCancelReasonsWithPaginationAsync([FromQuery] GetBookingCancelReasonsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [AccessGroup("config-reason.view")]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BookingCancelReasonDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BookingCancelReasonDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetBookingCancelReasonByIdQuery { Id = id });
    }

    [AccessGroup("config-reason.create")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BookingCancelReasonDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BookingCancelReasonDto>>> CreateAsync([FromForm] CreateBookingCancelReasonCommand command)
    {
        return await Mediator.Send(command);
    }

    [AccessGroup("config-reason.update")]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BookingCancelReasonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<BookingCancelReasonDto>>> UpdateAsync(int id, [FromForm] UpdateBookingCancelReasonCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        return await Mediator.Send(command);
    }

    [AccessGroup("config-reason.delete")]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteAsync(int id)
    {
        return await Mediator.Send(new DeleteBookingCancelReasonCommand(id));
    }

    [HttpGet("mobile-all")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BookingCancelReasonDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<BookingCancelReasonDto>>>> GetAllAsync()
    {
        return await Mediator.Send(new GetBookingCancelReasonAllQuery());
    }
}

