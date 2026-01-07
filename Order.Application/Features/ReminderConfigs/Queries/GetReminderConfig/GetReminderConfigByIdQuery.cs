using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Application.Features.ReminderConfigs.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.ReminderConfigs.Queries.GetReminderConfig;

public record GetReminderConfigByIdQuery : IRequest<ApiResponse<ReminderConfigDto>>
{
    public int Id { get; set; }
}

public class GetReminderConfigByIdQueryHandler : IRequestHandler<GetReminderConfigByIdQuery, ApiResponse<ReminderConfigDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public GetReminderConfigByIdQueryHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ReminderConfigDto>> Handle(GetReminderConfigByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.ReminderConfig.FindAsync(request.Id, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(ReminderConfig), request.Id);
        }
        return ApiResponse<ReminderConfigDto>.Success(_mapper.Map<ReminderConfigDto>(entity));
    }
}