using BuildingBlocks.CommonAuthorization.CommonAuthorizationAttributes;
using BuildingBlocks.Core.Response;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.BookingMerchants.Queries.GetBookingMerchantsWithPagination;
using Order.Application.Features.Bookings.Commands.CancelBooking;
using Order.Application.Features.Bookings.Commands.CreateBooking;
using Order.Application.Features.Bookings.Commands.DeleteBooking;
using Order.Application.Features.Bookings.Commands.UpdateBooking;
using Order.Application.Features.Bookings.Models;
using Order.Application.Features.Bookings.Queries.GetBooking;

namespace Order.Api.Controllers.V1;

[ApiVersion("1.0")]
public class BookingMerchantsController: ApiControllerBase
{
    [AccessGroup("booking.view")]
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<BookingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<BookingDto>>>> GetBookingsWithPaginationAsync([FromQuery] GetBookingMerchantsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [AccessGroup("booking.view")]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetBookingByIdQuery { Id = id });
    }

    [AccessGroup("booking.create")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> CreateAsync([FromForm] CreateBookingCommand command)
    {
        return await Mediator.Send(command);
    }

    [AccessGroup("booking.update")]
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

    [AccessGroup("booking.update")]
    [HttpPut("cancel/{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> CancelBookingAsync(int id)
    {
        return await Mediator.Send(new CancelBookingCommand { Id = id });
    }

    [AccessGroup("booking.delete")]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteAsync(int id)
    {
        return await Mediator.Send(new DeleteBookingCommand(id));
    }

}

