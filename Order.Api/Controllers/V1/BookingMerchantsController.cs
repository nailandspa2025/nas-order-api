using BuildingBlocks.CommonAuthorization.CommonAuthorizationAttributes;
using BuildingBlocks.Core.Response;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.BookingMerchants.Queries.GetBookingMerchantsWithPagination;
using Order.Application.Features.Bookings.Models;

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
}

