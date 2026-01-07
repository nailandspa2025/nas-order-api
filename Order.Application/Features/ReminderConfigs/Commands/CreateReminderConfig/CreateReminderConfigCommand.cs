using AutoMapper;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Application.Features.ReminderConfigs.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.ReminderConfigs.Commands.CreateReminderConfig;

public record CreateReminderConfigCommand : IRequest<ApiResponse<ReminderConfigDto>>
{
    public string Name { get; set; }
    public int StoreId { get; set; }
    public ReminderChannel Channel { get; set; }
    public int BeforeMinute { get; set; }
    public bool IsActive { get; set; }
}

public class CreateReminderConfigCommandHandler : IRequestHandler<CreateReminderConfigCommand, ApiResponse<ReminderConfigDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public CreateReminderConfigCommandHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ReminderConfigDto>> Handle(CreateReminderConfigCommand request, CancellationToken cancellationToken)
    {
        var entity = new ReminderConfig 
        {
            Name = request.Name,
            StoreId = request.StoreId,
            Channel = request.Channel,
            BeforeMinute = request.BeforeMinute,
            IsActive = request.IsActive
        };
        _context.ReminderConfig.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<ReminderConfigDto>.Success(_mapper.Map<ReminderConfigDto>(entity));
    }
}