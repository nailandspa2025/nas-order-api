using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Loyalty.Application.Common.Interfaces;
using Loyalty.Domain.Entities;
using MediatR;

namespace Loyalty.Application.Features.LoyaltySettings.Commands.DeleteLoyaltySetting;

public record DeleteLoyaltySettingCommand(long Id) : IRequest<ApiResponse>;

public class DeleteLoyaltySettingCommandHandler : IRequestHandler<DeleteLoyaltySettingCommand, ApiResponse>
{
    private readonly ILoyaltyDbContext _context;

    public DeleteLoyaltySettingCommandHandler (ILoyaltyDbContext context)
    {
        _context = context;
    }
    public async Task<ApiResponse> Handle(DeleteLoyaltySettingCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.LoyaltySetting
           .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(LoyaltySetting), request.Id);
        }

        _context.LoyaltySetting.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }
}

