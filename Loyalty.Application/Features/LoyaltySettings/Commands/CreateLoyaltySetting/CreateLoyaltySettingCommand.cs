using AutoMapper;
using BuildingBlocks.Core.Response;
using Loyalty.Application.Common.Interfaces;
using Loyalty.Application.Features.LoyaltySettings.Models;
using Loyalty.Domain.Entities;
using MediatR;

namespace Loyalty.Application.Features.LoyaltySettings.Commands.CreateLoyaltySetting;

public record CreateLoyaltySettingCommand: IRequest<ApiResponse<LoyaltySettingDto>>
{
    public string Name { get; init; } = null!;
    public int MerchantId { get; init; }
}

public class CreateLoyaltySettingCommandHandler : IRequestHandler<CreateLoyaltySettingCommand, ApiResponse<LoyaltySettingDto>>
{
    private readonly ILoyaltyDbContext _context;
    private readonly IMapper _mapper;

    public CreateLoyaltySettingCommandHandler(ILoyaltyDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<LoyaltySettingDto>> Handle(CreateLoyaltySettingCommand request, CancellationToken cancellationToken)
    {
        var entity = new LoyaltySetting
        {
            Name = request.Name,
            MerchantId = request.MerchantId
        };
        _context.LoyaltySetting.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<LoyaltySettingDto>.Success(_mapper.Map<LoyaltySettingDto>(entity));
    }
}
