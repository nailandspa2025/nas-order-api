using BuildingBlocks.CommonAuthorization.CommonAuthorizationAttributes;
using BuildingBlocks.Core.Response;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.PaymentProviders.Commands.CreatePaymentProviderCommand;
using Order.Application.Features.PaymentProviders.Commands.UodatePaymentProviderCommand;
using Order.Application.Features.PaymentProviders.Models;
using Order.Application.Features.PaymentProviders.Queries.GetPaymentProvider;
using Order.Application.Features.PaymentProviders.Queries.GetPaymentProvidersWithPagination;

namespace Order.Api.Controllers.V1;

[ApiVersion("1.0")]

public class PaymentProviderControler : ApiControllerBase
{
    [AccessGroup("payment-provider.view")]
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<PaymentProviderDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<PaymentProviderDto>>>> GetPaymentProvidersWithPaginationAsync([FromQuery] GetPaymentProvidersWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [AccessGroup("payment-provider.view")]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentProviderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaymentProviderDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetPaymentProviderByIdQuery { Id = id });
    }

    [AccessGroup("payment-provider.view")]
    [HttpGet("paymentMethod")]
    [ProducesResponseType(typeof(ApiResponse<PaymentProviderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaymentProviderDto>>> GetByMethodAsync(PaymentMethod paymentMethod)
    {
        return await Mediator.Send(new GetPaymentProviderByMethodQuery { PaymentMethod = paymentMethod });
    }

    [AccessGroup("payment-provider.create")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PaymentProviderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaymentProviderDto>>> CreateAsync([FromBody] CreatePaymentProviderCommand command)
    {
        return await Mediator.Send(command);
    }

    [AccessGroup("payment-provider..update")]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentProviderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PaymentProviderDto>>> UpdateAsync(int id, [FromBody] UpdatePaymentProviderCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        return await Mediator.Send(command);
    }
}