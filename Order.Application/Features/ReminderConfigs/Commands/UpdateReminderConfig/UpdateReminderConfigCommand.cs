using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Application.Features.ReminderConfigs.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.ReminderConfigs.Commands.UpdateReminderConfig;

public record UpdateReminderConfigCommand : IRequest<ApiResponse<ReminderConfigDto>>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int StoreId { get; set; }
    public ReminderChannel Channel { get; set; }
    public int BeforeMinute { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateReminderConfigCommandHandler : IRequestHandler<UpdateReminderConfigCommand, ApiResponse<ReminderConfigDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public UpdateReminderConfigCommandHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ReminderConfigDto>> Handle(UpdateReminderConfigCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ReminderConfig.FindAsync(request.Id, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(ReminderConfig), request.Id);
        }
        entity.Name = request.Name;
        entity.StoreId = request.StoreId;
        entity.Channel = request.Channel;
        entity.BeforeMinute = request.BeforeMinute;
        entity.IsActive = request.IsActive;
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<ReminderConfigDto>.Success(_mapper.Map<ReminderConfigDto>(entity));
    }
}