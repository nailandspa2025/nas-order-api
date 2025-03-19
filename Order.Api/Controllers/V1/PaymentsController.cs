using BuildingBlocks.Core.Response;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.Bookings.Commands.CreateBooking;
using Order.Application.Features.Bookings.Models;

namespace Order.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    public class PaymentsController : ApiControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<BookingDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<BookingDto>>> CreateAsync( [FromForm] CreateBookingCommand command)
        {
            return await Mediator.Send(command);
        }
    }
}
