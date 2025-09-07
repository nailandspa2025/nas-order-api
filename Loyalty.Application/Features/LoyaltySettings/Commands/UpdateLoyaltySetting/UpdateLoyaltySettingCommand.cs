using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Identity.Technicians.Models;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using Loyalty.Application.Common.Interfaces;
using Loyalty.Application.Features.LoyaltySettings.Commands.CreateLoyaltySetting;
using Loyalty.Application.Features.LoyaltySettings.Models;
using Loyalty.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Loyalty.Application.Features.LoyaltySettings.Commands.UpdateLoyaltySetting;

public record UpdateLoyaltySettingCommand : IRequest<ApiResponse<LoyaltySettingDto>>
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public int MerchantId { get; init; }
}

public class UpdateLoyaltySettingCommandHandler : IRequestHandler<UpdateLoyaltySettingCommand, ApiResponse<LoyaltySettingDto>>
{
    private readonly ILoyaltyDbContext _context;
    private readonly IMapper _mapper;

    public UpdateLoyaltySettingCommandHandler(ILoyaltyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public  async Task<ApiResponse<LoyaltySettingDto>> Handle(UpdateLoyaltySettingCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.LoyaltySetting
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(LoyaltySetting), request.Id);
        }
        entity.Name = request.Name;
        entity.MerchantId = request.MerchantId;
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<LoyaltySettingDto>.Success(_mapper.Map<LoyaltySettingDto>(entity));
    }
}
