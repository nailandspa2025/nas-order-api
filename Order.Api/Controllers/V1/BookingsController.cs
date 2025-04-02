using Order.Application.Features.Bookings.Commands.CreateBooking;
using Order.Application.Features.Bookings.Commands.DeleteBooking;
using Order.Application.Features.Bookings.Commands.UpdateBooking;
using Order.Application.Features.Bookings.Models;
using Order.Application.Features.Bookings.Queries.GetBooking;
using Order.Application.Features.Bookings.Queries.GetBookingsWithPagination;
using BuildingBlocks.Core.Response;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.Bookings.Commands.CreateBookingMobile;
using Order.Application.Features.Bookings.Queries.GetBookings;
using Order.Application.Features.Bookings.Commands.CancelBooking;

namespace Order.Api.Controllers.V1;

[ApiVersion("1.0")]
public class BookingsController : ApiControllerBase
{
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<BookingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<BookingDto>>>> GetBookingsWithPaginationAsync([FromQuery] GetBookingsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetBookingByIdQuery { Id = id });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> CreateAsync([FromForm] CreateBookingCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> UpdateAsync(int id, [FromForm] UpdateBookingCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteAsync(int id)
    {
        return await Mediator.Send(new DeleteBookingCommand(id));
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> CreateMobleAsync([FromForm] CreateBookingMobileCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<BookingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<BookingDto>>>> GetBookingsMeWithPaginationAsync([FromQuery] GetBookingsMeQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpPut("cancel-mobile/{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> CancelBookingMobileAsync(int id,[FromForm] CancelBookingCommand command)
    {
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            return await Mediator.Send(command);
        }
    }
}
