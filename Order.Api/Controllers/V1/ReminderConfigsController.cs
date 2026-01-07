using BuildingBlocks.CommonAuthorization.CommonAuthorizationAttributes;
using BuildingBlocks.Core.Response;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Features.ReminderConfigs.Commands.CreateReminderConfig;
using Order.Application.Features.ReminderConfigs.Commands.DeleteReminderConfig;
using Order.Application.Features.ReminderConfigs.Commands.UpdateReminderConfig;
using Order.Application.Features.ReminderConfigs.Models;
using Order.Application.Features.ReminderConfigs.Queries.GetReminderConfig;
using Order.Application.Features.ReminderConfigs.Queries.GetReminderConfigWithPagination;

namespace Order.Api.Controllers.V1;

[ApiVersion("1.0")]
public class ReminderConfigsController : ApiControllerBase
{
    [HttpGet("pagingation")]
    [AccessGroup("config-reminder.view")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ReminderConfigDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<ReminderConfigDto>>>> GetWithPaginationAsync([FromQuery] GetReminderConfigWithPaginationQuery query)
    {
        return await Mediator.Send(query);
    }

    [HttpGet("{id}")]
    [AccessGroup("config-reminder.view")]
    [ProducesResponseType(typeof(ApiResponse<ReminderConfigDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ReminderConfigDto>>> GetByIdAsync(int id)
    {
        return await Mediator.Send(new GetReminderConfigByIdQuery { Id = id });
    }

    [HttpPost]
    [AccessGroup("config-reminder.create")]
    [ProducesResponseType(typeof(ApiResponse<ReminderConfigDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ReminderConfigDto>>> CreateAsync([FromForm] CreateReminderConfigCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    [AccessGroup("config-reminder.update")]
    [ProducesResponseType(typeof(ApiResponse<ReminderConfigDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<ReminderConfigDto>>> UpdateAsync(int id, [FromForm] UpdateReminderConfigCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }
        return await Mediator.Send(command);
    }

    [HttpDelete("{id}")]
    [AccessGroup("config-reminder.delete")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> DeleteAsync(int id)
    {
        return await Mediator.Send(new DeleteReminderConfigCommand(id));
    }
}