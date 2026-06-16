using BuildingBlocks.Core.Response;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.Commissions.Models;
using Order.Application.Features.Commissions.Queries.GetCommissionsWithPagination;

namespace Order.Api.Controllers.V1;
[ApiVersion("1.0")]
public class CommissionsController : ApiControllerBase
{
    
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<CommissionDetailDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<CommissionDetailDto>>>> GetCommissionsWithPaginationAsync([FromQuery] GetCommissionsWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }
    
    [HttpGet("merchant-pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<CommissionDetailDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<CommissionDetailDto>>>> GetCommissionForMerchantWithPaginationAsync([FromQuery] GetCommissionForMerchantWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }
}
