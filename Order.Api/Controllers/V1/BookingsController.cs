using BuildingBlocks.CommonAuthorization.CommonAuthorizationAttributes;
using BuildingBlocks.Core.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.Bookings.Commands.CancelBooking;
using Order.Application.Features.Bookings.Commands.CreateBooking;
using Order.Application.Features.Bookings.Commands.CreateBookingMobile;
using Order.Application.Features.Bookings.Commands.DeleteBooking;
using Order.Application.Features.Bookings.Commands.UpdateBooking;
using Order.Application.Features.Bookings.Commands.UpdateBookingMobile;
using Order.Application.Features.Bookings.Commands.UpdateRateBooking;
using Order.Application.Features.Bookings.Models;
using Order.Application.Features.Bookings.Queries.GetBooking;
using Order.Application.Features.Bookings.Queries.GetBookingForMerchantsWithPagination;
using Order.Application.Features.Bookings.Queries.GetBookings;
using Order.Application.Features.Bookings.Queries.GetBookingsWithPagination;
using Order.Application.Features.Payments.Commands.CretePayment;
using Order.Application.Features.Payments.Models;

namespace Order.Api.Controllers.V1;

[ApiVersion("1.0")]
public class BookingsController : ApiControllerBase
{
    [AccessGroup("booking.view")]
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<BookingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<BookingDto>>>> GetBookingsWithPaginationAsync([FromQuery] GetBookingsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [AccessGroup("booking.view")]
    [HttpGet("merchant-pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<BookingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<BookingDto>>>> GetBookingForMerchantsWithPaginationAsync([FromQuery] GetBookingForMerchantsWithPaginationQuery query)
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
    public async Task<ActionResult<ApiResponse>> CancelBookingAsync(int id, [FromForm] CancelBookingCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        return await Mediator.Send(command);
    }

    [AccessGroup("booking.delete")]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteAsync(int id)
    {
        return await Mediator.Send(new DeleteBookingCommand(id));
    }

    [HttpPut("update/{id}")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> UpdateForMobileAsync(int id, [FromForm] UpdateBookingMoileCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        return await Mediator.Send(command);
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> CreateMobleAsync([FromForm] CreateBookingMobileCommand command)
    {
        return await Mediator.Send(command);
    }
    
    [HttpGet("mobile/{id}")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> GetForMobileByIdAsync(int id)
    {
        return await Mediator.Send(new GetBookingByIdQuery { Id = id });
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<BookingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<BookingDto>>>> GetBookingsMeWithPaginationAsync([FromQuery] GetBookingsMeQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpPut("cancel-mobile/{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> CancelBookingMobileAsync(int id, [FromForm] CancelBookingCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        return await Mediator.Send(command);
    }

    [HttpGet("storeIds/{storeIds}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BookingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<BookingDto>>>> GetByStoreIdsAsync(string storeIds)
    {
        return await Mediator.Send(new GetBookingByStoreIdsQuery { StoreIds = storeIds });
    }

    [AccessGroup("booking.payment")]
    [HttpPost("payment")]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> CreatePaymentAsync([FromForm] CreatePaymentCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPost("mobile-payment")]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> CreatePaymentForMobileAsync([FromForm] CreatePaymentCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet("technician")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BookingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<BookingDto>>>> GetTechnicianBookingsAsync([FromQuery] GetBookingTechnicianByStoreIdQuery command)
    {
        return await Mediator.Send(command);
    }

    [AllowAnonymous]
    [HttpPut("rated/{id}")]
    [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<BookingDto>>> UpdateRateAsync(int id, [FromForm] UpdateRateBookingCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        return await Mediator.Send(command);
    }
}
