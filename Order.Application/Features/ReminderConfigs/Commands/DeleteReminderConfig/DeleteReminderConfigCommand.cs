using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;

namespace Order.Application.Features.ReminderConfigs.Commands.DeleteReminderConfig;

public record DeleteReminderConfigCommand(int Id) : IRequest<ApiResponse>
{
    public int Id { get; set; }
}

public class DeleteReminderConfigCommandHandler : IRequestHandler<DeleteReminderConfigCommand, ApiResponse>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public DeleteReminderConfigCommandHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse> Handle(DeleteReminderConfigCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ReminderConfig.FindAsync(request.Id, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(ReminderConfig), request.Id);
        }
        _context.ReminderConfig.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse.Success();
    }
}