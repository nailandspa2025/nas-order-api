using BuildingBlocks.CommonAuthorization.CommonAuthorizationAttributes;
using BuildingBlocks.Core.Response;
using Loyalty.Application.Features.LoyaltySettings.Commands.CreateLoyaltySetting;
using Loyalty.Application.Features.LoyaltySettings.Commands.DeleteLoyaltySetting;
using Loyalty.Application.Features.LoyaltySettings.Commands.UpdateLoyaltySetting;
using Loyalty.Application.Features.LoyaltySettings.Models;
using Loyalty.Application.Features.LoyaltySettings.Queries.GetLoyaltySetting;
using Loyalty.Application.Features.LoyaltySettings.Queries.GetLoyaltySettings;
using Loyalty.Application.Features.LoyaltySettings.Queries.GetLoyaltySettingWthPagintion;
using Microsoft.AspNetCore.Mvc;

namespace Loyalty.Api.Controllers.V1;

public class LoyaltySettingsController: ApiControllerBase
{
    [AccessGroup("loyalty-setting.view")]
    [HttpGet("pagingation")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<LoyaltySettingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<LoyaltySettingDto>>>> GetWithPaginationAsync([FromQuery] GetLoyaltySettingWthPagintionQuery query)
    {
        return await Mediator.Send(query);
    }

    [AccessGroup("loyalty-setting.view")]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<LoyaltySettingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<LoyaltySettingDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetLoyaltySettingByIdQuery { Id = id });
    }

    [AccessGroup("loyalty-setting.view")]
    [HttpGet("ids")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<LoyaltySettingDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<LoyaltySettingDto>>>> GetByIdsAsync(string ids)
    {
        return await Mediator.Send(new GetLoyaltySettingByIdsQuery { Ids = ids });
    }

    [AccessGroup("loyalty-setting.create")]
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<LoyaltySettingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<LoyaltySettingDto>>> CreateAsync([FromForm] CreateLoyaltySettingCommand command)
    {
        return await Mediator.Send(command);
    }

    [AccessGroup("loyalty-setting.update")]
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<LoyaltySettingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<LoyaltySettingDto>>> UpdateAsync(int id, [FromForm] UpdateLoyaltySettingCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        return await Mediator.Send(command);
    }

    [AccessGroup("loyalty-setting.delete")]
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteAsync(int id)
    {
        return await Mediator.Send(new DeleteLoyaltySettingCommand(id));
    }
}