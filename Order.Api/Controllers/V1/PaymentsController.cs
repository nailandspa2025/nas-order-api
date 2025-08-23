using BuildingBlocks.CommonAuthorization.CommonAuthorizationAttributes;
using BuildingBlocks.Core.Response;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.Payments.Commands.CapturePaypal;
using Order.Application.Features.Payments.Commands.CretePayment;
using Order.Application.Features.Payments.Commands.UpdatePayment;
using Order.Application.Features.Payments.Models;
using Order.Application.Features.Payments.Queries.GetPayment;
using Order.Application.Features.Payments.Queries.GetPaymentForMerchantsWithPagination;
using Order.Application.Features.Payments.Queries.GetPaymentsWithPagination;

namespace Order.Api.Controllers.V1;

[ApiVersion("1.0")]
public class PaymentsController : ApiControllerBase
{
    [AccessGroup("payment.view")]
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<PaymentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<PaymentDto>>>> GetWithPaginationAsync([FromQuery] GetPaymentsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [AccessGroup("payment.view")]
    [HttpGet("merchant-pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<PaymentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<PaymentDto>>>> GetPaymentFormMaerchntWithPaginationAsync([FromQuery] GetPaymentForMerchantsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [AccessGroup("payment.view")]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetPaymentByIdQuery { Id = id });
    }

    [AccessGroup("payment.create")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> CreateAsync([FromForm] CreatePaymentCommand command)
    {
        return await Mediator.Send(command);
    }

    [AccessGroup("payment.update")]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> UpdateAsync(int id, [FromForm] UpdatePaymentCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        return await Mediator.Send(command);
    }
    [HttpGet("success")]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaymentDto>>> SuccessAsync([FromQuery] string token, [FromQuery] int bookingId)
    {
        var command = new CapturePaypalCommand
        {
            OrderId = token, 
            BookingId = bookingId
        };
        return await Mediator.Send(command);
    }
    [HttpGet("cancel")]
    public IActionResult CancelAsync()
    {
        return Redirect($"{Request.Scheme}://{Request.Host}/booking");
    }
}
